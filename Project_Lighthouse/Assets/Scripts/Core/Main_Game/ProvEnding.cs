using UnityEngine;

public class ProvEnding : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("EndGame", 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EndGame()
    {
        Application.Quit();
        Debug.Log("I quit");
    }
}
