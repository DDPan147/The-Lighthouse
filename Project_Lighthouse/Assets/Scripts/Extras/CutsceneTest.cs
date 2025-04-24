using UnityEngine;

public class CutsceneTest : MonoBehaviour
{
    private CutsceneTrigger cutscene;
    void Start()
    {
        cutscene = GetComponent<CutsceneTrigger>();
    }

    void Update()
    {
        if(cutscene != null)
        {
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                cutscene.cutsceneIndex--;
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                cutscene.cutsceneIndex++;
            }
            if(Input.GetKeyDown(KeyCode.C))
            {
                cutscene.TriggerCutscene();
            }
        }
    }
}
