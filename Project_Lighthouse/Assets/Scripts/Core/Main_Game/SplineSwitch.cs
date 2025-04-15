using System;
using UnityEngine;
using UnityEngine.Splines;

public class SplineSwitch : MonoBehaviour
{
    public SplineContainer[] splines = new SplineContainer[2];

    [Tooltip("Does this spline switch work automatically, or a button has to be pressed? (In is always 0 to 1)")]
    public bool isButtonIn;
    [Tooltip("Does this spline switch work automatically, or a button has to be pressed? (Out is always 1 to 0)")]
    public bool isButtonOut;
    [Tooltip("Where does the player spawn when changing spline? (In is always 0 to 1)")]
    public float percentageIn;
    [Tooltip("Where does the player spawn when changing spline? (Out is always 1 to 0)")]
    public float percentageOut;
    [Tooltip("For transitions, the knot index to redirect the player. (In is always 0 to 1)")]
    public int transitionKnotIndexIn;
    [Tooltip("For transitions, the knot index to redirect the player. (Out is always 1 to 0)")]
    public int transitionKnotIndexOut;



    Player player;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    void Update()
    {

    }

    public void SwitchSpline()
    {
        //IN
        if (CheckInOut())
        {
            if (isButtonIn && !Input.GetKeyDown(KeyCode.Z))
            {
                return;
            }

            player.spline = splines[1];
            player.splineLength = splines[1].CalculateLength();
            player.distancePercentage = GetTForKnot(splines[1].Spline, transitionKnotIndexIn);
            player.transitionSpline = BuildTransitionSpline(player.transform.position, splines[1], splines[1][0][transitionKnotIndexIn]);
            player.StartTransition();

            if (!isButtonOut)
            {
                player.UnassignActiveSplineSwitch();
            }

        }

        //OUT
        else
        {
            if (isButtonOut && !Input.GetKeyDown(KeyCode.Z))
            {
                return;
            }

            player.spline = splines[0];
            player.splineLength = splines[0].CalculateLength();
            player.distancePercentage = GetTForKnot(splines[0].Spline, transitionKnotIndexOut);

            player.transitionSpline = BuildTransitionSpline(player.transform.position, splines[0], splines[0][0][transitionKnotIndexOut]);
            player.StartTransition();

            if (!isButtonIn)
            {
                player.UnassignActiveSplineSwitch();
            }
        }
    }

    public bool CheckInOut()
    {
        return player.spline == splines[0];
    }

                //Construir una spline que transicione desde la posición actual del jugador hasta la posición de la spline que tenga el porcentaje que designamos
                    //Crear una spline de 2 nudos
                    //Acceder a los knots de la spline y cambiarles la posición
                    //Knot numero 1: Posición del jugador
                    //Knot número 2: Accede a la spline deseada en el porcentaje deseado
                //Pasar la spline resultado al player
                //El player designa un estado de transición
                //El player se mueve a través de la spline de transición automáticamente
    public Spline BuildTransitionSpline(Vector3 currentPosition, SplineContainer targetSpline, BezierKnot knot)
    {
        Spline newSpline = new Spline(0);
        newSpline.Add(currentPosition);
        newSpline.Add(knot.Position);

        return newSpline;
    }

    public float GetTForKnot(Spline spline, int knotIndex)
    {
        if (knotIndex < 0 || knotIndex >= spline.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(knotIndex), "Índice de nodo fuera de rango.");
        }

        float totalLength = spline.GetLength(); // Longitud total de la spline
        float accumulatedLength = 0f;

        // Sumar la longitud de los segmentos hasta llegar al nodo deseado
        for (int i = 0; i < knotIndex; i++)
        {
            accumulatedLength += spline.GetCurveLength(i);
        }

        return accumulatedLength / totalLength; // Proporción de la spline recorrida hasta el nodo
    }
}
