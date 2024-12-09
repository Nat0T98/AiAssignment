using System.Collections.Generic;
using UnityEngine;


public enum Team
{
    ENEMY,
    FRIENDLY,
}

public enum Objects
{
    ENEMY_FLAG,
    FRIENDLY_FLAG,

    HEALTH_PICKUP,
    POWER_PICKUP,

    NEAREST_ENEMY,
    FRIENDLY_WITH_FLAG,
    WEAKEST_FRIENDLY,
    THIS_AGENT,

    BASE,
    NOT_IN_BASE,
}

public enum Items
{
    ENEMY_FLAG,
    FRIENDLY_FLAG,
    HEALTH,
    POWER,
}

public enum PickUps
{
    HEALTH,
    POWER,
}

public class Checks
{ 
    //Check agent health
    public class AgentHeathLessThan : Node
    {
        private AgentData agentData;
        private TeamBlackboard teamBlackboard;
        Objects type;
        int value;

        public AgentHeathLessThan(AgentData agentData, TeamBlackboard teamBlackboard, Objects type, int value)
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
                case Objects.WEAKEST_FRIENDLY:

                    if (teamBlackboard.GetWeakestMember() && teamBlackboard.GetWeakestMember() != agentData.gameObject)
                    {
                        result = teamBlackboard.GetWeakestMember().GetComponent<AgentData>().CurrentHitPoints < value;
                    }
                    break;
                case Objects.THIS_AGENT:
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

    //Check if there is a Pick Up of type available
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

    //Check if team of type has flag
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
                    /*Debug.Log("Does the " + teamBlackboard.name + " team have the enemy flag?");*/
                    if (teamBlackboard.GetMemberWithEnemyFlag()) hasFlag = true;
                    break;
                case Team.FRIENDLY:
                    /*Debug.Log("Does the " + teamBlackboard.name + " team have the friendly flag?");*/

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

    //Check if member has an item
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

    //Is PickUp in range
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

    //Check if there is an enemy in attack range
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

    //Check if a friendly team member is chasing flag
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

    //Check if there is a health pickup within reach of the weakest member
    public class HealthNearWeakest : Node
    {
        private TeamBlackboard teamBlackboard;

