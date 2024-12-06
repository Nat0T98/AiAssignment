using System.Collections.Generic;
using UnityEngine;

public class TeamBlackboard : MonoBehaviour
{
    [Header("Friendly Team Refs")]
    [SerializeField] private SetScore friendlyBase;
    [SerializeField] private GameObject friendlyFlag;
    [Space(20)]
    [Header("Enemy Team Refs")]
    [SerializeField] private SetScore enemyBase;
    [SerializeField] private GameObject enemyFlag;
    

    //Getters
    public SetScore GetFriendlyBase() 
    {
        return friendlyBase; 
    }

    public GameObject GetFriendlyFlag() 
    { 
        return friendlyFlag;
    }

    public SetScore GetEnemyBase() 
    { 
        return enemyBase; 
    }

    public GameObject GetEnemyFlag() 
    {
        return enemyFlag;
    }

    private List<AgentData> team = new List<AgentData>();
    public void AddTeamMember(AgentData friendly) { team.Add(friendly); }
    public void RemoveTeamMember(AgentData friendly) 
    {
        GameObject memberGO = friendly.gameObject;
        if (GetMemberWithEnemyFlag() == memberGO)
        {
            SetMemberWithEnemyFlag(null);
        }
        if (GetMemberWithFriendlyFlag() == memberGO)
        {
            SetMemberWithFriendlyFlag(null);
        }
        if (GetMembersChasingFlag().Contains(memberGO))
        {
            RemoveMemberChasingFlag(memberGO);
        }
        if (GetWeakestMember() == memberGO)
        {
            SetWeakestMember(null);
        }
        team.Remove(friendly); 
    }

    
    //Weakest member
    private GameObject weakestMember;
    public GameObject GetWeakestMember() 
    { 
        return weakestMember; 
    }
    public void SetWeakestMember(GameObject weakestMember) 
    {
        this.weakestMember = weakestMember; 
    }

    //Members holding flags
    private GameObject memberWithEnemyFlag;
    public GameObject GetMemberWithEnemyFlag() 
    { 
        return memberWithEnemyFlag;
    }

    public void SetMemberWithEnemyFlag(GameObject member) 
    {
        memberWithEnemyFlag = member;
    }

    private GameObject memberWithFriendlyFlag;
    public GameObject GetMemberWithFriendlyFlag() 
    { 
        return memberWithFriendlyFlag;
    }
    public void SetMemberWithFriendlyFlag(GameObject member) 
    { 
        memberWithFriendlyFlag = member;
    }


    //Members chasing flags
    private List<GameObject> membersChasingFlag = new List<GameObject>();
    public List<GameObject> GetMembersChasingFlag() 
    {
        return membersChasingFlag;
    }
    public void AddMemberChasingFlag(GameObject member) 
    {
        if (!membersChasingFlag.Contains(member)) membersChasingFlag.Add(member);
    }
    public void RemoveMemberChasingFlag(GameObject member) 
    {
        membersChasingFlag.Remove(member); 
    }

    private void Update()
    {
        FindWeakest();
    }

   //Search through team to find member with least health
    private void FindWeakest()
    {
        int health = 100;
        AgentData weakest = null;
        for (int i = 0; i < team.Count; i++)
        {
            if (team[i].CurrentHitPoints < health)
            {
                health = team[i].CurrentHitPoints;
                weakest = team[i];
            }
        }
        if (weakest)
        {
            weakestMember = weakest.gameObject;
        }
    }
}
