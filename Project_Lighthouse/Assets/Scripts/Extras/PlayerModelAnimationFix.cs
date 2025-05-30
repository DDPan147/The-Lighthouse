using UnityEngine;

public class PlayerModelAnimationFix : MonoBehaviour
{
    private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewEvent()
    {
        player.OpenDoor();
    }
}
