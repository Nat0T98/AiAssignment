/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///================================================================================
/// <summary>
/// Let the agent know if the assigned flag is in a certain base. This node 
/// determines wheter to go collect a flag or defend the base.
/// -------------------------------------------------------------------------------
/// Flow: IsFlagInBase? ==> GoToFlag ==> CollectFlag 
/// </summary>
///================================================================================

public class IsFlagAtBase : Node
{
    private Flag whichFlag;
    private string whichBase;

    public IsFlagAtBase(string _whichFlag, string _whichBase)
    {
        whichFlag = GameObject.Find(_whichFlag).GetComponent<Flag>();
        whichBase = _whichBase;
    }


    public override NodeState Evaluate()
    {
        return whichFlag.WhichBaseIsFlagIn == whichBase ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}


///================================================================================
/// <summary>
/// Let the agent know if the assigned flag is outside of any base. If this node
/// returns SUCCESS, the agent goes to that flag
/// -------------------------------------------------------------------------------
/// Flow: IsFlagOutsideAnyBase? ==> GoToFlag ==> CollectFlag 
/// </summary>
///================================================================================
public class IsFlagNotInAnyBase : Node
{
    private Flag whichFlag;

    public IsFlagNotInAnyBase(string _whichFlag)
    {
        whichFlag = GameObject.Find(_whichFlag).GetComponent<Flag>();
    }


    public override NodeState Evaluate()
    {
        return whichFlag.WhichBaseIsFlagIn == "" ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}

///================================================================================
/// <summary>
/// Let the agent know if both flags are in friendly base. Generally used for the 
/// defence state.
/// -------------------------------------------------------------------------------
/// Flow: AreBothFlagsInFriendlyBase? ==> GoToDefendFriendlyBase? ==> DefendBase
/// </summary>
///================================================================================

public class AreBothFlagsInFriendlyBaseCondition : Node
{
    private AgentData data;

    public AreBothFlagsInFriendlyBaseCondition(AgentData _data)
    {
        data = _data;
    }


    public override NodeState Evaluate()
    {
        //Get both flags and checks if they are in the friendly base
        var flags = GameObject.FindObjectsOfType<Flag>();
        foreach (var flag in flags)
        {
            if (flag.WhichBaseIsFlagIn != data.FriendlyBase.name)
                return NodeState.FAILURE;
        }

        //Both flags are in friendly base 
        return NodeState.SUCCESS;
    }
}


///================================================================================
/// <summary>
/// Check if the agent is holding a flag. This node determins if a flag can be 
/// brought back in the friendly base.
/// -------------------------------------------------------------------------------
/// Flow: HasTeamAnyFlag? ==> BringFlagBackToBase 
/// </summary>
///================================================================================

public class HasTeamAnyFlagCondition : Node
{
    private AgentData data;

    public HasTeamAnyFlagCondition(AgentData _data)
    {
        data = _data;
    }


    public override NodeState Evaluate()
    {
        //Does not care about enemies around (Let teammates fight)
        if (data.helperData.engagingFight)
            return NodeState.SUCCESS;

        //Find all teammates and check if they have flag
        var team = GameObject.FindGameObjectsWithTag(data.gameObject.tag);
        foreach (var member in team)
        {
            AgentData memberData = member.GetComponent<AgentData>();
            if (memberData.HasFriendlyFlag || memberData.HasEnemyFlag)
                return NodeState.SUCCESS; //Has a flag 
        }

        //Nobody is holding a flag 
        return NodeState.FAILURE;
    }
}


///================================================================================
/// <summary>
/// Let the agent know if a friendly is holding a flag. This node determines the 
/// defence state.
/// -------------------------------------------------------------------------------
/// Flow: IsFriendHoldingAFlag? ==> GoToFriend ==> DefendFriend 
/// </summary>
///================================================================================

public class IsFriendHoldingAFlagCondition : Node
{
    private AgentData data;

    public IsFriendHoldingAFlagCondition(AgentData _data)
    {
        data = _data;
    }


    public override NodeState Evaluate()
    {
        var members = GameObject.FindGameObjectsWithTag(data.gameObject.tag);
        foreach (var member in members)
        {
            if (member.GetComponent<AgentData>().HasEnemyFlag || member.GetComponent<AgentData>().HasFriendlyFlag)
                return NodeState.SUCCESS;
        }

        //Return FAILURE if nobody has a flag
        return NodeState.FAILURE;
    }
}



///================================================================================
/// <summary>
/// Check if agent is holding any flag. Determines whether to bring the flag back
/// to base or not.
/// -------------------------------------------------------------------------------
/// Flow: IsHoldingAnyFlag? ==> BringFlagBackToBase
/// </summary>
///================================================================================

public class IsHoldingAnyFlagCondition : Node
{
    private AgentData data;

    public IsHoldingAnyFlagCondition(AgentData _data)
    {
        data = _data;
    }


    public override NodeState Evaluate()
    {
        return (data.HasEnemyFlag || data.HasFriendlyFlag) ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
*/