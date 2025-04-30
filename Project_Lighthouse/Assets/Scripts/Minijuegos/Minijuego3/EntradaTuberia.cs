using UnityEngine;

public class EntradaTuberia : MonoBehaviour
{
    public enum PipeParts
    {
        Part1,
        Part2
    }
    public PipeParts whichPart;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tuberia_Entrada"))
        {
            if(whichPart == PipeParts.Part1)
            {
                GetComponentInParent<Tuberia>().isPart1Connected = true;
            }
            else if(whichPart == PipeParts.Part2)
            {
                GetComponentInParent<Tuberia>().isPart2Connected = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Tuberia_Entrada"))
        {
            if (whichPart == PipeParts.Part1)
            {
                GetComponentInParent<Tuberia>().isPart1Connected = true;
            }
            else if (whichPart == PipeParts.Part2)
            {
                GetComponentInParent<Tuberia>().isPart2Connected = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tuberia_Entrada"))
        {
            if (whichPart == PipeParts.Part1)
            {
                GetComponentInParent<Tuberia>().isPart1Connected = false;
            }
            else if (whichPart == PipeParts.Part2)
            {
                GetComponentInParent<Tuberia>().isPart2Connected = false;
            }
        }
    }
}

