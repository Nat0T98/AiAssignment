/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///================================================================================
/// <summary>
/// Let the agent go to the the closest collectable (not flags) 
/// -------------------------------------------------------------------------------
/// Flow: FindCollectable ==> CanPickItUp? ==> GoToCollectable ==> Take  
/// </summary>
///================================================================================

public class GoToClosestCollectableAction : Node
{
    private AgentData data;
    private AgentActions action;

    public GoToClosestCollectableAction(AgentData _data, AgentActions _action)
    {
        data = _data;
        action = _action;
    }

    public override NodeState Evaluate()
    {
        //Debug
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToClosestCollectableAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToClosestCollectableAction";


        //Move and check if destination is reached 
        if (data.helperData.pickUpToTake.gameObject.layer != 0 && action.MoveTo(data.helperData.pickUpToTake))
        {
            //Success if destination is reached 
            if (Vector3.Distance(data.helperData.pickUpToTake.transform.position, action.gameObject.transform.position) <= AgentData.PickUpRange)
                return NodeState.SUCCESS;
            else //Running if he still has to reach the destination   
                return NodeState.RUNNING;
        }

        //Fails if destinations is not yet reached or if item is already taken 
        return NodeState.FAILURE;
    }
}


///================================================================================
/// <summary>
/// Let the agent take the pick up in front of it. This action is performed after
/// the "Flee" node (whenever it fails).
/// -------------------------------------------------------------------------------
/// Flow: IsNotFleeing? ==> CanTakePickUp ==> GoToPickUp ==> TakePickUp
/// </summary>
///================================================================================

public class TakePickUpAction : Node
{
    private AgentData data;
    private AgentActions action;
    private InventoryController inventory;

    public TakePickUpAction(AgentData _data, AgentActions _action, InventoryController _inv)
    {
        data = _data;
        action = _action;
        inventory = _inv;
    }


    public override NodeState Evaluate()
    {
        //Debug
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "TakePickUpAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "TakePickUpAction";


        //Take item
        if (data.helperData.pickUpToTake != null)
        {
            action.CollectItem(data.helperData.pickUpToTake);
        }
        //Check if item has been taken
        if (inventory.HasItem(data.helperData.pickUpToTake.name))
        {
            data.helperData.pickUpToTake = null;

            return NodeState.SUCCESS;
        }

        //Fails if attempt to take item fails    
        return NodeState.FAILURE;
    }
}



///================================================================================
/// <summary>
/// Let the agent use a pick up before fighting an enemy if chances of winning are
/// low. 
/// -------------------------------------------------------------------------------
/// Flow: CanWin? ==> HasPickUp? ==> UsePickUp ==> Fight
/// </summary>
///================================================================================

public class UsePickUpAction : Node
{
    private AgentData data;
    private AgentActions action;

    public UsePickUpAction(AgentData _data, AgentActions _action)
    {
        data = _data;
        action = _action;
    }


    public override NodeState Evaluate()
    {
        //Debug 
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "UsePickUpAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "UsePickUpAction";


        //Use item 
        if (data.helperData.currentPickUpToUse != null)
        {
            action.UseItem(data.helperData.currentPickUpToUse);
            data.helperData.currentPickUpToUse = null;

            data.helperData.itemUsed = true;
            action.StartCoroutine(HasItemBeingUsed());

            return NodeState.SUCCESS;
        }

        //Fails if item cannot be used for any reason (SHOULD NEVER FAIL!)
        return NodeState.FAILURE;
    }

    //Ensures the agent does not flee anymore after using an item
    IEnumerator HasItemBeingUsed()
    {
        yield return new WaitForSeconds(AgentData.itemUsedInterval);
        data.helperData.itemUsed = false;
    }
}
*/