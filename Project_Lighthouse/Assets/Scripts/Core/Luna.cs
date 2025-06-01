using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class Luna : MonoBehaviour
{
    [SerializeField]private SplineContainer spline;
    [SerializeField]private bool attachedToSpline;
    [SerializeField]private float distancePercentage;
    [SerializeField]private Material faceMaterial;
    [SerializeField]private int faceIndex = 1;
    [SerializeField] private Animator meshAnimator;
    void Start()
    {
        faceIndex = 1;
    }

    void Update()
    {
        if (attachedToSpline)
        {
            SplineMovement();
        }
        faceMaterial.mainTextureOffset = new Vector2(faceMaterial.mainTextureOffset.x, -0.1f * faceIndex);
    }

    void SplineMovement()
    {
        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
        if (newDirection.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(newDirection, transform.up);
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void MoveToPosition(Transform target)
    {
        transform.position = target.position;
    }

    public void AttachToSpline(SplineContainer _spline)
    {
        spline = _spline;
        attachedToSpline = true;
    }
    public void UnattachFromSpline()
    {
        spline = null;
        attachedToSpline = false;
    }

    public void SetPercentage(float percentage)
    {
        distancePercentage = percentage;
    }

    public void TextureChange(int index)
    {
        faceIndex = index;
    }
}
