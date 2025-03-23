using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    public string missionName;
    public enum TriggerType {Discover, Complete};
    public TriggerType type;

    public void TriggerMission()
    {
        if (type == TriggerType.Discover)
        {
            FindFirstObjectByType<GameManager>().UnlockNewMission(missionName);
        }
        else if (type == TriggerType.Complete)
        {
            FindFirstObjectByType<GameManager>().CompleteMission(missionName);
        }
    }
}
