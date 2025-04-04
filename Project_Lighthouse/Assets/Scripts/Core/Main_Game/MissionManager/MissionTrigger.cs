using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    public string missionName;
    public enum TriggerType {Discover, Complete};
    public TriggerType type;

    private GameManager gm;
    public void Start()
    {
        gm = FindAnyObjectByType<GameManager>();
    }
    public void TriggerMission()
    {
        if (type == TriggerType.Discover)
        {
            gm.UnlockNewMission(missionName);
        }
        else if (type == TriggerType.Complete && gm.GetMission(missionName).isDiscovered)
        {
            gm.CompleteMission(missionName);
        }
    }
}
