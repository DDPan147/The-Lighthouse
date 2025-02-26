using UnityEngine;

public class MinigameSwitch : MonoBehaviour
{
    public int minigameIndex;
    void Start()
    {

    }
    public void TriggerMinigame()
    {
        FindFirstObjectByType<GameManager>().LoadMinigame(minigameIndex);
    }
}
