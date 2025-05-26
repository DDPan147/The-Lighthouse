using System;
using System.Collections;
using UnityEngine;

public class ClockManager : MonoBehaviour
{
    public static ClockManager Instance;
    public GearSlot[] gearSlots;
    public ParticleSystem completionParticles;
    public AudioSource completionSound;
    public bool isClockFixed = false;
    [SerializeField] private ClockRepairMode repairMode;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void CheckCompletion()
    {
        if (isClockFixed) return;

        bool allCorrect = true;
        foreach (GearSlot slot in gearSlots)
        {
            if (!slot.isOccupied || slot.currentGear.correctSlotPosition != slot.slotPosition)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("All clocks are occupied");
            CompleteClockRepair();
        }
    }

    private void CompleteClockRepair()
    {
        isClockFixed = true;

        // Efectos de completación
        // if (completionParticles != null)
        //     completionParticles.Play();
        //     
        // if (completionSound != null)
        //     completionSound.Play();

        // Notificar al modo de reparación
        StartCoroutine(CompleteClockLevel());
    }

    IEnumerator CompleteClockLevel()
    {
        yield return new WaitForSeconds(1f); // Espera para ver la animación
        if (repairMode != null)
        {
            repairMode.OnClockFixed();
        }
        MinigameFourManager.Instance.OnClockFixed();
    }


}
