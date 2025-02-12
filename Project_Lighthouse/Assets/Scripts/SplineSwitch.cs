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
        player = FindFirstObjectByType<Player>();
    }

    void Update()
    {
        
    }

    public void SwitchSpline()
    {
        //IN
        if (CheckInOut())
        {
            Debug.Log("I am here too");
            if (isButtonIn && !Input.GetKeyDown(KeyCode.Z))
            {
                return;
            }

            player.spline = splines[1];
            player.splineLength = splines[1].CalculateLength();
            player.distancePercentage = percentageIn;

            if (!isButtonOut)
            {
                player.UnassignActiveSwitch();
            }

        }

        //OUT
        else
        {
            Debug.Log("Im here");
            if (isButtonOut && !Input.GetKeyDown(KeyCode.Z))
            {
                return;
            }
            Debug.Log("Im here 2");

            player.spline = splines[0];
            player.distancePercentage = percentageOut;

            if (!isButtonIn)
            {
                player.UnassignActiveSwitch();
            }
        }
    }

    public bool CheckInOut()
    {
        return player.spline == splines[0];
    }

    //Construir una spline que transicione desde la posición actual del jugador hasta la posición de la spline que tenga el porcentaje que designamos
        //Crear una spline de 2 nudos **
        //Acceder a los knots de la spline y cambiarles la posición **
        //Knot numero 1: Posición del jugador **
        //Knot número 2: Accede a la spline deseada en el porcentaje deseado
    //Pasar la spline resultado al player
    //El player designa un estado de transición
    //El player se mueve a través de la spline de transición automáticamente
    public Spline BuildTransitionSpline(Vector3 currentPosition, SplineContainer targetSpline)
    {
        Spline newSpline = new Spline(2);
        newSpline.this[0] = currentPosition;
        newSpline.this[1] = targetSpline.EvaluatePosition();
    }
}
