using UnityEngine;

public class Tuberia : MonoBehaviour
{
    public bool isPart1Connected, isPart2Connected, isConnected;

    void Update()
    {
        if(isPart1Connected && isPart2Connected)
        {
            isConnected = true;
        }
    }

}
