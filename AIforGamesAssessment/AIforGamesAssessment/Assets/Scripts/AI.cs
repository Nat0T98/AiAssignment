using System.Collections.Generic;
using UnityEngine;

/*****************************************************************************************************************************
 * Write your core AI code in this file here. The private variable 'agentScript' contains all the agents actions which are listed
 * below. Ensure your code it clear and organised and commented.
 *
 * Unity Tags
 * public static class Tags
 * public const string BlueTeam = "Blue Team";	The tag assigned to blue team members.
 * public const string RedTeam = "Red Team";	The tag assigned to red team members.
 * public const string Collectable = "Collectable";	The tag assigned to collectable items (health kit and power up).
 * public const string Flag = "Flag";	The tag assigned to the flags, blue or red.
 * 
 * Unity GameObject names
 * public static class Names
 * public const string PowerUp = "Power Up";	Power up name
 * public const string HealthKit = "Health Kit";	Health kit name.
 * public const string BlueFlag = "Blue Flag";	The blue teams flag name.
 * public const string RedFlag = "Red Flag";	The red teams flag name.
 * public const string RedBase = "Red Base";    The red teams base name.
 * public const string BlueBase = "Blue Base";  The blue teams base name.
 * public const string BlueTeamMember1 = "Blue Team Member 1";	Blue team member 1 name.
 * public const string BlueTeamMember2 = "Blue Team Member 2";	Blue team member 2 name.
 * public const string BlueTeamMember3 = "Blue Team Member 3";	Blue team member 3 name.
 * public const string RedTeamMember1 = "Red Team Member 1";	Red team member 1 name.
 * public const string RedTeamMember2 = "Red Team Member 2";	Red team member 2 name.
 * public const string RedTeamMember3 = "Red Team Member 3";	Red team member 3 name.
 * 
 * _agentData properties and public variables
 * public bool IsPoweredUp;	Have we powered up, true if we’re powered up, false otherwise.
 * public int CurrentHitPoints;	Our current hit points as an integer
 * public bool HasFriendlyFlag;	True if we have collected our own flag
 * public bool HasEnemyFlag;	True if we have collected the enemy flag
 * public GameObject FriendlyBase; The friendly base GameObject
 * public GameObject EnemyBase;    The enemy base GameObject
 * public int FriendlyScore; The friendly teams score
 * public int EnemyScore;       The enemy teams score
 * 
 * _agentActions methods
 * public bool MoveTo(GameObject target)	Move towards a target object. Takes a GameObject representing the target object as a parameter. Returns true if the location is on the navmesh, false otherwise.
 * public bool MoveTo(Vector3 target)	Move towards a target location. Takes a Vector3 representing the destination as a parameter. Returns true if the location is on the navmesh, false otherwise.
 * public bool MoveToRandomLocation()	Move to a random location on the level, returns true if the location is on the navmesh, false otherwise.
 * public void CollectItem(GameObject item)	Pick up an item from the level which is within reach of the agent and put it in the inventory. Takes a GameObject representing the item as a parameter.
 * public void DropItem(GameObject item)	Drop an inventory item or the flag at the agents’ location. Takes a GameObject representing the item as a parameter.
 * public void UseItem(GameObject item)	Use an item in the inventory (currently only health kit or power up). Takes a GameObject representing the item to use as a parameter.
 * public void AttackEnemy(GameObject enemy)	Attack the enemy if they are close enough. ). Takes a GameObject representing the enemy as a parameter.
 * public void Flee(GameObject enemy)	Move in the opposite direction to the enemy. Takes a GameObject representing the enemy as a parameter.
 * 
 * _agentSenses properties and methods
 * public List<GameObject> GetObjectsInViewByTag(string tag)	Return a list of objects with the same tag. Takes a string representing the Unity tag. Returns null if no objects with the specified tag are in view.
 * public GameObject GetObjectInViewByName(string name)	Returns a specific GameObject by name in view range. Takes a string representing the objects Unity name as a parameter. Returns null if named object is not in view.
 * public List<GameObject> GetObjectsInView()	Returns a list of objects within view range. Returns null if no objects are in view.
 * public List<GameObject> GetCollectablesInView()	Returns a list of objects with the tag Collectable within view range. Returns null if no collectable objects are in view.
 * public List<GameObject> GetFriendliesInView()	Returns a list of friendly team AI agents within view range. Returns null if no friendlies are in view.
 * public List<GameObject> GetEnemiesInView()	Returns a list of enemy team AI agents within view range. Returns null if no enemies are in view.
 * public GameObject GetNearestEnemyInView()   Returns the nearest enemy AI in view to the agent. Returns null if no enemies are in view.
 * public bool IsItemInReach(GameObject item)	Checks if we are close enough to a specific collectible item to pick it up), returns true if the object is close enough, false otherwise.
 * public bool IsInAttackRange(GameObject target)	Check if we're with attacking range of the target), returns true if the target is in range, false otherwise.
 * public Vector3 GetVectorToTarget(GameObject target)  Return a normalised vector pointing to the target GameObject
 * public Vector3 GetVectorToTarget(Vector3 target)     Return a normalised vector pointing to the target vector
 * public Vector3 GetFleeVectorFromTarget(GameObject target)    Return a normalised vector pointing away from the target GameObject
 * public Vector3 GetFleeVectorFromTarget(Vector3 target)   Return a normalised vector pointing away from the target vector
 * 
 * _agentInventory properties and methods
 * public bool AddItem(GameObject item)	Adds an item to the inventory if there's enough room (max capacity is 'Constants.InventorySize'), returns true if the item has been successfully added to the inventory, false otherwise.
 * public GameObject GetItem(string itemName)	Retrieves an item from the inventory as a GameObject, returns null if the item is not in the inventory.
 * public bool HasItem(string itemName)	Checks if an item is stored in the inventory, returns true if the item is in the inventory, false otherwise.
 * 
 * You can use the game objects name to access a GameObject from the sensing system. Thereafter all methods require the GameObject as a parameter.
 * 
*****************************************************************************************************************************/

