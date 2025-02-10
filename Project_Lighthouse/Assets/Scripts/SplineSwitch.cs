using UnityEngine;
using UnityEngine.Splines;

public class SplineSwitch : MonoBehaviour
{
    public SplineContainer[] splines = new SplineContainer[2];

    Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        
    }

    public void SwitchSpline()
    {
        Debug.Log("a");
        if(player.spline = splines[0])
        {
            player.spline = splines[1];
        }
        else
        {
            player.spline = splines[0];
        }
    }
}
