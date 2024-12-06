using System.Collections.Generic;
using UnityEngine;


public enum Items
{
    ENEMY_FLAG,
    FRIENDLY_FLAG,
    HEALTH,
    POWER,
}
public enum GameObjects
{
    ENEMY_FLAG,
    FRIENDLY_FLAG,

    HEALTH_PACK,
    POWER_PACK,

    NEAREST_ENEMY,
    FRIENDLY_WITH_FLAG,
    WEAKEST_FRIENDLY,
    THIS_AGENT,

    BASE,
    NOT_IN_BASE,
}
public enum PickUps
{
    HEALTH,
    POWER,
}
public enum Team
{
    ENEMY,
    FRIENDLY,
}



public class Actions
{
    // Pick up the nearest Collectable_Type of collectable
    public class CollectItem : Node
    {
        private AgentActions agentActions;
        private AgentData agentData;
        private Sensing sensing;
        private Items type;

        public CollectItem(AgentActions agentActions, AgentData agentData, Sensing sensing, Items type)
        {
            this.agentActions = agentActions;
            this.agentData = agentData;
            this.sensing = sensing;
            this.type = type;
        }
        public override NodeState Evaluate()
        {
#if DEBUG
            Debug.Log("Pick Up Collectable");
#endif //Debug
            // What collectables can the agent see
            List<GameObject> collectablesInView = sensing.GetObjectsInView();
            for (int i = 0; i < collectablesInView.Count; i++)
            {
                // Are any of them in reach and of the type we're looking for
                if (sensing.IsItemInReach(collectablesInView[i]) &&
                        type == Items.FRIENDLY_FLAG && collectablesInView[i].name.Equals(agentData.FriendlyFlagName) ||
                        type == Items.ENEMY_FLAG && collectablesInView[i].name.Equals(agentData.EnemyFlagName) ||
                        type == Items.POWER && collectablesInView[i].name.Equals("Power Up") ||
                        type == Items.HEALTH && collectablesInView[i].name.Equals("Health Kit"))
                {
                    // If yes then pick it up
                    agentActions.CollectItem(collectablesInView[i]);
                    return NodeState.SUCCESS;
                }
            }
            // We have failed to find a collectable
#if DEBUG
            Debug.LogError("Failed to find a collectable");
#endif //Debug
            return NodeState.FAILURE;
        }
    }

    // Drop the first type of that collectable held
    public class DropItem : Node
    {
        private AgentActions agentActions;
        private AgentData agentData;
        private InventoryController inventoryController;
        private Items type;

        public DropItem(AgentActions agentActions, AgentData agentData, InventoryController inventoryController, Items type)
        {
            this.agentActions = agentActions;
            this.agentData = agentData;
            this.inventoryController = inventoryController;
            this.type = type;
        }
        public override NodeState Evaluate()
        {
#if DEBUG
            Debug.Log("Drop Collectable");
#endif //DEBUG
            // Get the item of Collectable_Type from the inventory
            GameObject collectable = null;
            switch (type)
            {
                case Items.FRIENDLY_FLAG:
                    collectable = inventoryController.GetItem(agentData.FriendlyFlagName);
                    break;
                case Items.ENEMY_FLAG:
                    collectable = inventoryController.GetItem(agentData.EnemyFlagName);
                    break;
                case Items.HEALTH:
                    collectable = inventoryController.GetItem("Health Kit");
                    break;
                case Items.POWER:
                    collectable = inventoryController.GetItem("Power Up");
                    break;
            }
            // Drop the item
            if (collectable)
            {
                agentActions.DropItem(collectable);
                return NodeState.SUCCESS;
            }
#if DEBUG
            Debug.LogWarning("No collectable found to drop (probably just due to agent death");
#endif //DEBUG
            return NodeState.FAILURE;
        }
    }

    // Move to the nearest of that type of game object
    public class MoveTowardsGO : Node
    {
        private AgentActions agentActions;
        private AgentData agentData;
        private Sensing sensing;
        private GameObjects type;

