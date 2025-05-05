using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public bool canMove;
    [Space(10)]
    public float speed;
    public float distancePercentage;

    [Space(10)]
    public float transitionSpeed;
    public float transitionPercentage;
    private Vector3 lastPosition;

    [HideInInspector]public float splineLength;

    [HideInInspector]public float moveVector;

    [HideInInspector]public Spline transitionSpline;
    public SplineContainer spline;
    private SplineSwitch activeSplineSwitch;
    public TMP_Text triggerSwitchText;


    private MinigameSwitch activeMinigameSwitch;
    public TMP_Text triggerMinigameText;


    private GameManager gm;

    public enum MoveStates
    {
        Control,
        Transition
    }

    [HideInInspector]public MoveStates moveState;

    #region Unity Functions and State Machine
    void Start()
    {
        splineLength = spline.CalculateLength();
        canMove = true;
        gm = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        Debug.Log("MinigameActive " + GameManager.minigameActive);
        Debug.Log("CutsceneActive " + GameManager.cutsceneActive);
        if (!GameManager.minigameActive && !GameManager.cutsceneActive)
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
        else if (GameManager.cutsceneActive)
        {
            CutsceneState();
        }
    }

    private void CutsceneState()
    {
        //Player Movement Across Spline
        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        //Player Rotation Across Spline
        Vector3 direction;
        if (lastPosition == Vector3.zero)
        {
            Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
            direction = nextPosition - currentPosition;
        }
        else
        {
            direction = currentPosition - lastPosition;
        }

        lastPosition = currentPosition;
        Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
        if (newDirection.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(newDirection, transform.up);
        }
        triggerSwitchText.enabled = false;
        Debug.Log("Im on cutscene");
    }
    private void TransitionState()
    {
        Vector3 currentPosition = transitionSpline.EvaluatePosition(transitionPercentage);
        transform.position = currentPosition;
        Debug.Log("Im on transition");
        triggerSwitchText.enabled = false;
    }

    private void ControlState()
    {
        Debug.Log("Im on control");

        //Transform Input Data into Movement
        moveVector = Input.GetAxis("Horizontal");
        if (!canMove) moveVector = 0;
        distancePercentage -= moveVector * speed * Time.deltaTime / splineLength;
        distancePercentage = Mathf.Clamp01(distancePercentage);

        //Player Movement Across Spline
        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        //Player Rotation Across Spline

        Vector3 direction;
        if (lastPosition == Vector3.zero)
        {
            Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
            direction = nextPosition - currentPosition;
        }
        else
        {
            direction = currentPosition - lastPosition;
        }

        lastPosition = currentPosition;
            
        Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
        if (newDirection.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(newDirection, transform.up);
        }


        if (activeSplineSwitch != null && canMove)
        {
            activeSplineSwitch.SwitchSpline();
            triggerSwitchText.enabled = true;
        }
        else
        {
            triggerSwitchText.enabled = false;
        }

        if(activeMinigameSwitch != null && canMove && Input.GetKeyDown(KeyCode.Z))
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
        if (other.gameObject.CompareTag("CutsceneTrigger_Core"))
        {
            other.gameObject.GetComponent<CutsceneTrigger>().TriggerCutscene();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("DialogueTrigger_Core"))
        {
            if (moveState != MoveStates.Control) return;

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
    #endregion
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
            //if(activeSplineSwitch != null) activeSplineSwitch.SetHighlight(false);
            
            activeSplineSwitch = _switch;
            //activeSplineSwitch.SetHighlight(true);
        }
        else
        {
            _switch.SwitchSpline();
        }
    }

    public void UnassignActiveSplineSwitch()
    {
        //activeSplineSwitch.SetHighlight(false);
        activeSplineSwitch = null;
    }

    public void StartTransition()
    {
        moveState = MoveStates.Transition;
        float duration = transitionSpline.GetLength() * transitionSpeed;
        DOTween.To(() => transitionPercentage, x => transitionPercentage = x, 1, duration).OnComplete(() => EndTransition());
        //Play Transition Anim
    }

    public void StartTransition(Action onCompleteAction)
    {
        moveState = MoveStates.Transition;
        float duration = transitionSpline.GetLength() * transitionSpeed;
        DOTween.To(() => transitionPercentage, x => transitionPercentage = x, 1, duration).OnComplete(() => onCompleteAction());
        //Play Transition Anim
    }

    public void CreateNewTransition(SplineContainer _spline, int knotIndex)
    {
        Spline newSpline = new Spline(0);
        newSpline.Add(transform.position);
        newSpline.Add(_spline[0][knotIndex].Position);

        
        transitionSpline = newSpline;
        StartTransition();
        ChangeSpline(_spline);
        distancePercentage = SplineSwitch.GetTForKnot(spline[0], knotIndex);
    }

    public void RevertTransition()
    {
        moveState = MoveStates.Transition;
        float duration = transitionSpline.GetLength() * transitionSpeed;
        DOTween.To(() => transitionPercentage, x => transitionPercentage = x, 0, duration).OnComplete(() => EndTransition());//OnComplete(() => completeAction());
        //Play Transition Anim
    }

    public void EndTransition()
    {
        moveState = MoveStates.Control;
        transitionPercentage = 0;
    }

    /*void TakePath(Spline path, Action onCompleteAction)
    {
        //activePath = path;
        //moveState = MoveStates.Cutscene;
        float duration = transitionSpline.GetLength() * transitionSpeed;
        DOTween.To(() => transitionPercentage, x => transitionPercentage = x, 1, duration).OnComplete(() => onCompleteAction());
    }*/
    public Spline BuildTransitionSpline(Vector3 currentPosition, Vector3 targetPosition)
    {
        Spline newSpline = new Spline(0);
        newSpline.Add(currentPosition);
        newSpline.Add(targetPosition);

        return newSpline;
    }
    #endregion
    #region MinigameSystem
    void CheckCurrentMinigameSwitch(MinigameSwitch _switch)
    {
        MinigameData minigame = FindAnyObjectByType<GameManager>().minigames[_switch.minigameIndex];

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
    #region EventFunctions
    public void ToggleAnimator()
    {
        GetComponent<Animator>().enabled = true;
        lastPosition = Vector3.zero;
    }
    public void UntoggleAnimator()
    {
        GetComponent<Animator>().enabled = false;
        lastPosition = Vector3.zero;
    }
    #endregion
    #region SignalFunctions
    public void ChangeSpline(SplineContainer _spline)
    {
        spline = _spline;
        Debug.Log("My spline is: " + _spline.name);
        splineLength = _spline.CalculateLength();
    }
    public void SetPercentage(float value)
    {
        distancePercentage = value;

        transform.position = spline.EvaluatePosition(distancePercentage);
    }
    #endregion
}
