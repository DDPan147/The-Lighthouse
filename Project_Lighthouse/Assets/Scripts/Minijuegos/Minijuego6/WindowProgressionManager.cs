using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class WindowProgressionManager : MonoBehaviour
{
    [System.Serializable]
    public class WindowSection
    {
        public GameObject windowContainer;
        public DragDrop_Slot[] slots;
        public DragDrop_Item[] glassFragments;
        public CanvasGroup fadeGroup;
        public bool isCompleted = false;
        public WindowCompletionEffect completionEffect;
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab) && Input.GetKeyDown(KeyCode.RightControl))
        {
            DebugGameCompleted();
        }
    }

    [Header("Configuración de Ventanas")]
    [SerializeField] private WindowSection[] windowSections;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float delayBetweenWindows = 1f;
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private MinigameComments mc;
    private int commentPosition;

    [Header("Referencias")]
    [SerializeField] private WindowRestorationManager restorationManager;

    private int currentWindowIndex = 0;
    private bool isTransitioning = false;

    [Header("Cristal Arreglado")]
    public GameObject cristalArreglado;
    public WindowCompletionEffect defCompletionEffect;

    private void Start()
    {
        mc = GetComponent<MinigameComments>();
        InitializeWindows();
        restorationManager.onFragmentPlaced.AddListener(OnFragmentPlaced);
    }
    public void OnFragmentPlaced(int slotPosition)
    {
        if (isTransitioning)
        {
            return;
        }
        CheckCurrentWindowCompletion();
    }

    private void CheckCurrentWindowCompletion()
    {
        WindowSection currentWindow = windowSections[currentWindowIndex];
        
        int completedSlots = 0;
        foreach (var slot in currentWindow.slots)
        {
            if (restorationManager.IsSlotCompleted(slot.slotPosition))
            {
                completedSlots++;
            }
            Debug.Log($"Slot {slot.slotPosition}: {restorationManager.IsSlotCompleted(slot.slotPosition)}");
        }

        if (completedSlots == currentWindow.slots.Length && !currentWindow.isCompleted)
        {
            currentWindow.isCompleted = true;
            Debug.Log($"Ventana {currentWindowIndex} completada! ({completedSlots}/{currentWindow.slots.Length} slots)");
            StartCoroutine(TransitionToNextWindow());
        }
        else
        {
            Debug.Log($"Ventana actual tiene {completedSlots}/{currentWindow.slots.Length} slots completados");
        }
    }

    private void InitializeWindows()
    {
        for (int i = 0; i < windowSections.Length; i++)
        {
            bool isActive = i == 0;
            windowSections[i].windowContainer.SetActive(isActive);
            if (windowSections[i].fadeGroup != null)
            {
                windowSections[i].fadeGroup.alpha = isActive ? 1f : 0f;
                windowSections[i].fadeGroup.interactable = isActive;
                windowSections[i].fadeGroup.blocksRaycasts = isActive;
            }
        }
        StartCoroutine(StartComments());

    }
    private IEnumerator StartComments()
    {
        yield return new WaitForSeconds(.3f);
        mc.DisplayComment(commentPosition);
        commentPosition++;
    }

    private IEnumerator TransitionToNextWindow()
    {
        isTransitioning = true;
        WindowSection currentWindow = windowSections[currentWindowIndex];

        if (currentWindow.fadeGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                currentWindow.fadeGroup.alpha = Mathf.Lerp(1f, 0.3f, elapsedTime / fadeOutDuration);
                yield return null;
            }
            currentWindow.fadeGroup.interactable = false;
            currentWindow.fadeGroup.blocksRaycasts = false;
        }
        if (currentWindow.completionEffect != null)
        {
            currentWindow.completionEffect.PlayCompletionEffect();
            // Esperar a que termine el efecto antes de continuar
            yield return new WaitForSeconds(currentWindow.completionEffect.sweepDuration);
        }

        yield return new WaitForSeconds(delayBetweenWindows);

        currentWindowIndex++;
        if (currentWindow.glassFragments != null && currentWindow.glassFragments.Length > 0)
        {
            foreach (var fragment in currentWindow.glassFragments)
            {
                if (fragment != null)
                {
                    fragment.gameObject.SetActive(false);
                }
            }
        }
        currentWindow.windowContainer.SetActive(false);

        if (currentWindowIndex < windowSections.Length)
        {
            // Activar y mostrar la siguiente ventana
            WindowSection nextWindow = windowSections[currentWindowIndex];
            nextWindow.windowContainer.SetActive(true);
            mc.DisplayComment(commentPosition);

            if (nextWindow.fadeGroup != null)
            {
                nextWindow.fadeGroup.interactable = true;
                nextWindow.fadeGroup.blocksRaycasts = true;
                nextWindow.fadeGroup.alpha = 0f;
                
                float elapsedTime = 0f;
                while (elapsedTime < fadeOutDuration)
                {
                    elapsedTime += Time.deltaTime;
                    nextWindow.fadeGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeOutDuration);
                    yield return null;
                }
            }
            commentPosition++;
        }
        else
        {
            ShowCompletionEffect();
        }

        isTransitioning = false;
    }

    private void ShowCompletionEffect()
    {
        if (cristalArreglado != null)
        {
            cristalArreglado.SetActive(true);
            StartCoroutine(PlayAnimationCompleted());
        }
        
    }

    private IEnumerator PlayAnimationCompleted()
    {
        if (defCompletionEffect != null)
        {
            if (SoundManager.instance != null)
            {
                SoundManager.instance.Play("PuzleSolvedColorsin");
            }
            defCompletionEffect.PlayCompletionEffect();
            yield return new WaitForSeconds(defCompletionEffect.sweepDuration + 0.5f);
            if (completionPanel != null)
            {
                completionPanel.SetActive(true);
            }

            /*Alvaro*/ //Function to complete minigame and return to lobby
            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm != null)
            {
                gm.MinigameCompleted(5);
            }
            else
            {
                Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
            }
        }
        else
        {
            Debug.LogWarning("No se ha asignado un efecto de finalización por defecto.");
        }

        
    }

    private void DebugGameCompleted()
    {/*Alvaro*/ //Function to complete minigame and return to lobby
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.MinigameCompleted(5);
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el Game Manager de la escena principal. No se va a volver al juego");
        }

    }
}
