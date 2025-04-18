using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    public int cutsceneIndex;

    public bool played;
    private Player player;
    void Start()
    {
        player = FindAnyObjectByType<Player>();
    }
    public void TriggerCutscene()
    {
        if (played == false)
        {
            FindAnyObjectByType<GameManager>().CutsceneStart(cutsceneIndex);
            played = true;
        }
    }
}
