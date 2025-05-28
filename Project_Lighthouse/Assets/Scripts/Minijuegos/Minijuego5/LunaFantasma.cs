using UnityEngine;
using UnityEngine.Splines;

public class LunaFantasma : MonoBehaviour
{
    public SplineContainer spline;
    private float distancePercentage;

    public bool activated;

    [SerializeField]private float speed;
    void Start()
    {
        
    }

    void Update()
    {
        if (activated)
        {
            distancePercentage += speed * Time.deltaTime;

            Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
            transform.position = currentPosition;

            if(distancePercentage > 1f)
        }
    }
}
