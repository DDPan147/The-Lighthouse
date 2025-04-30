using UnityEngine;
using UnityEngine.Splines;

public class CutsceneEndPatch : MonoBehaviour
{
    public SplineContainer spline;
    public int knotIndex;
    public void CreateTransitionForPlayer()
    {
        FindAnyObjectByType<Player>().CreateNewTransition(spline, knotIndex);
    }
}
