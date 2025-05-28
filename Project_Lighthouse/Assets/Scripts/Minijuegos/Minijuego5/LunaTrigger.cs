using UnityEngine;

public class LunaTrigger : MonoBehaviour
{
    public LunaFantasma luna;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        luna.ToggleAnim();
    }
}
