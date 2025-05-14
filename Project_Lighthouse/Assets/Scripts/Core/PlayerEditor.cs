using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class PlayerEditor : MonoBehaviour
{
    public bool canMove;

    public float speed;
    public float transitionSpeed;

    public float distancePercentage;
    public float transitionPercentage;

    public float splineLength;

    public float moveVector;

    public Spline transitionSpline;
    //public Spline activePath;
    public SplineContainer spline;

    private Player player;

    public enum MoveStates
    {
        Control,
        Transition,
        Cutscene
    }

    public MoveStates moveState;

    void Awake()
    {
        player = GetComponent<Player>();
        splineLength = spline.CalculateLength();
    }

    void Update()
    {
        speed = player.speed;
        transitionSpeed = player.transitionSpeed;
        distancePercentage = player.distancePercentage;
        transitionPercentage = player.transitionPercentage;
        splineLength = player.splineLength;
        moveVector = Player.moveVector;
        transitionSpline = player.transitionSpline;
        spline = player.spline;
        ControlState();

    }

    private void ControlState()
    {

        /*moveVector = Input.GetAxis("Horizontal");
        if (!canMove) moveVector = 0;

        distancePercentage -= moveVector * speed * Time.deltaTime / splineLength;
        distancePercentage = Mathf.Clamp01(distancePercentage);*/

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
}
