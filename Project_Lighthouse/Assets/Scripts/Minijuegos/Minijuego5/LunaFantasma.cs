using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class LunaFantasma : MonoBehaviour
{
    public SplineContainer spline;
    private float distancePercentage;

    public Material mat;

    public bool move;
    void Start()
    {

    }

    void Update()
    {
        if (move)
        {
            Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
            transform.position = currentPosition;

            Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
            Vector3 direction = nextPosition - currentPosition;
            transform.rotation = Quaternion.LookRotation(direction, transform.up);
        }
    }

    public void ToggleAnim()
    {
        mat.DOFloat(1, "_Alpha", 1).OnComplete(() =>
        {
            Invoke("Dissappear", 1f);
        });

        if (move)
        {
            DOTween.To(() => distancePercentage, x => distancePercentage = x, 1, 3).SetEase(Ease.Linear);
        }
    }

    void Dissappear()
    {
        mat.DOFloat(0, "_Alpha", 1).OnComplete(() => { Destroy(this.gameObject); });
    }
}
