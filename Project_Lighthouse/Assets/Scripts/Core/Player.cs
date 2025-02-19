using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{
    public SplineContainer spline;
    public Spline transitionSpline;

    public float speed;
    public float transitionSpeed;

    public float distancePercentage;
    public float transitionPercentage;

    public float splineLength;

    public float moveVector;

    public SplineSwitch activeSplineSwitch;

    public enum MoveStates
    {
        Control,
        Transition
    }

    public MoveStates moveState;

    void Start()
    {
        splineLength = spline.CalculateLength();
    }

    void Update()
    {
        if(moveState == MoveStates.Control)
        {
            moveVector = Input.GetAxis("Horizontal");

            distancePercentage -= moveVector * speed * Time.deltaTime / splineLength;
            distancePercentage = Mathf.Clamp01(distancePercentage);

            Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
            transform.position = currentPosition;


            Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
            Vector3 direction = nextPosition - currentPosition;
            Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
            if (newDirection.magnitude > 0)
            {
                transform.rotation = Quaternion.LookRotation(newDirection, transform.up);
            }

            if (activeSplineSwitch != null)
            {
                activeSplineSwitch.SwitchSpline();
            }
        }
        else if(moveState == MoveStates.Transition)
        {
            //transitionPercentage += speed * Time.deltaTime / splineLength;
            //transitionPercentage = Mathf.Clamp01(transitionPercentage);

            Vector3 currentPosition = transitionSpline.EvaluatePosition(transitionPercentage);
            transform.position = currentPosition;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SplineSwitch") && moveState != MoveStates.Transition)
        {
            CheckIsButtonSpline(other.gameObject.GetComponent<SplineSwitch>());
        }
        if (other.gameObject.CompareTag("DialogueTrigger_Core"))
        {
            other.gameObject.GetComponent<DialogueTrigger>().TriggerComment();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SplineSwitch") && moveState != MoveStates.Transition)
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

    public void StartTransition()
    {
        moveState = MoveStates.Transition;
        float duration = transitionSpline.GetLength() * transitionSpeed;
        DOTween.To(() => transitionPercentage, x => transitionPercentage = x, 1, duration).OnComplete(() => EndTransition());
        //Play Transition Anim
    }

    void EndTransition()
    {
        moveState = MoveStates.Control;
        transitionPercentage = 0;
    }
}
