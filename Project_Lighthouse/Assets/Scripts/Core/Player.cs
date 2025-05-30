using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.Splines.Interpolators;

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

    [HideInInspector]public RoomPart currentRoom;

    [HideInInspector]public float splineLength;

    [HideInInspector]public static float moveVector;

    [HideInInspector]public Spline transitionSpline;
    public SplineContainer spline;
    private SplineSwitch activeSplineSwitch;
    public TMP_Text triggerSwitchText;


    private MinigameSwitch activeMinigameSwitch;
    public TMP_Text triggerMinigameText;

    private UnlockLadder activeLadderSwitch;
    public TMP_Text triggerLadderText;

    [HideInInspector] public static bool interact;

    private GameManager gm;


    

    public enum MoveStates
    {
        Control,
        Transition
    }

    public MoveStates moveState;

    //Animation&Sound
    [SerializeField] private SoundManager sm;
    [SerializeField] public Animator meshAnimator;
    [SerializeField] private Material faceMaterial;
    private int faceIndex;

    private bool cutsceneEndPatch;
    private int walkState;
    private float walkAmount;
    [SerializeField]private bool hasTorchlight;
    [SerializeField]private bool isOnStairs;

    [SerializeField] private bool isOnGrass;


    #region Unity Functions and State Machine
    void Start()
    {
        splineLength = spline.CalculateLength();
        canMove = true;
        gm = FindAnyObjectByType<GameManager>();
        sm = FindAnyObjectByType<SoundManager>();
        faceIndex = 1;
    }

    /*void StartUp()
    {

    }*/
    void Update()
    {
        Debug.Log("MinigameActive " + GameManager.minigameActive);
        Debug.Log("CutsceneActive " + GameManager.cutsceneActive);
        AnimationAndSoundControl();
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
        else if (GameManager.minigameActive)
        {
            walkAmount = 0;
        }
    }

    private void CutsceneState()
    {
        //Player Movement Across Spline
        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        //Player Rotation Across Spline
        Vector3 direction;
        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        direction = nextPosition - currentPosition;
        /*if (lastPosition == Vector3.zero)
        {
            
        }
        else
        {
            direction = currentPosition - lastPosition;
        }*/

        lastPosition = currentPosition;
        Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
        if (newDirection.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(newDirection, transform.up);
        }
        triggerSwitchText.enabled = false;
    }
    private void TransitionState()
    {
        Vector3 currentPosition = transitionSpline.EvaluatePosition(transitionPercentage);
        transform.position = currentPosition;
        triggerSwitchText.enabled = false;

        if (canMove || cutsceneEndPatch)
        {
            walkAmount = 1;
        }
        else
        {
            walkAmount = 0;
        }

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
            if (Vector3.Cross(newDirection, transform.up) != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(newDirection, transform.up);
            }

        }
    }

    private void ControlState()
    {

        //Transform Input Data into Movement
        //moveVector = Input.GetAxis("Horizontal");
        if (!canMove) moveVector = 0;
        distancePercentage -= moveVector * speed * Time.deltaTime / splineLength;
        distancePercentage = Mathf.Clamp01(distancePercentage);


        //For animator
        walkAmount = moveVector;

        //Player Movement Across Spline
        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        //Player Rotation Across Spline
        if (!spline.GetComponent<SplineAdditionalData>().isLadder)
        {
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
                if (Vector3.Cross(newDirection, transform.up) != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(newDirection, transform.up);
                }

            }
        }
        else
        {
            Vector3 direction;
            Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
            direction = nextPosition - currentPosition;
            Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
            if (newDirection.magnitude > 0)
            {
                if (Vector3.Cross(newDirection, transform.up) != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(newDirection, transform.up);
                }

            }
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

        if(activeMinigameSwitch != null && canMove && interact)
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
        else if (activeLadderSwitch != null && canMove && interact)
        {
            activeLadderSwitch.UnlockTheLadder();
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
            Debug.Log(other.gameObject.name);
            other.gameObject.GetComponent<MissionTrigger>().TriggerMission();
        }
        if (other.gameObject.CompareTag("CutsceneTrigger_Core"))
        {
            other.gameObject.GetComponent<CutsceneTrigger>().TriggerCutscene();
        }
        if (other.gameObject.CompareTag("UnlockLadder_Core"))
        {
            CheckCurrentLadderSwitch(other.GetComponent<UnlockLadder>());
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("DialogueTrigger_Core"))
        {
            if (moveState != MoveStates.Control) return;
            if (GameManager.cutsceneActive) return;
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
    #region Input Events
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>().x;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interact = true;
        }
        else
        {
            interact = false;
        }

    }
    #endregion
    #region SplineSystem
    void CheckIsButtonSpline(SplineSwitch _switch)
    {

        if (GameManager.cutsceneActive)
        {
            return;
        }
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
            if(activeSplineSwitch != null) activeSplineSwitch.SetHighlight(false);
            activeSplineSwitch = _switch;
            activeSplineSwitch.SetHighlight(true);
        }
        else
        {
            _switch.SwitchSpline();
        }
    }

    public void UnassignActiveSplineSwitch()
    {
        activeSplineSwitch.SetHighlight(false);
        activeSplineSwitch = null;
    }

    public void StartTransition()
    {
        moveState = MoveStates.Transition;
        float duration = transitionSpline.GetLength() / transitionSpeed;
        DOTween.To(() => transitionPercentage, x => transitionPercentage = x, 1, duration).OnComplete(() => EndTransition()).SetEase(Ease.Linear);
        //Play Transition Anim
    }

    public void StartTransition(Action onCompleteAction)
    {
        moveState = MoveStates.Transition;
        float duration = transitionSpline.GetLength() / transitionSpeed;
        DOTween.To(() => transitionPercentage, x => transitionPercentage = x, 1, duration).OnComplete(() => onCompleteAction()).SetEase(Ease.Linear);
        //Play Transition Anim
    }

    public void CreateNewTransition(SplineContainer _spline, int knotIndex)
    {
        Spline newSpline = new Spline(0);
        newSpline.Add(transform.position);
        newSpline.Add(_spline[0][knotIndex].Position);
        cutsceneEndPatch = true;


        Action cutsceneTransitionPatch = new Action(() =>
        {
            cutsceneEndPatch = false;
            EndTransition();
        });

        transitionSpline = newSpline;
        StartTransition();
        ChangeSpline(_spline);
        distancePercentage = SplineSwitch.GetTForKnot(spline[0], knotIndex);
    }

    public void RevertTransition()
    {
        moveState = MoveStates.Transition;
        float duration = transitionSpline.GetLength() / transitionSpeed;
        DOTween.To(() => transitionPercentage, x => transitionPercentage = x, 0, duration).OnComplete(() => EndTransition());//OnComplete(() => completeAction());
        //Play Transition Anim
    }

    public void EndTransition()
    {
        moveState = MoveStates.Control;
        transitionPercentage = 0;
        if(activeSplineSwitch != null)
        {
            activeSplineSwitch.SetHighlight(true);
        }
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
            //activeMinigameSwitch.Awawawa(true); No se por que cojones esto da puto error wtf
        }
    }
    public void UnassignActiveMinigameSwitch()
    {
        triggerMinigameText.enabled = false;
        //activeMinigameSwitch.Awawawa(false);
        activeMinigameSwitch = null;
    }
    #endregion
    #region UnlockableLadder
    void CheckCurrentLadderSwitch(UnlockLadder _switch)
    {
        //MinigameData minigame = FindAnyObjectByType<GameManager>().minigames[_switch.minigameIndex];

        if (_switch.isOpen)
        {
            triggerLadderText.enabled = true;
            activeLadderSwitch = _switch;
        }
    }
    public void UnassignActiveLadderSwitch()
    {
        triggerLadderText.enabled = false;
        activeLadderSwitch = null;
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
    public void ToggleCollider()
    {
        GetComponent<Collider>().enabled = true;
    }
    public void UntoggleCollider()
    {
        GetComponent<Collider>().enabled = false;
    }
    public void OpenDoor()
    {
        Debug.Log("Se abre la puerta");
        if (activeSplineSwitch.CheckInOut())
        {
            if (!activeSplineSwitch.doorReverse)
            {
                activeSplineSwitch.highlightIn.transform.parent.GetComponent<Animator>().Play("OpenDoorInside");
            }
            else
            {
                activeSplineSwitch.highlightIn.transform.parent.GetComponent<Animator>().Play("OpenDoor");
            }
            
        }
        else
        {
            if (!activeSplineSwitch.doorReverse)
            {
                activeSplineSwitch.highlightOut.transform.parent.GetComponent<Animator>().Play("OpenDoor");
            }
            else
            {
                activeSplineSwitch.highlightOut.transform.parent.GetComponent<Animator>().Play("OpenDoorInside");
            }
        }
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

    public void AnimPatch_Position(Transform desiredPosition)
    {
        transform.position = desiredPosition.position;
    }

    public void AnimPatch_Rotation(float rot)
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rot, transform.eulerAngles.z);
    }
    #endregion
    #region Animations
    public void TextureChange(int index)
    {
        faceIndex = index;
    }
    void AnimationAndSoundControl()
    {

        //FaceTexture
        faceMaterial.mainTextureOffset = new Vector2(faceMaterial.mainTextureOffset.x, -0.1f * faceIndex);

        //Cutscene or Gameplay
        meshAnimator.SetBool("isCutscene", GameManager.cutsceneActive);

        if (GameManager.cutsceneActive)
        {
            return;
        }
        //isWalk
        bool isWalk;
        if (moveState == MoveStates.Control)
        {
            isWalk = walkAmount != 0 && distancePercentage != 1 && distancePercentage != 0;
        }
        else
        {
            isWalk = walkAmount != 0;
        }
         
        meshAnimator.SetBool("isWalk", isWalk);

        if (isWalk)
        {
            CalculateGrass();
            if (isOnGrass)
            {
                string[] soundNames = new string[] {"PasosHierba1", "PasosHierba2", "PasosHierba3" };
                if(!sm.FindSound("PasosHierba1").source.isPlaying && !sm.FindSound("PasosHierba2").source.isPlaying && !sm.FindSound("PasosHierba3").source.isPlaying)
                {
                    sm.PlayRandomInRange(soundNames);
                }
            }
            else
            {
                string[] soundNames = new string[] { "PasosMadera1", "PasosMadera2", "PasosMadera3" };
                if (!sm.FindSound("PasosMadera1").source.isPlaying && !sm.FindSound("PasosMadera2").source.isPlaying && !sm.FindSound("PasosMadera3").source.isPlaying)
                {
                    sm.PlayRandomInRange(soundNames); 
                }
            }

        }

        //walkState


        if (moveState == MoveStates.Control)
        {
            GetWalkState();
        }
        else
        {
            walkState = 0;
        }

        meshAnimator.SetInteger("WalkState", walkState);




    }

    private void GetWalkState()
    {
        CalculateStairs();
        
        if (hasTorchlight)
        {
            walkState = 1;
        }
        else if (isOnStairs)
        {
            if (walkAmount < 0)
            {
                walkState = 3;
            }
            else if (walkAmount > 0)
            {
                walkState = 2;
            }
        }
        else if (spline.gameObject.GetComponent<SplineAdditionalData>().isLadder)
        {
            if (walkAmount < 0)
            {
                walkState = 4;
            }
            else if (walkAmount > 0)
            {
                walkState = 5;
            }
        }
        else
        {
            walkState = 0;
        }

        if (walkState > 1)
        {
            if (walkAmount == 0)
            {
                meshAnimator.speed = 0;
            }
            else
            {
                meshAnimator.speed = 1;
            }
        }
        else
        {
            meshAnimator.speed = 1;
        }
    }

    private void CalculateGrass()
    {
        var datavaluesA = spline[0].GetOrCreateFloatData("IsGrass");

        if (datavaluesA.Count != 0)
        {
            int segment = GetCurrentSegment(spline[0], distancePercentage);

            if (ExistsWithinArray(datavaluesA, segment) && ExistsWithinArray(datavaluesA, segment + 1))
            {
                isOnGrass = true;
            }
            else
            {
                isOnGrass = false;
            }
        }
        else
        {
            isOnGrass = false;
        }
    }
    private void CalculateStairs()
    {
        var datavaluesA = spline[0].GetOrCreateFloatData("IsStairs");

        if (datavaluesA.Count != 0)
        {
            int segment = GetCurrentSegment(spline[0], distancePercentage);

            if (ExistsWithinArray(datavaluesA, segment) && ExistsWithinArray(datavaluesA, segment + 1))
            {
                isOnStairs = true;
            }
            else
            {
                isOnStairs = false;
            }
        }
        else
        {
            isOnStairs = false;
        }
    }

    int GetCurrentSegment(Spline spline, float t)
    {


        float totalLength = spline.GetLength();

        float accumulatedLength = 0f;

        for (int i = 0; i < spline.GetCurveCount(); i++)
        {
            float segmentLength = spline.GetCurveLength(i);

            float normalizedStart = accumulatedLength / totalLength;
            float normalizedEnd = (accumulatedLength + segmentLength) / totalLength;

            if (t >= normalizedStart && t <= normalizedEnd)
            {
                return i;
            }

            accumulatedLength += segmentLength;
        }

        return -1;
    }

    bool ExistsWithinArray(SplineData<float> dataPoints, int knot)
    {
        foreach(var point in dataPoints)
        {
            if(point.Index == knot)
            {
                return true;
            }
        }

        return false;
    }

    public void GetTorchlight(bool b)
    {
        hasTorchlight = b;
    }

    #endregion
}
