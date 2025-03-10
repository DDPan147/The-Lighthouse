using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;
using System.Xml.Linq;

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

    private Queue<string> sentences = new Queue<string>();
    private Queue<Speaker> speakers = new Queue<Speaker>();
    private Queue<DialogueEvent> events = new Queue<DialogueEvent>();

    [SerializeField]private Speaker currentSpeakerTest;
    public enum Speaker
    {
        Abuelo,
        Luna
    }

    private Speaker lastSpeaker;
    public string[] sentencesTest = new string[] { };

    [SerializeField] private TMP_Text textDisplayGUI;
    [SerializeField] private TMP_Text textDisplayAbuelo;
    [SerializeField] private TMP_Text textDisplayLuna;

    private AudioSource sound;

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
        sound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        sentencesTest = sentences.ToArray();
        if (Input.GetKeyDown(KeyCode.M))
        {
            CutComment();
        }
    }
    public void StartComment(DialogueComment _comment)
    {
        sentences.Clear();
        speakers.Clear();
        events.Clear();
        StopAllCoroutines();

        foreach (string sentence in _comment.sentences)
        {
            sentences.Enqueue(sentence);
        }
        foreach (Speaker speaker in _comment.speakers)
        {
            speakers.Enqueue(speaker);
        }

        //EVENTS

        if (_comment.events.Length != 0)
        {
            foreach (DialogueEvent DialogueEvent in _comment.events)
            {
                if (DialogueEvent.eventName == "")
                {
                    DialogueEvent.active = false;
                }
                events.Enqueue(DialogueEvent);
            }
        }

        //SPEAKERS
        Speaker currentSpeaker = speakers.Dequeue();
        currentSpeakerTest = currentSpeaker;
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

        string currentSentence = sentences.Dequeue();



        //EVENTS
        if (events.Count > 0)
        {
            DialogueEvent nEvent = events.Dequeue();

            if (nEvent.active)
            {
                if (nEvent.eventReceiver == null)
                {
                    nEvent.eventReceiver = FindFirstObjectByType<Player>();
                }
                nEvent.eventReceiver.Invoke(nEvent.eventName, nEvent.timeOffset);
            }
        }

        //SPEAKERS
        if (target == textDisplayGUI)
        {
            StopAllCoroutines();
            StartCoroutine(TypeSentence(currentSentence, target));

            //target.parent activates
            //target starts typing text
        }
        else
        {
            target.text = currentSentence;
            target.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            target.transform.parent.transform.DOScale(1, popupScaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                StartCoroutine(WaitForNextSentence(target));
                StartCoroutine(TypeSentence(currentSentence, target.transform.GetChild(1).GetComponent<TMP_Text>()));
            });
            
            
        }

    }
    IEnumerator WaitForNextSentence(TMP_Text target)
    {
        sentenceTyped = false;
        do
        {
            yield return null;
        } while (!sentenceTyped);
        //Descale parent
        target.transform.parent.transform.DOScale(0, popupScaleDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            Speaker currentSpeaker = speakers.Dequeue();
            currentSpeakerTest = currentSpeaker;
            TMP_Text nextTarget = DecideNextSpeaker(currentSpeaker);
            DisplayNextSentence(nextTarget);
        });
    }

    IEnumerator TypeSentence(string _sentence, TMP_Text target)
    {
        
        target.text = "";
        foreach (char letter in _sentence.ToCharArray())
        {
            target.text += letter;
            //Play Letter Sound
            for (int i = 0; i < letterWaitFrames; i++)
            {
                yield return null;
            }
        }
        yield return new WaitForSeconds(waitTime);
        sentenceTyped = true;
        
    }

    void EndComment(TMP_Text target)
    {
        target.text = "";
        target.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
        CloseBubble(lastSpeaker);
    }

    public void CutComment()
    {
        sentences.Clear();
        events.Clear();
        StopAllCoroutines();
        //textDisplay.text = "";
    }

    void OpenBubble(Speaker speaker)
    {
        //OpenBubble depending of speaker
    }
    void CloseBubble(Speaker speaker)
    {
        //CloseBubble depending of speaker
    }




    //David: Kike por que vas al dentista
    //Kike: []
    
}

