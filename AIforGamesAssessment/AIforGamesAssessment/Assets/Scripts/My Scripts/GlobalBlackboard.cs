using UnityEngine;

public class GlobalBlackboard : MonoBehaviour
{
    public TeamBlackboard blueTeamBlackboard;
    public TeamBlackboard redTeamBlackboard;

    public TeamBlackboard GetBlueTeamBlackboard() { return blueTeamBlackboard; }
    public TeamBlackboard GetRedTeamBlackboard() { return redTeamBlackboard; }

}
