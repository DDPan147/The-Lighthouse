using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;
using System.Xml.Linq;
using EasyTextEffects;

[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Tooltip("The amount of time the Dialogue waits between phrases")]
    [SerializeField]
    private float waitTime = 1.0f;
    [Tooltip("The amount of frames the Dialogue waits between characters")]
    [SerializeField]
    private int letterWaitFrames;

    [SerializeField]private float popupScaleDuration;
    private bool sentenceTyped;
    private bool sentenceSkipped;

    private Queue<Sentence> sentences = new Queue<Sentence>();

    public enum Speaker
    {
        Abuelo,
        Luna
    }
    public enum Emotion
    {
        Default,
        Abuelo_Triste,
        Abuelo_Molesto,
        Abuelo_Enfadado,
        Abuelo_Alegre,
        Abuelo_Pregunta,
        Abuelo_Asustado,
        Abuelo_Preocupado,
        Luna_Triste,
        Luna_Asco,
        Luna_Indignada,
        Luna_Traviesa,
        Luna_Alegre,
        Luna_Emocionada,
        Luna_Pregunta,
        Luna_Preocupada,

    }

    [SerializeField] private TMP_Text textDisplayGUI;
    [SerializeField] private TMP_Text textDisplayAbuelo;
    [SerializeField] private TMP_Text textDisplayLuna;

    private SoundManager sm;

    private void Awake()
    {
        //Singleton
        /*if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);*/
        //
    }
    void Start()
    {
        sm = FindFirstObjectByType<SoundManager>();
    }

    private void Update()
    {

    }
    public void StartComment(DialogueComment _comment)
    {
        sentences.Clear();
        StopAllCoroutines();

        foreach (Sentence sentence in _comment.sentences)
        {
            if(sentence.sentenceEvent.eventName == "")
            {
                sentence.sentenceEvent.active = false;
            }
            sentences.Enqueue(sentence);

        }

        //SPEAKERS
        Speaker currentSpeaker = sentences.Peek().speaker;
        TMP_Text nextTarget = DecideNextSpeaker(_comment.type, currentSpeaker);
        DisplayNextSentence(nextTarget);
    }

    private TMP_Text DecideNextSpeaker(DialogueComment.DialogueTypes commentType, Speaker currentSpeaker)
    {
        if (commentType == DialogueComment.DialogueTypes.Popup)
        {
            if (currentSpeaker == Speaker.Abuelo)
            {
                return textDisplayAbuelo;
            }
            else
            {
                return textDisplayLuna;
            }
        }
        else
        {
            return textDisplayGUI;
        }
    }

    private TMP_Text DecideNextSpeaker(Speaker currentSpeaker)
    {
        if (currentSpeaker == Speaker.Abuelo)
        {
            return textDisplayAbuelo;
        }
        else
        {
            return textDisplayLuna;
        }
    }

    void DisplayNextSentence(TMP_Text target)
    {
        if (sentences.Count == 0)
        {
            EndComment(target);
            return;
        }

        Sentence currentSentence = sentences.Dequeue();

        

        //EVENTS
        DialogueEvent nEvent = currentSentence.sentenceEvent;

        if (nEvent.active)
        {
            if (nEvent.eventReceiver == null)
            {
                nEvent.eventReceiver = FindFirstObjectByType<Player>();
            }
            nEvent.eventReceiver.Invoke(nEvent.eventName, nEvent.timeOffset);
        }

        //EMOTIONS
        string emotionSound = EmotionSoundSelector(currentSentence.emotionSound);
        if(emotionSound != null)
        {
            sm.Play(emotionSound);
        }


        //SPEAKERS
        if (target == textDisplayGUI)
        {
            StopAllCoroutines();
            StartCoroutine(TypeSentence(currentSentence.sentenceText, target));

            //target.parent activates
            //target starts typing text
        }
        else
        {
            target.text = currentSentence.sentenceText;
            Debug.Log("Nueva sentence: " + currentSentence.sentenceText);
            target.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            target.transform.GetChild(1).GetComponent<TextEffect>().enabled = false;
            target.transform.parent.transform.DOScale(1, popupScaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                sm.Play("Texto");
                StartCoroutine(WaitForNextSentence(target));
                StartCoroutine(TypeSentence(currentSentence.sentenceText, target.transform.GetChild(1).GetComponent<TMP_Text>()));
            });
            
            
        }

    }
    IEnumerator WaitForNextSentence(TMP_Text target)
    {
        sentenceTyped = false;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (sentenceTyped)
                {
                    CloseBubble(target.transform.parent);
                    yield break;
                }
                else
                {
                    sentenceSkipped = true;
                }
            }
            yield return null;
        }

        

        //Play Talking Sound
    }

    void CloseBubble(Transform target)
    {
        target.DOScale(0, popupScaleDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            Speaker currentSpeaker = sentences.Peek().speaker;
            TMP_Text nextTarget = DecideNextSpeaker(currentSpeaker);
            DisplayNextSentence(nextTarget);
            Debug.Log("He cerrao ventana lol");
        });
    }

    IEnumerator TypeSentence(string _sentence, TMP_Text target)
    {
        
        target.text = "";
        
        bool isTag = false;
        foreach (char letter in _sentence.ToCharArray())
        {
            target.text += letter;

            if(letter == '<') //Si se está escribiendo un tag en el texto, se escribe directamente sin esperar por cada caracter
            {
                isTag = true;
            }

            if (!isTag)
            {
                //Play Letter Sound
                for (int i = 0; i < letterWaitFrames; i++)
                {
                    yield return null;
                }
            }

            if(letter == '>') //Cuando el tag acaba, sigue escribiendo de normal
            {
                isTag = false;
            }

            if (sentenceSkipped)
            {
                target.text = _sentence;
                break;
            }
            
        }
        target.GetComponent<TextEffect>().enabled = true;
        sm.Stop("Texto");
        sentenceTyped = true;
        
    }

    void EndComment(TMP_Text target)
    {
        target.text = "";
        target.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
    }

    string EmotionSoundSelector(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Default:
                return null;
            case Emotion.Abuelo_Triste:
                return "AbueloPregunta";
            case Emotion.Abuelo_Molesto:
                return "AbueloHablar1";
            case Emotion.Abuelo_Enfadado:
                return "AbueloPreocupado";
            /*case Emotion.Abuelo_Alegre:
                break;
            case Emotion.Abuelo_Pregunta:
                break;
            case Emotion.Abuelo_Asustado:
                break;
            case Emotion.Abuelo_Preocupado:
                break;
            case Emotion.Luna_Triste:
                break;
            case Emotion.Luna_Asco:
                break;
            case Emotion.Luna_Indignada:
                break;
            case Emotion.Luna_Traviesa:
                break;
            case Emotion.Luna_Alegre:
                break;
            case Emotion.Luna_Emocionada:
                break;
            case Emotion.Luna_Pregunta:
                break;
            case Emotion.Luna_Preocupada:
                break;*/
            default:
                return null;
        }
    }




    //David: Kike por que vas al dentista
    //Kike: []
    //Álvaro: *decepcionado consigo mismo*
}

