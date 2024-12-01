/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///================================================================================
/// <summary>
/// Let the agent go to the assigned flag. This node fails if any enemies are in 
/// in front of the agent. 
/// -------------------------------------------------------------------------------
/// Flow: CanTakeFlag? ==> GoToFlag ==> TakeFlag   
/// </summary>
///================================================================================

public class GoToFlagAction : Node
{
    private AgentData data;
    private AgentActions action;
    private Sensing senses;
    private GameObject flag;

    public GoToFlagAction(AgentData _data, AgentActions _action, Sensing _senses, GameObject _flag)
    {
        data = _data;
        action = _action;
        senses = _senses;
        flag = _flag;
    }

    public override NodeState Evaluate()
    {
        //Debug
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToFlagAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToFlagAction";


        //Check if we are really close to the flag. If so, ignore enemies 
        if (Vector3.Distance(action.gameObject.transform.position, flag.transform.position) > AgentData.minDistanceToIgnoreEnemies)
        {
            //Fails if an enemy is in the view range and in front of agent 
            var enemies = senses.GetEnemiesInView();
            foreach (var enemy in enemies)
            {
                bool isWallInBetween = false;
                //Check if there are any wall between the agent and the enemy 
                action.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                RaycastHit hit;
                Vector3 rayDirection = enemy.transform.position - action.gameObject.transform.position;
                if (Physics.Raycast(action.gameObject.transform.position, rayDirection, out hit))
                {
                    if (hit.collider.tag != data.EnemyTeamTag)
                    {
                        isWallInBetween = true;
                        action.gameObject.GetComponent<CapsuleCollider>().enabled = true;
                    }
                }
                action.gameObject.GetComponent<CapsuleCollider>().enabled = true;

                //Check if enemy is in front of agent and close and not behind wall, if yes fails 
                if (Vector3.Dot(action.Velocity, (enemy.transform.position - action.gameObject.transform.position)) > 0.0f &&
                    Vector3.Distance(action.gameObject.transform.position, enemy.transform.position) > AgentData.alertRange && !isWallInBetween)
                    return NodeState.FAILURE;
            }
        }


        //Move and check if destination is reached 
        if (action.MoveTo(flag.transform.position))
        {
            //Success if destination is reached 
            if (Vector3.Distance(flag.transform.position, action.gameObject.transform.position) < AgentData.PickUpRange + 0.5f)
            {
                data.helperData.currentFlagToTake = flag;
                return NodeState.SUCCESS;
            }
            else //Running if he still has to reach the destination  
                return NodeState.RUNNING;
        }

        //Should never fail
        return NodeState.FAILURE;
    }
}


///================================================================================
/// <summary>
/// Let the agent take the just approached flag. 
/// -------------------------------------------------------------------------------
/// Flow: CanGetFlag? ==> GoToFlag ==> TakeFlag
/// </summary>
///================================================================================

public class TakeFlagAction : Node
{
    private AgentActions action;

    public TakeFlagAction(AgentActions _action)
    {
        action = _action;
    }


    public override NodeState Evaluate()
    {
        //Debug
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "TakeFlagAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "TakeFlagAction";


        AgentData data = action.gameObject.GetComponent<AgentData>();

        //If I don't already have a flag, collect one
        if (!data.HasFriendlyFlag && !data.HasEnemyFlag)
            action.CollectItem(data.helperData.currentFlagToTake);

        //Cannot Fail, as the player is within range at this point   
        return (data.HasFriendlyFlag || data.HasEnemyFlag) ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}



///================================================================================
/// <summary>
/// Let the agent drop the flag once the base has been reached 
/// -------------------------------------------------------------------------------
/// Flow: CanGetFlag? ==> GoToFlag ==> TakeFlag ==> BringFlagToBase  
/// </summary>
///================================================================================

public class DropFlagAction : Node
{
    private AgentData data;
    private AgentActions action;

    public DropFlagAction(AgentData _data, AgentActions _action)
    {
        data = _data;
        action = _action;
    }