        public MoveTowardsGO(AgentActions agentActions, AgentData agentData, Sensing sensing, GameObjects type)
        {
            this.agentActions = agentActions;
            this.agentData = agentData;
            this.sensing = sensing;
            this.type = type;
        }
        public override NodeState Evaluate()
        {
#if DEBUG
            Debug.Log("Move To Game Object");
#endif //DEBUG
            // Get the target according to GameObject_Type
            GameObject target = null;
            bool check_if_in_reach = false;
            bool check_if_in_attack_range = false;
            switch (type)
            {
                case GameObjects.ENEMY_FLAG:
                    target = agentData.GetTeamBlackboard().GetEnemyFlag();
                    check_if_in_reach = true;
                    break;
                case GameObjects.FRIENDLY_FLAG:
                    target = agentData.GetTeamBlackboard().GetFriendlyFlag();
                    check_if_in_attack_range = true;
                    break;
                case GameObjects.HEALTH_PACK:
                    target = GetCollectableTarget("Health Kit");
                    check_if_in_reach = true;
                    break;
                case GameObjects.POWER_PACK:
                    target = GetCollectableTarget("Power Up");
                    check_if_in_reach = true;
                    break;
                case GameObjects.NEAREST_ENEMY:
                    target = sensing.GetNearestEnemyInView();
                    check_if_in_attack_range = true;
                    break;
                case GameObjects.FRIENDLY_WITH_FLAG:
                    target = agentData.GetTeamBlackboard().GetMemberWithEnemyFlag();
                    check_if_in_attack_range = true;
                    break;
                case GameObjects.WEAKEST_FRIENDLY:
                    target = agentData.GetTeamBlackboard().GetWeakestMember();
                    check_if_in_reach = true;
                    break;
                case GameObjects.BASE:
                    target = agentData.FriendlyBase;
                    check_if_in_attack_range = true;
                    break;
                case GameObjects.NOT_IN_BASE:
                    check_if_in_reach = true;
                    break;
            }

            // Not in base is for moving the flag to a random location outside of the base (it will succeed due to the check before it)
            if (type == GameObjects.NOT_IN_BASE)
            {
                agentActions.MoveToRandomLocation();
                return NodeState.RUNNING;
            }

            // Move to the target
            if (check_if_in_reach && sensing.IsItemInReach(target) ||
                check_if_in_attack_range && sensing.IsInAttackRange(target))
            {
                return NodeState.SUCCESS;
            }
            agentActions.MoveTo(target);
            return NodeState.RUNNING;
        }
        // Get a collectable with the name 'name' in the agent's view
        private GameObject GetCollectableTarget(string name)
        {
            List<GameObject> collectablesInView = sensing.GetCollectablesInView();
            for (int i = 0; i < collectablesInView.Count; i++)
            {
                if (collectablesInView[i].name.Equals(name))
                {
                    return collectablesInView[i];
                }
            }
            return null;
        }
    }

    // Flee the nearest enemy
    public class Flee : Node
    {
        private AgentActions agentActions;
        private Sensing sensing;

        public Flee(AgentActions agentActions, Sensing sensing)
        {
            this.agentActions = agentActions;
            this.sensing = sensing;

        }
        public override NodeState Evaluate()
        {
#if DEBUG
            Debug.Log("Flee");
#endif //DEBUG
            GameObject nearestEnemy = sensing.GetNearestEnemyInView();
            if (!sensing.IsInAttackRange(nearestEnemy))
            {
                return NodeState.SUCCESS;
            }
            agentActions.Flee(nearestEnemy);
            return NodeState.RUNNING;
        }
    }

    // Use the first type of that useable in inventory
    public class UsePickUp : Node
    {
        private AgentActions agentActions;
        private AgentData agentData;
        private InventoryController inventoryController;
        private PickUps type;

        public UsePickUp(AgentActions agentActions, AgentData agentData, InventoryController inventoryController, PickUps type)
        {
            this.agentActions = agentActions;
            this.agentData = agentData;
            this.inventoryController = inventoryController;
            this.type = type;
        }
        public override NodeState Evaluate()
        {
            // Get an item of Useable_Type from our inventory
            GameObject item = null;
            switch (type)
            {
                case PickUps.HEALTH:
#if DEBUG
                    Debug.Log("Use health kit");
#endif //DEBUG
                    item = inventoryController.GetItem("Health Kit");
                    break;
                case PickUps.POWER:
#if DEBUG
                    Debug.Log("Use power up");
#endif //DEBUG
                    item = inventoryController.GetItem("Power Up");
                    break;
            }

            // Use the item
            agentActions.UseItem(item);
            return NodeState.SUCCESS;
        }
    }

    // Attack the nearest enemy
    public class Attack : Node
    {
        Sensing sensing;
        AgentActions agentActions;

