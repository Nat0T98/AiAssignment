using UnityEngine;

public class GlobalBlackboard : MonoBehaviour
{
    [SerializeField] private TeamBlackboard blueTeamBlackboard;
    [SerializeField] private TeamBlackboard redTeamBlackboard;

    public TeamBlackboard GetBlueTeamBlackboard() 
    { 
        return blueTeamBlackboard; 
    }
    public TeamBlackboard GetRedTeamBlackboard() 
    {
        return redTeamBlackboard;
    }

}
