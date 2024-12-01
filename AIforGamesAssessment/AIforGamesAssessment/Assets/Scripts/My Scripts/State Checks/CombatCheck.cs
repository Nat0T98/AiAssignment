/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///================================================================================
/// <summary>
/// Let agent know if there are any enemies in view. This node is used to get into
/// the "fight state".
/// -------------------------------------------------------------------------------
/// Flow: AreEnemiesInView? ==> TryToAttck
/// </summary>
///================================================================================

public class AreEnemiesInViewCondition : Node
{
    private Sensing senses;

    public AreEnemiesInViewCondition(Sensing _senses)
    {
        senses = _senses;
    }


    public override NodeState Evaluate()
    {
        return senses.GetEnemiesInView().Count > 0 ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}


///================================================================================
/// <summary>
/// Checks if the agent has enough health to eventually attack.
/// -------------------------------------------------------------------------------
/// Flow: HasEnoughHealth? ==> GoToItem ==> TakeItem ==> Attack
/// </summary>
///================================================================================

public class HasEnoughHealthCondition : Node
{
    private AgentData data;

    public HasEnoughHealthCondition(AgentData _data)
    {
        data = _data;
    }


    public override NodeState Evaluate()
    {
        return (data.CurrentHitPoints > AgentData.MIN_HEALTH_TO_ATTACK) ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}


///================================================================================
/// <summary>
/// Check if the agent is engaging a fight. This node let the agent focus on the 
/// fight and avoids collecting the flag.
/// -------------------------------------------------------------------------------
/// Flow: isEngagingAFight? ==> AvoidFlag ==> Fight 
/// </summary>
///================================================================================

public class IsEngagingAFightCondition : Node
{
    private AgentData data;

    public IsEngagingAFightCondition(AgentData _data)
    {
        data = _data;
    }


    public override NodeState Evaluate()
    {
        return data.helperData.engagingFight ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
*/