        public Attack(Sensing sensing, AgentActions agentActions)
        {
            this.sensing = sensing;
            this.agentActions = agentActions;
        }

        public override NodeState Evaluate()
        {
#if DEBUG
            Debug.Log("Attack");
#endif //DEBUG
            GameObject nearestEnemy = sensing.GetNearestEnemyInView();
            agentActions.AttackEnemy(nearestEnemy);
            if (!sensing.IsInAttackRange(sensing.GetNearestEnemyInView()))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.RUNNING;
        }
    }
}



public class Conditions
{
    // Check agent health
    public class AgentHeathLessThan : Node
    {
        private AgentData agentData;
        private TeamBlackboard teamBlackboard;
        GameObjects type;
        int value;

        public AgentHeathLessThan(AgentData agentData, TeamBlackboard teamBlackboard, GameObjects type, int value)
        {
            this.agentData = agentData;
            this.teamBlackboard = teamBlackboard;
            this.type = type;
            this.value = value;
        }
        public override NodeState Evaluate()
        {
            bool result = false;
            switch (type)
            {
                case GameObjects.WEAKEST_FRIENDLY:

                    if (teamBlackboard.GetWeakestMember() && teamBlackboard.GetWeakestMember() != agentData.gameObject)
                    {
                        result = teamBlackboard.GetWeakestMember().GetComponent<AgentData>().CurrentHitPoints < value;
                    }
                    break;
                case GameObjects.THIS_AGENT:
                    result = agentData.CurrentHitPoints < value;
                    break;
            }
            if (result)
            {
                return NodeState.SUCCESS;
            }
            else
            {

                return NodeState.FAILURE;
            }
        }
    }

    // Check if there is a Pick Up of type available
    public class PickUpAvailable : Node
    {
        PickUps type;

