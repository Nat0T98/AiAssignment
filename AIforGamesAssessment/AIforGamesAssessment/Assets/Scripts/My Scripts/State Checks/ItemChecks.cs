/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///================================================================================
/// <summary>
/// Checks the agent's inventory and determines whether an item can be picked up
/// -------------------------------------------------------------------------------
/// Flow: CanTakeItem? ==> GoToItem ==> TakeItem
/// </summary>
///================================================================================

public class CanTakeItemCondition : Node
{
    private InventoryController inventory;
    private AgentData data;

    public CanTakeItemCondition(AgentData _data, InventoryController _inv)
    {
        data = _data;
        inventory = _inv;
    }


    public override NodeState Evaluate()
    {
        //Check the player can pick up the item 
        if (data.helperData.pickUpToTake != null && !inventory.HasItem(data.helperData.pickUpToTake.name))
            return NodeState.SUCCESS;

        //disables fighting state and fails if the item can't be picked up
        data.helperData.engagingFight = false;
        return NodeState.FAILURE;
    }
}


///================================================================================
/// <summary>
/// Check if a pick up item has been recently used, so that the agent does not go
/// in flee mode, instead it attacks.
/// -------------------------------------------------------------------------------
/// Flow: HasPickUpItemBeenUsed? ==> GoToCLosestEnemy ==> Attack
/// </summary>
///================================================================================

public class HasPickUpItemBeenUsedCondition : Node
{
    private AgentData data;

    public HasPickUpItemBeenUsedCondition(AgentData _data)
    {
        data = _data;
    }


    public override NodeState Evaluate()
    {
        return data.helperData.itemUsed ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
*/