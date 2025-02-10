using UnityEngine;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{
    public SplineContainer spline;

    public float speed;

    public float distancePercentage;

    float splineLength;

    public float moveVector;

    void Start()
    {
        splineLength = spline.CalculateLength();
    }

    void Update()
    {
        moveVector = Input.GetAxis("Horizontal");

        distancePercentage += moveVector * speed * Time.deltaTime / splineLength;
        distancePercentage = Mathf.Clamp01(distancePercentage);

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;


        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }
}