        public PickUpAvailable(PickUps type)
        {
            this.type = type;
        }
        public override NodeState Evaluate()
        {
            bool result = false;
            switch (type)
            {
                case PickUps.HEALTH:

                    Debug.Log("Is there a health pack available");

                    HealthKit[] health = GameObject.FindObjectsOfType<HealthKit>();
                    for (int i = 0; i < health.Length; i++)
                    {
                        if (!health[i].transform.parent)
                        {
                            result = true;
                            break;
                        }
                    }
                    break;
                case PickUps.POWER:

                    Debug.Log("Is power available");

                    PowerUp[] power = GameObject.FindObjectsOfType<PowerUp>();
                    for (int i = 0; i < power.Length; i++)
                    {
                        if (!power[i].transform.parent)
                        {
                            result = true;
                            break;
                        }
                    }
                    break;
            }
            if (result)
            {
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }

    // Check if team of type has flag
    public class TeamHasFlag : Node
    {
        TeamBlackboard teamBlackboard;
        Team type;

        public TeamHasFlag(TeamBlackboard teamBlackboard, Team type)
        {
            this.teamBlackboard = teamBlackboard;
            this.type = type;
        }
        public override NodeState Evaluate()
        {
            bool hasFlag = false;

            switch (type)
            {
                case Team.ENEMY:
                    Debug.Log("Does the " + teamBlackboard.name + " team have the enemy flag?");
                    if (teamBlackboard.GetMemberWithEnemyFlag()) hasFlag = true;
                    break;
                case Team.FRIENDLY:
                    Debug.Log("Does the " + teamBlackboard.name + " team have the friendly flag?");

                    if (teamBlackboard.GetMemberWithFriendlyFlag()) hasFlag = true;
                    break;
            }

            if (hasFlag)
            {
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }

    // Check if member has an item
    public class HasItem : Node
    {
        private AgentData agentData;
        private InventoryController inventoryController;
        Items type;

        public HasItem(AgentData agentData, InventoryController inventoryController, Items type)
        {
            this.agentData = agentData;
            this.inventoryController = inventoryController;
            this.type = type;
        }
        public override NodeState Evaluate()
        {
            bool result = false;
            switch (type)
            {
                case Items.ENEMY_FLAG:
                    if (inventoryController.HasItem(agentData.EnemyFlagName))
                    {
                        result = true;
                    }
                    break;
                case Items.FRIENDLY_FLAG:

                    if (inventoryController.HasItem(agentData.FriendlyFlagName))
                    {
                        result = true;
                    }
                    break;
                case Items.HEALTH:

                    if (inventoryController.HasItem("Health Kit"))
                    {
                        result = true;
                    }
                    break;
                case Items.POWER:
                    if (inventoryController.HasItem("Power Up"))
                    {
                        result = true;
                    }
                    break;
            }
            if (result)
            {
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }

    // Is PickUp in range
    public class isPickUpInRange : Node
    {
        private AgentData agentData;
        private Sensing sensing;
        Items type;

        public isPickUpInRange(AgentData agentData, Sensing sensing, Items type)
        {
            this.agentData = agentData;
            this.sensing = sensing;
            this.type = type;
        }
        public override NodeState Evaluate()
        {
            bool result = false;
            List<GameObject> objectsInView = sensing.GetObjectsInView();
            for (int i = 0; i < objectsInView.Count; i++)
            {
                // If the item is in reach
                if (sensing.IsItemInReach(objectsInView[i]))
                {
                    
                    switch (type)
                    {
                        case Items.ENEMY_FLAG:
                            if (objectsInView[i].name.Equals(agentData.EnemyFlagName))
                            {
                                result = true;
                            }
                            break;
                        case Items.FRIENDLY_FLAG:

                            if (objectsInView[i].name.Equals(agentData.FriendlyFlagName))
                            {
                                result = true;
                            }
                            break;
                        case Items.HEALTH:
                            if (objectsInView[i].name.Equals("Health Kit"))
                            {
                                result = true;
                            }
                            break;
                        case Items.POWER:
                            if (objectsInView[i].name.Equals("Power Up"))
                            {
                                result = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            if (result)
            {

                return NodeState.SUCCESS;
            }
            else
            {

                return NodeState.FAILURE;
            }
        }
    }

    // Check if there is an enemy in attack range
    public class EnemyInAttackRange : Node
    {
        private Sensing sensing;

        public EnemyInAttackRange(Sensing sensing)
        {
            this.sensing = sensing;
        }
        public override NodeState Evaluate()
        {
            if (sensing.IsInAttackRange(sensing.GetNearestEnemyInView()))
            {
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }

    // Check if a friendly team member is chasing flag
    public class TeamMemberChasingFlag : Node
    {
        private AgentData thisAgent;

        public TeamMemberChasingFlag(AgentData thisAgent)
        {
            this.thisAgent = thisAgent;
        }
        public override NodeState Evaluate()
        {
            List<GameObject> ChasingFlag = thisAgent.GetTeamBlackboard().GetMembersChasingFlag();
            if ((ChasingFlag.Count == 1 && ChasingFlag[0] != thisAgent.gameObject) || ChasingFlag.Count > 1)
            {
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }

    public class FlagAtBase : Node
    {
        private TeamBlackboard teamBlackboard;

        public FlagAtBase(TeamBlackboard teamBlackboard)
        {
            this.teamBlackboard = teamBlackboard;
        }
        public override NodeState Evaluate()
        {
            if (teamBlackboard.GetFriendlyBase().IsEnemyFlagInBase())
            {
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }

    public class HealthNextToWeakest : Node
    {
        private TeamBlackboard teamBlackboard;

        public HealthNextToWeakest(TeamBlackboard teamBlackboard)
        {
            this.teamBlackboard = teamBlackboard;
        }
        public override NodeState Evaluate()
        {
            bool health_next_to_weakest = false;
            if (teamBlackboard.GetWeakestMember())
            {
                // Check if there is a health pickup within reach of the weakest member
                Sensing weakest_senses = teamBlackboard.GetWeakestMember().GetComponentInChildren<Sensing>();
                List<GameObject> collectables_in_view = weakest_senses.GetCollectablesInView();
                for (int i = 0; i < collectables_in_view.Count; i++)
                {
                    if (weakest_senses.IsItemInReach(collectables_in_view[i]) && collectables_in_view[i].name.Equals("Health Kit"))
                    {
                        health_next_to_weakest = true;
                    }
                }
            }

            if (health_next_to_weakest)
            {
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }
}




/// <summary>
/// A decorator node that inverts the result of its child node.
/// </summary>
public class Decorators
{
   
    public class Inverter : Node
    {
        private Node node;

        public Inverter(Node node)
        {
            this.node = node;
        }


        public override NodeState Evaluate()
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    nodeState = NodeState.RUNNING;
                    break;
                case NodeState.SUCCESS:
                    nodeState = NodeState.FAILURE;
                    break;
                case NodeState.FAILURE:
                    nodeState = NodeState.SUCCESS;
                    break;
                default:
                    break;
            }
            return nodeState;
        }
    }
}
