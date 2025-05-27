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

        Debug.Log("Checking completion...");

        bool allCorrect = true;
        int correctCount = 0;

        foreach (GearSlot slot in gearSlots)
        {
            Debug.Log($"Slot {slot.slotPosition}: Occupied={slot.isOccupied}, " +
                      $"Gear={slot.currentGear?.gearID}, " +
                      $"CorrectPos={slot.currentGear?.correctSlotPosition}");

            if (!slot.isOccupied ||
                slot.currentGear == null ||
                slot.currentGear.correctSlotPosition != slot.slotPosition ||
                slot.currentGear.isDragging) // Verificar que no esté siendo arrastrado
            {
                allCorrect = false;
                break;
            }
            correctCount++;
        }

        Debug.Log($"Correct gears in place: {correctCount}/{gearSlots.Length}");

        if (allCorrect && correctCount == gearSlots.Length)
        {
            Debug.Log("All gears correctly placed!");
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
