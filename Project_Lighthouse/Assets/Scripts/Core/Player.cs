using UnityEngine;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{
    public SplineContainer spline;
    public Spline transitionSpline;

    public float speed;

    public float distancePercentage;

    public float splineLength;

    public float moveVector;

    public SplineSwitch activeSplineSwitch;

    

    void Start()
    {
        splineLength = spline.CalculateLength();
    }

    void Update()
    {
        moveVector = Input.GetAxis("Horizontal");

        distancePercentage -= moveVector * speed * Time.deltaTime / splineLength;
        distancePercentage = Mathf.Clamp01(distancePercentage);

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;


        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
        if(newDirection.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(newDirection, transform.up);
        }

        if (activeSplineSwitch != null)
        {
            activeSplineSwitch.SwitchSpline();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SplineSwitch"))
        {
            CheckIsButtonSpline(other.gameObject.GetComponent<SplineSwitch>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SplineSwitch"))
        {
            if(activeSplineSwitch == other.gameObject.GetComponent<SplineSwitch>())
            {
                UnassignActiveSwitch();
            }
        }
    }

    void CheckIsButtonSpline(SplineSwitch _switch)
    {

        //objetivo: si la switch es auto al entrar, se activa solo en el enter. En caso contrario, se guarda la variable en activeSplineSwitch
        //Confirmar si es in o out
        //Confirmar si la entrada es auto o no
        bool condition; 
        if (_switch.CheckInOut())
        {
            
            condition = _switch.isButtonIn;
        }
        else
        {
            
            condition = _switch.isButtonOut;
        }

        if (condition)
        {
            activeSplineSwitch = _switch;
        }
        else
        {
            _switch.SwitchSpline();
        }
    }

    public void UnassignActiveSwitch()
    {
        activeSplineSwitch = null;
    }
}
