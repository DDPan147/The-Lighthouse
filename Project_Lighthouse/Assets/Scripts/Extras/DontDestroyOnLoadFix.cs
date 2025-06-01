using UnityEngine;

public class DontDestroyOnLoadFix : MonoBehaviour
{
    public static bool firstLoop = false;
    public static DontDestroyOnLoadFix instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        Invoke("ChangeFirstLoop", 1f);
    }

    void ChangeFirstLoop()
    {
        firstLoop = true;
    }

    void Update()
    {
        
    }
}
