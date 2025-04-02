using UnityEngine;

public class MinigameSwitch : MonoBehaviour
{
    public int minigameIndex;
    public bool usesStartPosition;
    public Transform startPosition;
    void Start()
    {

    }
    public void TriggerMinigame()
    {
        FindFirstObjectByType<GameManager>().LoadMinigame(minigameIndex);
    }
}