/// <summary>
/// Implement your AI script here, you can put everything in this file, or better still, break your code up into multiple files
/// and call your script here in the Update method. This class includes all the data members you need to control your AI agent
/// get information about the world, manage the AI inventory and access essential information about your AI.
///
/// You may use any AI algorithm you like, but please try to write your code properly and professionaly and don't use code obtained from
/// other sources, including the labs.
///
/// See the assessment brief for more details
/// </summary>
public class AI : MonoBehaviour
{
    // Gives access to important data about the AI agent (see above)
    private AgentData _agentData;
    // Gives access to the agent senses
    private Sensing _agentSenses;
    // gives access to the agents inventory
    private InventoryController _agentInventory;
    // This is the script containing the AI agents actions
    // e.g. agentScript.MoveTo(enemy);
    private AgentActions _agentActions;

    //Base node
    Selector Nodes;

    // Use this for initialization
    void Start()
    {
        // Initialise the accessable script components
        _agentData = GetComponent<AgentData>();
        _agentActions = GetComponent<AgentActions>();
        _agentSenses = GetComponentInChildren<Sensing>();
        _agentInventory = GetComponentInChildren<InventoryController>();

        //Behaviour Tree Creation

        #region Condition Checks
        // Flag At Base
        Checks.FlagAtBase FlagAtFriendlyBase;
        Checks.FlagAtBase FlagAtEnemyBase;
        if (_agentData.FriendlyTeam == AgentData.Teams.BlueTeam)
        {
            FlagAtEnemyBase = new Checks.FlagAtBase(_agentData.GetGlobalBlackboard().GetRedTeamBlackboard());
            FlagAtFriendlyBase = new Checks.FlagAtBase(_agentData.GetGlobalBlackboard().GetBlueTeamBlackboard());
        }
        else
        {
            FlagAtEnemyBase = new Checks.FlagAtBase(_agentData.GetGlobalBlackboard().GetBlueTeamBlackboard());
            FlagAtFriendlyBase = new Checks.FlagAtBase(_agentData.GetGlobalBlackboard().GetRedTeamBlackboard());
        }
        // Team Has Flag
        Checks.TeamHasFlag FriendlyHasEnemyFlag = new Checks.TeamHasFlag(_agentData.GetTeamBlackboard(), Team.ENEMY);
        Checks.TeamHasFlag FriendlyHasFriendlyFlag = new Checks.TeamHasFlag(_agentData.GetTeamBlackboard(), Team.FRIENDLY);
        Checks.TeamHasFlag EnemyHasFlag;
        // The blue team need to check the red team blackboard and vice versa
        if (_agentData.FriendlyTeam == AgentData.Teams.BlueTeam)
        {
            EnemyHasFlag = new Checks.TeamHasFlag(_agentData.GetGlobalBlackboard().GetRedTeamBlackboard(), Team.ENEMY);
        }
        else
        {
            EnemyHasFlag = new Checks.TeamHasFlag(_agentData.GetGlobalBlackboard().GetBlueTeamBlackboard(), Team.ENEMY);
        }

        Checks.TeamMemberChasingFlag TeamMemberPursuingFlag = new Checks.TeamMemberChasingFlag(_agentData);

        // Collectable in Pickup Range
        Checks.isPickUpInRange EnemyFlagInRange = new Checks.isPickUpInRange(_agentData, _agentSenses, Items.ENEMY_FLAG);
        Checks.isPickUpInRange FriendlyFlagInRange = new Checks.isPickUpInRange(_agentData, _agentSenses, Items.FRIENDLY_FLAG);
        Checks.isPickUpInRange HealthInRange = new Checks.isPickUpInRange(_agentData, _agentSenses, Items.HEALTH);
        Checks.isPickUpInRange PowerInRange = new Checks.isPickUpInRange(_agentData, _agentSenses, Items.POWER);

        //Has Item(s)
        Checks.HasItem HasEnemyFlag = new Checks.HasItem(_agentData, _agentInventory, Items.ENEMY_FLAG);
        Checks.HasItem HasFriendlyFlag = new Checks.HasItem(_agentData, _agentInventory, Items.FRIENDLY_FLAG);
        Checks.HasItem HasHealth = new Checks.HasItem(_agentData, _agentInventory, Items.HEALTH);
        Checks.HasItem HasPower = new Checks.HasItem(_agentData, _agentInventory, Items.POWER);

        // Enemy in Attack Range
        Checks.EnemyInAttackRange EnemyInAttackRange = new Checks.EnemyInAttackRange(_agentSenses);

        // Agent Health Less Than
        Checks.AgentHeathLessThan ThisAgentHealthCheck = new Checks.AgentHeathLessThan(_agentData, _agentData.GetTeamBlackboard(), Objects.THIS_AGENT, 30);
        Checks.AgentHeathLessThan WeakestMember = new Checks.AgentHeathLessThan(_agentData, _agentData.GetTeamBlackboard(), Objects.WEAKEST_FRIENDLY, 40);
        Checks.HealthNearWeakest HealthNearWeakest = new Checks.HealthNearWeakest(_agentData.GetTeamBlackboard());
        // Useable on Level
        Checks.PickUpAvailable HealthAvailable = new Checks.PickUpAvailable(PickUps.HEALTH);
        Checks.PickUpAvailable PowerAvailable = new Checks.PickUpAvailable(PickUps.POWER);

       

        //Inverters
        Inverter NotGotFriendlyFlag = new Inverter(HasFriendlyFlag);
        Inverter NotGotEnemyFlag = new Inverter(HasEnemyFlag);
        Inverter NoFriendlyHasFriendlyFlag = new Inverter(FriendlyHasFriendlyFlag);
        Inverter NoFriendlyHasEnemyFlag = new Inverter(FriendlyHasEnemyFlag);
        Inverter NoTeamMemberPursuingFlag = new Inverter(TeamMemberPursuingFlag);
        Inverter NoFlagAtFriendlyBase = new Inverter(FlagAtFriendlyBase);
        Inverter NoFlagAtEnemyBase = new Inverter(FlagAtEnemyBase);
        Inverter NoHealthNearWeakest = new Inverter(HealthNearWeakest);
        #endregion

        #region Actions


        //Move Towards Obj
        Actions.MoveToObj MoveToEnemyFlag = new Actions.MoveToObj(_agentActions, _agentData, _agentSenses, Objects.ENEMY_FLAG);
        Actions.MoveToObj MoveToFriendlyFlag = new Actions.MoveToObj(_agentActions, _agentData, _agentSenses, Objects.FRIENDLY_FLAG);
        Actions.MoveToObj MoveToHealth = new Actions.MoveToObj(_agentActions, _agentData, _agentSenses, Objects.HEALTH_PICKUP);
        Actions.MoveToObj MoveToPower = new Actions.MoveToObj(_agentActions, _agentData, _agentSenses, Objects.POWER_PICKUP);
        Actions.MoveToObj MoveToNearestEnemy = new Actions.MoveToObj(_agentActions, _agentData, _agentSenses, Objects.NEAREST_ENEMY);
        Actions.MoveToObj MoveToFriendlyWithFlag = new Actions.MoveToObj(_agentActions, _agentData, _agentSenses, Objects.FRIENDLY_WITH_FLAG);
        Actions.MoveToObj MoveToWeakestFriendly = new Actions.MoveToObj(_agentActions, _agentData, _agentSenses, Objects.WEAKEST_FRIENDLY);
        Actions.MoveToObj MoveToBase = new Actions.MoveToObj(_agentActions, _agentData, _agentSenses, Objects.BASE);
        Actions.MoveToObj MoveToNotInBase = new Actions.MoveToObj(_agentActions, _agentData, _agentSenses, Objects.NOT_IN_BASE);

        // Attack
        Actions.Attack Attack = new Actions.Attack(_agentSenses, _agentActions);
        // Flee
        Actions.Flee Flee = new Actions.Flee(_agentActions, _agentSenses);

        //Collect Items
        Actions.CollectItem PickUpEnemyFlag = new Actions.CollectItem(_agentActions, _agentData, _agentSenses, Items.ENEMY_FLAG);
        Actions.CollectItem PickUpFriendlyFlag = new Actions.CollectItem(_agentActions, _agentData, _agentSenses, Items.FRIENDLY_FLAG);
        Actions.CollectItem PickUpHealth = new Actions.CollectItem(_agentActions, _agentData, _agentSenses, Items.HEALTH);
        Actions.CollectItem PickUpPower = new Actions.CollectItem(_agentActions, _agentData, _agentSenses, Items.POWER);

        //Use Pick Ups
        Actions.UsePickUp UseHealth = new Actions.UsePickUp(_agentActions, _agentData, _agentInventory, PickUps.HEALTH);
        Actions.UsePickUp UsePower = new Actions.UsePickUp(_agentActions, _agentData, _agentInventory, PickUps.POWER);

        //Drop Item
        Actions.DropItem DropHealth = new Actions.DropItem(_agentActions, _agentData, _agentInventory, Items.HEALTH);
        Actions.DropItem DropEnemyFlag = new Actions.DropItem(_agentActions, _agentData, _agentInventory, Items.ENEMY_FLAG);
        Actions.DropItem DropFriendlyFlag = new Actions.DropItem(_agentActions, _agentData, _agentInventory, Items.FRIENDLY_FLAG);

        #endregion // Declare Actions

        #region Behaviour Tree Branches
        // Grab Item
        Sequence GrabHealth = new Sequence(new List<Node> { HealthInRange, PickUpHealth });
        Sequence GrabPower = new Sequence(new List<Node> { PowerInRange, PickUpPower });
        Sequence GrabConsideringAid = new Sequence(new List<Node> { NoHealthNearWeakest, GrabHealth });
        Selector GrabItem = new Selector(new List<Node> { GrabConsideringAid, GrabPower });

        // Attack Enemy
        Sequence PowerAttack = new Sequence(new List<Node> { HasPower, UsePower, Attack });
        Selector DoAttack = new Selector(new List<Node> { PowerAttack, Attack });
        Sequence AttackEnemy = new Sequence(new List<Node> { EnemyInAttackRange, DoAttack });

        // Get Flag
        Sequence GetEnemyFlag = new Sequence(new List<Node> { NotGotFriendlyFlag, NoFlagAtFriendlyBase, MoveToEnemyFlag, EnemyFlagInRange, PickUpEnemyFlag });

        // Stock Up
        Sequence GetHealth = new Sequence(new List<Node> { HealthAvailable, MoveToHealth, GrabHealth });
        Sequence GetPower = new Sequence(new List<Node> { PowerAvailable, MoveToPower, GrabPower });
        Selector StockUp = new Selector(new List<Node> { GetHealth, GetPower });

        // Protect Flag
        Sequence AttackNearestEnemy = new Sequence(new List<Node> { MoveToNearestEnemy, AttackEnemy });
        Sequence ProtectEnemyFlag = new Sequence(new List<Node> { FriendlyHasEnemyFlag, MoveToFriendlyWithFlag, AttackNearestEnemy });

        /*// Aid
        Sequence GivePack = new Sequence(new List<Node> { HasHealth, MoveToWeakestFriendly, DropHealth });
        Sequence FindPack = new Sequence(new List<Node> { HealthAvailable, MoveToHealth, GrabHealth, MoveToWeakestFriendly, DropHealth });
        Selector ProvideHealthPack = new Selector(new List<Node> { GivePack, FindPack });
        Sequence Aid = new Sequence(new List<Node> { WeakestMember, ProvideHealthPack });*/

        // Save Flag
        Sequence SaveFriendlyFlag = new Sequence(new List<Node> { EnemyHasFlag, MoveToFriendlyFlag, AttackEnemy });

        // Remove Friendly Flag (from enemy base) 
        Sequence RemoveFriendlyFlag = new Sequence(new List<Node> { NotGotEnemyFlag, NoFriendlyHasFriendlyFlag, FlagAtEnemyBase, MoveToFriendlyFlag, FriendlyFlagInRange, PickUpFriendlyFlag, MoveToNotInBase });
        // If we picked up the friendly flag in order to remove it from the enemy base then drop it
        Sequence PutFriendlyFlagDown = new Sequence(new List<Node> { HasFriendlyFlag, NoFlagAtEnemyBase, DropFriendlyFlag });

        // Pursue Flag
        Sequence PursueEnemyFlag = new Sequence(new List<Node> { NoFriendlyHasEnemyFlag, NoTeamMemberPursuingFlag, GetEnemyFlag });

        // Protect Self
        Sequence UseHealthPack = new Sequence(new List<Node> { HasHealth, UseHealth });
        Sequence GetHealthPack = new Sequence(new List<Node> { HealthAvailable, MoveToHealth, GrabHealth, HasHealth, UseHealth });
        Sequence Escape = new Sequence(new List<Node> { EnemyInAttackRange, Flee });
        Selector ProtectHealth = new Selector(new List<Node> { UseHealthPack, GetHealthPack, Escape });
        Sequence ProtectSelf = new Sequence(new List<Node> { ThisAgentHealthCheck, ProtectHealth });

        // Return Flag
        Sequence ReturnEnemyFlag = new Sequence(new List<Node> { HasEnemyFlag, MoveToBase, DropEnemyFlag });

        // Friendly Flag Defense
        Selector FriendlyFlagDefence = new Selector(new List<Node> { PutFriendlyFlagDown, RemoveFriendlyFlag, SaveFriendlyFlag });
        #endregion // Create Branches

        //Node that AI will use to select branch
        Nodes = new Selector(new List<Node> {
            GrabItem, ReturnEnemyFlag, ProtectSelf, 
            PursueEnemyFlag, FriendlyFlagDefence, 
            ProtectEnemyFlag, StockUp, 
            GetEnemyFlag, AttackNearestEnemy });

    }

    // Update is called once per frame
    void Update()
    {
        // Run your AI code in here
        Nodes.Evaluate();
    }  

}