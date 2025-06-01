using UnityEngine;

public class CurtainMenuFix : MonoBehaviour
{
    public static CurtainMenuFix instance;
    public Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!DontDestroyOnLoadFix.firstLoop)
        {
            anim.Play("FadeIn 0");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