        public HealthNearWeakest(TeamBlackboard teamBlackboard)
        {
            this.teamBlackboard = teamBlackboard;
        }
        public override NodeState Evaluate()
        {
            bool health_near_weakest = false;
            if (teamBlackboard.GetWeakestMember())
            {
                Sensing weakest_senses = teamBlackboard.GetWeakestMember().GetComponentInChildren<Sensing>();
                List<GameObject> collectables_in_view = weakest_senses.GetCollectablesInView();
                for (int i = 0; i < collectables_in_view.Count; i++)
                {
                    if (weakest_senses.IsItemInReach(collectables_in_view[i]) && collectables_in_view[i].name.Equals("Health Kit"))
                    {
                        health_near_weakest = true;
                    }
                }
            }

            if (health_near_weakest)
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


public class Actions
{       
    //Move towards closest GameObject of type
    public class MoveToObj : Node
    {
        private AgentActions agentActions;
        private AgentData agentData;
        private Sensing sensing;
        private Objects type;

        public MoveToObj(AgentActions agentActions, AgentData agentData, Sensing sensing, Objects type)
        {
            this.agentActions = agentActions;
            this.agentData = agentData;
            this.sensing = sensing;
            this.type = type;
        }
        public override NodeState Evaluate()
        {
            // Get the GameObject according to type
            GameObject GO = null;
            bool isInReach = false;
            bool notInReach = false;
            switch (type)
            {
                case Objects.ENEMY_FLAG:
                    GO = agentData.GetTeamBlackboard().GetEnemyFlag();
                    isInReach = true;
                    break;
                case Objects.FRIENDLY_FLAG:
                    GO = agentData.GetTeamBlackboard().GetFriendlyFlag();
                    notInReach = true;
                    break;
                case Objects.HEALTH_PICKUP:
                    GO = GetItemTarget("Health Kit");
                    isInReach = true;
                    break;
                case Objects.POWER_PICKUP:
                    GO = GetItemTarget("Power Up");
                    isInReach = true;
                    break;
                case Objects.NEAREST_ENEMY:
                    GO = sensing.GetNearestEnemyInView();
                    notInReach = true;
                    break;
                case Objects.FRIENDLY_WITH_FLAG:
                    GO = agentData.GetTeamBlackboard().GetMemberWithEnemyFlag();
                    notInReach = true;
                    break;
                case Objects.WEAKEST_FRIENDLY:
                    GO = agentData.GetTeamBlackboard().GetWeakestMember();
                    isInReach = true;
                    break;
                case Objects.BASE:
                    GO = agentData.FriendlyBase;
                    notInReach = true;
                    break;
                case Objects.NOT_IN_BASE:
                    isInReach = true;
                    break;
            }

            if (type == Objects.NOT_IN_BASE)
            {
                agentActions.MoveToRandomLocation();
                return NodeState.RUNNING;
            }


            if (isInReach && sensing.IsItemInReach(GO) ||
                notInReach && sensing.IsInAttackRange(GO))
            {
                return NodeState.SUCCESS;
            }
            agentActions.MoveTo(GO);
            return NodeState.RUNNING;
        }

        private GameObject GetItemTarget(string name)
        {
            List<GameObject> itemsInView = sensing.GetCollectablesInView();
            for (int i = 0; i < itemsInView.Count; i++)
            {
                if (itemsInView[i].name.Equals(name))
                {
                    return itemsInView[i];
                }
            }
            return null;
        }
    }

    //Attack nearest enemy
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
            GameObject nearestEnemy = sensing.GetNearestEnemyInView();
            agentActions.AttackEnemy(nearestEnemy);
            if (!sensing.IsInAttackRange(sensing.GetNearestEnemyInView()))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.RUNNING;
        }
    }
    //Pick up the nearest Item of type
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
            //Items in view range of agent
            List<GameObject> collectablesInView = sensing.GetObjectsInView();
            for (int i = 0; i < collectablesInView.Count; i++)
            {
                if (sensing.IsItemInReach(collectablesInView[i]) && type == Items.FRIENDLY_FLAG && collectablesInView[i].name.Equals(agentData.FriendlyFlagName)
                        || type == Items.ENEMY_FLAG && collectablesInView[i].name.Equals(agentData.EnemyFlagName)
                        || type == Items.POWER && collectablesInView[i].name.Equals("Power Up")
                        || type == Items.HEALTH && collectablesInView[i].name.Equals("Health Kit"))
                {

                    agentActions.CollectItem(collectablesInView[i]);
                    return NodeState.SUCCESS;
                }
            }
            return NodeState.FAILURE; //No item
        }
    }

    //Drop item of type
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
            // Get the item from member inventory
            GameObject item = null;
            switch (type)
            {
                case Items.FRIENDLY_FLAG:
                    item = inventoryController.GetItem(agentData.FriendlyFlagName);
                    break;
                case Items.ENEMY_FLAG:
                    item = inventoryController.GetItem(agentData.EnemyFlagName);
                    break;
                case Items.HEALTH:
                    item = inventoryController.GetItem("Health Kit");
                    break;
                case Items.POWER:
                    item = inventoryController.GetItem("Power Up");
                    break;
            }

            // Drop the item
            if (item)
            {
                agentActions.DropItem(item);
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }   

    //Flee
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
            GameObject nearestEnemy = sensing.GetNearestEnemyInView();
            if (!sensing.IsInAttackRange(nearestEnemy))
            {
                return NodeState.SUCCESS;
            }
            agentActions.Flee(nearestEnemy);
            return NodeState.RUNNING;
        }
    }

    //Use first type of PickUp in inventory
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
            GameObject item = null;
            switch (type)
            {
                case PickUps.HEALTH:
                    item = inventoryController.GetItem("Health Kit");
                    break;
                case PickUps.POWER:
                    item = inventoryController.GetItem("Power Up");
                    break;
            }
            // Use item
            agentActions.UseItem(item);
            return NodeState.SUCCESS;
        }
    }
}





