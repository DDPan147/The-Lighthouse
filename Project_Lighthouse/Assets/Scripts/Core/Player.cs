using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{
    public bool canMove;

    public float speed;
    public float transitionSpeed;

    public float distancePercentage;
    public float transitionPercentage;

    public float splineLength;

    public float moveVector;

    public Spline transitionSpline;
    public SplineContainer spline;
    public SplineSwitch activeSplineSwitch;


    public MinigameSwitch activeMinigameSwitch;
    public TMP_Text triggerMinigameText;

    private GameManager gm;

    public enum MoveStates
    {
        Control,
        Transition
    }

    public MoveStates moveState;

    void Start()
    {
        splineLength = spline.CalculateLength();
        canMove = true;
        gm = FindAnyObjectByType<GameManager>();

        foreach(UnityEvent uEvent in gm.OnMinigameEnded)
        {
            uEvent.AddListener(RevertTransition);
        }
    }

    void Update()
    {
        if (!GameManager.minigameActive)
        {
            if (moveState == MoveStates.Control)
            {
                ControlState();
            }
            else if (moveState == MoveStates.Transition)
            {
                TransitionState();
            }
        }
    }

    private void TransitionState()
    {
        //transitionPercentage += speed * Time.deltaTime / splineLength;
        //transitionPercentage = Mathf.Clamp01(transitionPercentage);

        Vector3 currentPosition = transitionSpline.EvaluatePosition(transitionPercentage);
        transform.position = currentPosition;
    }

    private void ControlState()
    {
        
        moveVector = Input.GetAxis("Horizontal");
        if (!canMove) moveVector = 0;

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

        if(activeMinigameSwitch != null && Input.GetKeyDown(KeyCode.Z))
        {
            if (activeMinigameSwitch.usesStartPosition)
            {
                transitionSpline = BuildTransitionSpline(transform.position, activeMinigameSwitch.startPosition.position);

                Action triggerMinigame = new Action(activeMinigameSwitch.TriggerMinigame);

                StartTransition(triggerMinigame);

                
            }
            else
            {
                activeMinigameSwitch.TriggerMinigame();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SplineSwitch") && moveState != MoveStates.Transition)
        {
            CheckIsButtonSpline(other.gameObject.GetComponent<SplineSwitch>());
        }
        if (other.gameObject.CompareTag("MinigameSwitch_Core"))
        {
            CheckCurrentMinigameSwitch(other.GetComponent<MinigameSwitch>());
        }
        if (other.gameObject.CompareTag("MissionTrigger_Core"))
        {
            other.gameObject.GetComponent<MissionTrigger>().TriggerMission();
        }
    }

    void OnTriggerStay(Collider other)
    {
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
                UnassignActiveSplineSwitch();
            }
        }
        if (other.gameObject.CompareTag("MinigameSwitch_Core"))
        {
            UnassignActiveMinigameSwitch();
        }
    }
    #region SplineSystem
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

    public void UnassignActiveSplineSwitch()
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

    public void StartTransition(Action triggerMinigame)
    {
        moveState = MoveStates.Transition;
        float duration = transitionSpline.GetLength() * transitionSpeed;
        DOTween.To(() => transitionPercentage, x => transitionPercentage = x, 1, duration).OnComplete(() => triggerMinigame());
        //Play Transition Anim
    }

    void RevertTransition()
    {
        moveState = MoveStates.Transition;
        float duration = transitionSpline.GetLength() * transitionSpeed;
        DOTween.To(() => transitionPercentage, x => transitionPercentage = x, 0, duration).OnComplete(() => EndTransition());//OnComplete(() => completeAction());
        //Play Transition Anim
    }

    void EndTransition()
    {
        moveState = MoveStates.Control;
        transitionPercentage = 0;
    }
    #endregion
    #region MinigameSystem
    void CheckCurrentMinigameSwitch(MinigameSwitch _switch)
    {
        MinigameData minigame = FindFirstObjectByType<GameManager>().minigames[_switch.minigameIndex];

         if(minigame.isAvailable && !minigame.isCompleted)
        {
            triggerMinigameText.enabled = true;
            activeMinigameSwitch = _switch;
        }
    }
    public void UnassignActiveMinigameSwitch()
    {
        triggerMinigameText.enabled = false;
        activeMinigameSwitch = null;
    }
    #endregion

    public Spline BuildTransitionSpline(Vector3 currentPosition, Vector3 targetPosition)
    {
        Spline newSpline = new Spline(0);
        newSpline.Add(currentPosition);
        newSpline.Add(targetPosition);

        return newSpline;
    }
}