    public override NodeState Evaluate()
    {
        //Debug
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "DropFlagAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "DropFlagAction";


        //Drop the flag 
        if (data.helperData.currentFlagToTake != null)
        {
            action.DropItem(data.helperData.currentFlagToTake);
            data.helperData.currentFlagToTake = null;

            return NodeState.SUCCESS; //Exit node
        }

        //Cannot Fail, as the player is within range at this point   
        return NodeState.FAILURE;
    }
}


///================================================================================
/// <summary>
/// Let the agent go to the friendly who is holding the flag. This node is always
/// RUNNING, unless an enemy is approaching or friendly has lost the flag.  
/// -------------------------------------------------------------------------------
/// Flow: IsFriendlyHolding? ==> GoToFriendlyBase ==> DropFlag   
/// </summary>
///================================================================================

public class GoToFriendWithFlagAction : Node
{
    private AgentData data;
    private AgentActions action;
    private Sensing senses;

    public GoToFriendWithFlagAction(AgentData _data, AgentActions _action, Sensing _senses)
    {
        data = _data;
        action = _action;
        senses = _senses;
    }


    public override NodeState Evaluate()
    {
        //Debug
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToFriendWithFlagAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToFriendWithFlagAction";


        //get enemies in view 
        var enemies = senses.GetEnemiesInView();
        foreach (var enemy in enemies)
        {
            bool isWallInBetween = false;
            //Check if there are any wall between the agent and the enemy 
            action.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            RaycastHit hit;
            Vector3 rayDirection = enemy.transform.position - action.gameObject.transform.position;
            if (Physics.Raycast(action.gameObject.transform.position, rayDirection, out hit))
            {
                if (hit.collider.tag != data.EnemyTeamTag)
                {
                    isWallInBetween = true;
                    action.gameObject.GetComponent<CapsuleCollider>().enabled = true;
                }
            }
            action.gameObject.GetComponent<CapsuleCollider>().enabled = true;

            //Check if enemy is in front of agent and close and not behind wall, if yes fails 
            if (Vector3.Dot(action.Velocity, (enemy.transform.position - action.gameObject.transform.position)) > 0.0f &&
                Vector3.Distance(action.gameObject.transform.position, enemy.transform.position) > AgentData.alertRange && !isWallInBetween)
                return NodeState.FAILURE;
        }


        //Get friend with a flag
        List<GameObject> friendlies = senses.GetFriendliesInView();
        GameObject friendWithFlag = null;
        foreach (var friend in friendlies)
        {
            if (friend.GetComponent<AgentData>().HasEnemyFlag || friend.GetComponent<AgentData>().HasEnemyFlag)
                friendWithFlag = friend;
        }
        //If friendly has a flag, move towards him and check if destination is reached 
        if (friendWithFlag != null && action.MoveTo(friendWithFlag.transform.position))
        {
            //Success if destination is reached 
            if (Vector3.Distance(friendWithFlag.transform.position, action.gameObject.transform.position) < AgentData.AttackRange)
            {
                return NodeState.RUNNING;
            }
        }


        //Fails if friend has lost the flag or enemy is near/in front
        return NodeState.FAILURE;
    }
}

///================================================================================
/// <summary>
/// Let the agent go to the friendly base in order to drop the held flag. This node
/// does not take any enemies around into consideration.
/// -------------------------------------------------------------------------------
/// Flow: IsHoldingAFlag? ==> GoToFriendlyBase ==> DropFlag   
/// </summary>
///================================================================================

public class GoToFriendlyBaseBringFlagAction : Node
{
    private AgentData data;
    private AgentActions action;

    public GoToFriendlyBaseBringFlagAction(AgentData _data, AgentActions _action)
    {
        data = _data;
        action = _action;
    }


    public override NodeState Evaluate()
    {
        //Debug 
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToFriendlyBaseBringFlagAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToFriendlyBaseBringFlagAction";


        //Move and check if destination is reached 
        if (action.MoveTo(data.FriendlyBase))
        {
            //Success if destination is reached 
            if (Vector3.Distance(action.Destination, action.gameObject.transform.position) <= 1.0f)
                return NodeState.SUCCESS;
        }

        //Running until destination is reached
        return NodeState.RUNNING;
    }
}


*/