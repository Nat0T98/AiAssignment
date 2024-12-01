/*using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///================================================================================
/// <summary>
/// Let the agent go to the closest enemy in view. Exits the node if there are no
/// enemies in view
/// -------------------------------------------------------------------------------
/// Flow: IsThereAnEnemyInView? ==> GoToClosestEnemy ==> Attack  
/// </summary>
///================================================================================

public class GoToClosestEnemyInViewAction : Node
{
    private AgentData data;
    private AgentActions action;
    private Sensing senses;

    private GameObject closestEnemy;

    public GoToClosestEnemyInViewAction(AgentData _data, AgentActions _action, Sensing _senses)
    {
        data = _data;
        action = _action;
        senses = _senses;

        closestEnemy = null;
    }


    public override NodeState Evaluate()
    {
        //Debug
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToClosestEnemyInViewAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToClosestEnemyInViewAction";


        //Get closest enemy from the ones in view 
        if (closestEnemy == null)
        {
            List<GameObject> enemies = senses.GetEnemiesInView();

            //Get closest enemy once
            if (enemies.Count > 0)
                closestEnemy = enemies.OrderBy(m => Vector3.Distance(action.gameObject.transform.position, m.transform.position)).First();
        }
        if (closestEnemy == null) //Exit fight state if no enemies are around 
        {
            data.helperData.engagingFight = false;
        }


        //Move to closest enemy
        if (closestEnemy != null && action.MoveTo(closestEnemy))
        {
            //Success if destination is reached 
            if (Vector3.Distance(closestEnemy.transform.position, action.gameObject.transform.position) < AgentData.AttackRange)
            {
                data.helperData.closestEnemy = closestEnemy; //ensure next node knows which enemy to attack 
                closestEnemy = null;
                return NodeState.SUCCESS;
            }
            else //Running if he still has to reach the destination  
            {
                data.helperData.engagingFight = true;
                return NodeState.RUNNING;
            }
        }


        //Fails if no enemies are around
        closestEnemy = null;
        return NodeState.FAILURE;
    }
}



///=============================================================================
/// <summary>
/// Performs an Attack after the enemy position is reached. 
/// ----------------------------------------------------------------------------
/// Flow: Can Win? ==> GoToClosestEnemy ==> Attack
/// </summary>
///============================================================================= 

public class AttackAction : Node
{
    private AgentData data;
    private AgentActions action;

    public AttackAction(AgentData _data, AgentActions _action)
    {
        data = _data;
        action = _action;
    }


    public override NodeState Evaluate()
    {
        //Debug
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "AttackAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "AttackAction";


        //Attack
        action.AttackEnemy(data.helperData.closestEnemy);
        if (data.helperData.closestEnemy == null) //if Enemy is dead
        {
            data.helperData.engagingFight = false;  //Let's the other nodes know the fight is over
            return NodeState.SUCCESS;
        }


        return NodeState.RUNNING; //Continue attacking  
    }
}




///================================================================================
/// <summary>
/// Let the agent drop the flag once the base has been reached 
/// -------------------------------------------------------------------------------
/// Flow: FindClosestEnemy ==> Flee 
/// </summary>
///================================================================================

public class FleeAction : Node
{
    private AgentData data;
    private AgentActions action;
    private Sensing senses;

    public FleeAction(AgentData _data, AgentActions _action, Sensing _senses)
    {
        data = _data;
        action = _action;
        senses = _senses;
    }


    public override NodeState Evaluate()
    {
        //Debug 
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "FleeAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "FleeAction";


        //Find closest enemy and flee from it 
        List<GameObject> enemies = senses.GetEnemiesInView();
        GameObject enemyToFleeFrom;
        if (enemies.Count > 0)
        {
            enemyToFleeFrom = enemies.OrderBy(m => Vector3.Distance(action.gameObject.transform.position, m.transform.position)).First();
            action.Flee(enemyToFleeFrom);
        }
        else //No enemies are around 
        {
            data.helperData.engagingFight = false; //Disable fight state
            return NodeState.FAILURE;
        }

        //Check if enemy is too distant, if yes fails (disable fight state)
        if (Vector3.Distance(enemyToFleeFrom.transform.position, action.gameObject.transform.position) > AgentData.alertRange)
        {
            data.helperData.engagingFight = false;
            return NodeState.FAILURE;
        }

        //Check if an item is in front of player, if yes exit flee state  
        List<GameObject> items = senses.GetObjectsInViewByTag(Tags.Collectable);
        if (items.Count > 0)
        {
            GameObject closeItem = items.OrderBy(m => Vector3.Distance(action.gameObject.transform.position, m.transform.position)).First();
            if (Vector3.Dot(action.Velocity, closeItem.transform.position - action.gameObject.transform.position) > 0.0f)
            {
                //Check if there are any wall between the player and the item 
                action.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                RaycastHit hit;
                Vector3 rayDirection = closeItem.transform.position - action.gameObject.transform.position;
                if (Physics.Raycast(action.gameObject.transform.position, rayDirection, out hit))
                {
                    if (hit.collider.tag == Tags.Collectable)
                    {
                        //Set item to take 
                        data.helperData.pickUpToTake = closeItem;

                        action.gameObject.GetComponent<CapsuleCollider>().enabled = true;
                        return NodeState.FAILURE;
                    }
                }
                action.gameObject.GetComponent<CapsuleCollider>().enabled = true;
            }
        }


        //Continue fleeing 
        return NodeState.RUNNING;
    }
}



///================================================================================
/// <summary>
/// Let the agent go to a defense position. This position equals to the friendly 
/// base in front of the flags
/// -------------------------------------------------------------------------------
/// Flow: AreBothFlagsInFriendlyBase? ==> GoToDefendFriendlyBase ==> Defend   
/// </summary>
///================================================================================

public class GoToDefendFriendlyBaseAction : Node
{
    private AgentData data;
    private AgentActions action;
    Vector3 defensePos;

    public GoToDefendFriendlyBaseAction(AgentData _data, AgentActions _action)
    {
        data = _data;
        action = _action;

        defensePos = Vector3.zero;
    }


    public override NodeState Evaluate()
    {
        //Debug
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToDefendFriendlyBaseAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "GoToDefendFriendlyBaseAction";


        //Fails if the flags are not in the base anymore 
        var flags = GameObject.FindObjectsOfType<Flag>();
        foreach (var flag in flags)
        {
            if (flag.WhichBaseIsFlagIn != data.FriendlyBase.name)
            {
                return NodeState.FAILURE;
            }
        }

        if (data.helperData.engagingFight) //Fails if agent is in a fight
            return NodeState.FAILURE;


        //Calculate defense position
        if (defensePos == Vector3.zero)
        {
            defensePos = (Vector3.zero - flags[0].transform.position);
            Quaternion rot = Quaternion.identity; rot.eulerAngles = new Vector3(0.0f, Random.Range(-45.0f, 45.0f), 0.0f);
            var dir = rot * defensePos;
            defensePos = flags[0].transform.position + (dir.normalized * 4);
            Debug.Log(defensePos);
        }
        //Move and check if destination is reached 
        if (action.MoveTo(defensePos))
        {
            //Success if destination is reached 
            if (Vector3.Distance(defensePos, action.gameObject.transform.position) <= 1.0f)
                return NodeState.SUCCESS;
        }


        //Running until destination is reached
        return NodeState.RUNNING;
    }
}



///=============================================================================
/// <summary>
/// Makes the agent defend the base when both flags are in the friendly base. 
/// ----------------------------------------------------------------------------
/// Flow: AreBothFlagsInFriendlyBase? ==> GoToDefendPosition ==> DefendBase
/// </summary>
///============================================================================= 

public class DefendFriendlyBaseAction : Node
{
    private AgentData data;
    private AgentActions action;
    private Sensing senses;

    public DefendFriendlyBaseAction(AgentData _data, AgentActions _action, Sensing _senses)
    {
        data = _data;
        action = _action;
        senses = _senses;
    }


    public override NodeState Evaluate()
    {
        //Debug
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "DefendFriendlyBaseAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "DefendFriendlyBaseAction";


        //Fails if the flags are not in the base anymore 
        var flags = GameObject.FindObjectsOfType<Flag>();
        foreach (var flag in flags)
        {
            if (flag.WhichBaseIsFlagIn != data.FriendlyBase.name)
                return NodeState.FAILURE;
        }


        //Check if there are enemies around
        var enemies = senses.GetEnemiesInView();
        foreach (var enemy in enemies)
        {
            bool isWallInBetween = false;

            //Check if there are any walls between the agent and the enemy 
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

            //Check if enemy is close and not behind wall, if yes exit defence 
            if (Vector3.Distance(action.gameObject.transform.position, enemy.transform.position) < AgentData.alertRange && !isWallInBetween)
                return NodeState.SUCCESS; //Exit from the node to eventually attack
        }

        //Continue defending base
        return NodeState.RUNNING;
    }
}


///================================================================================
/// <summary>
/// Makes the agent defend a team member holding the flag by reaching his position.
/// If the flag is lost or an enemy is getting close, it returns SUCCESS and 
/// eventually attack
/// -------------------------------------------------------------------------------
/// Flow: IsFriendHoldingFlag? ==> GoToFriendWithFlag ==> DefendFriend 
/// </summary>
///================================================================================

public class DefendFriendlyWithFlagAction : Node
{
    private AgentData data;
    private AgentActions action;
    private Sensing senses;

    public DefendFriendlyWithFlagAction(AgentData _data, AgentActions _action, Sensing _senses)
    {
        data = _data;
        action = _action;
        senses = _senses;
    }


    public override NodeState Evaluate()
    {
        //Debug 
        if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.RedTeam)
            GameObject.Find("RedMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "DefendFriendlyWithFlagAction";
        else if (action.GetComponent<AgentData>().FriendlyTeam == AgentData.Teams.BlueTeam)
            GameObject.Find("BlueMemberTextDebug").GetComponent<UnityEngine.UI.Text>().text = "DefendFriendlyWithFlagAction";


        //Fails if no one has a flag anymore
        var friendlyWithFlag = senses.GetFriendliesInView();
        bool hasFlag = false;
        foreach (var friend in friendlyWithFlag)
        {
            if (friend.GetComponent<AgentData>().HasEnemyFlag || friend.GetComponent<AgentData>().HasFriendlyFlag)
            {
                hasFlag = true;
                break;
            }
        }
        if (!hasFlag) return NodeState.FAILURE;


        //Get all enemies
        var enemies = senses.GetEnemiesInView();
        foreach (var enemy in enemies)
        {
            bool isWallInBetween = false;
            //Check if there are any walls between the agent and the enemy 
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

            //Check if enemy is close and not behind wall, if yes fails 
            if (Vector3.Distance(action.gameObject.transform.position, enemy.transform.position) > AgentData.alertRange && !isWallInBetween)
                return NodeState.SUCCESS; //Exit from the node to eventually attack (does not matter if fails or succeeds)
        }

        //Continue holding defence position
        return NodeState.RUNNING;
    }
}*/