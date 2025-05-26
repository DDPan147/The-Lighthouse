using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
using EasyTextEffects;
using UnityEngine.InputSystem;

[System.Serializable]
public class MinigameComment
{
    [TextArea(3, 10)]
    public string comment;
    public float time = 5;
    public DialogueManager.Speaker speaker;
    public bool commented;
}

[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Tooltip("The amount of frames the Dialogue waits between characters")]
    [SerializeField]
    private int letterWaitFrames;

    [SerializeField]private float popupScaleDuration;
    private bool sentenceTyped;
    private bool sentenceSkipped;
    private bool sentencePass;

    private Queue<Sentence> sentences = new Queue<Sentence>();

    public enum Speaker
    {
        Abuelo,
        Luna
    }
    public enum Emotion
    {
        Default,
        Hablar1,
        Hablar2,
        Pregunta,
        Preocupado,
        Asustado

    }

    [SerializeField] private GameObject GUICommentHolder;
    [SerializeField] private TMP_Text textDisplayGUI;
    [SerializeField] private TMP_Text textDisplayAbuelo;
    [SerializeField] private TMP_Text textDisplayLuna;

    [SerializeField]private Color colorAbuelo;
    [SerializeField]private Color colorLuna;

    private SoundManager sm;
    private Player player;

    private IEnumerator activeCoroutine;
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
        sm = FindAnyObjectByType<SoundManager>();
        player = FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        CanvasLookToScreen();
    }
    public void StartComment(DialogueComment _comment)
    {
        sentences.Clear();
        player.canMove = false;
        StopAllCoroutines();

        foreach (Sentence sentence in _comment.sentences)
        {
            sentences.Enqueue(sentence);
        }

        //SPEAKERS
        Speaker currentSpeaker = sentences.Peek().speaker;
        DecideFirstSpeaker(_comment.type, currentSpeaker);
        
    }

    private void DecideFirstSpeaker(DialogueComment.DialogueTypes commentType, Speaker currentSpeaker)
    {
        if (commentType == DialogueComment.DialogueTypes.Popup)
        {
            if (currentSpeaker == Speaker.Abuelo)
            {
                DisplayNextSentence(textDisplayAbuelo);
            }
            else
            {
                DisplayNextSentence(textDisplayLuna);
            }
        }
        else
        {
            //DisplayGUIComment();
            //return textDisplayGUI;
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
    public void DisplayGUIComment(MinigameComment mg)
    {
        if(activeCoroutine != null){
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartGUIComment(mg.comment, mg.time, mg.speaker);
        StartCoroutine(activeCoroutine);
    }

    public IEnumerator StartGUIComment(string comment, float time, Speaker speaker)
    {
        GUICommentHolder.SetActive(true);
        textDisplayGUI.text = comment;
        textDisplayGUI.transform.parent.GetComponent<TMP_Text>().text = comment;
        if(speaker == Speaker.Abuelo)
        {
            textDisplayGUI.color = colorAbuelo;
        }
        else
        {
            textDisplayGUI.color = colorLuna;
        }
        yield return new WaitForSeconds(time);
        GUICommentHolder.SetActive(false);

    }
    void DisplayNextSentence(TMP_Text target)
    {
        if (sentences.Count == 0)
        {
            EndComment();
            return;
        }

        Sentence currentSentence = sentences.Dequeue();

        

        //EVENTS
        DialogueEvent newEvent = currentSentence.sentenceEvent;

        if (newEvent.WhenToPlay == DialogueEvent.PlayWhen.PlayAtStart)
        {
            StartCoroutine(DelayEvent(newEvent.timeOffset, newEvent.uEvent));
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
            StartCoroutine(TypeSentence(currentSentence, target));

            //target.parent activates
            //target starts typing text
        }
        else
        {
            target.text = currentSentence.sentenceText;
            target.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            target.transform.GetChild(1).GetComponent<TextEffect>().enabled = false;
            target.transform.parent.transform.DOScale(1, popupScaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if (currentSentence.hasAnimation)
                {
                    int num = Random.Range(0, 2);
                    player.meshAnimator.SetTrigger("Talk" + num);
                }
                
                sm.Play("Texto");
                StartCoroutine(WaitForNextSentence(target, currentSentence));
                StartCoroutine(TypeSentence(currentSentence, target.transform.GetChild(1).GetComponent<TMP_Text>()));
            });
            
            
        }

    }
    IEnumerator WaitForNextSentence(TMP_Text target, Sentence sentence)
    {
        sentenceTyped = false;
        sentenceSkipped = false;
        while (true)
        {
            if (GetPassTextCondition())
            {
                if (sentenceTyped)
                {
                    CloseBubble(target.transform.parent);
                    if (sentence.sentenceEvent.WhenToPlay == DialogueEvent.PlayWhen.PlayAfterInput)
                    {
                        StartCoroutine(DelayEvent(sentence.sentenceEvent.timeOffset, sentence.sentenceEvent.uEvent));
                    }
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

    bool GetPassTextCondition()
    {
        if (GameManager.cutsceneActive) 
        {
            bool b = sentencePass;
            sentencePass = false;
            return b;
        }
        else
        {
            return /*Input.GetKeyDown(KeyCode.Z)*/ Player.interact;
        }  
    }
    void CloseBubble(Transform target)
    {
        target.DOScale(0, popupScaleDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            TMP_Text nextTarget = null;
            if (sentences.Count > 0)
            {
                Speaker currentSpeaker = sentences.Peek().speaker;
                nextTarget = DecideNextSpeaker(currentSpeaker);
            }
            DisplayNextSentence(nextTarget);
        });
    }

    IEnumerator TypeSentence(Sentence _sentence, TMP_Text target)
    {
        
        target.text = "";
        
        bool isTag = false;
        foreach (char letter in _sentence.sentenceText.ToCharArray())
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
                    yield return new WaitForFixedUpdate();
                }
            }

            if(letter == '>') //Cuando el tag acaba, sigue escribiendo de normal
            {
                isTag = false;
            }

            if (sentenceSkipped)
            {
                target.text = _sentence.sentenceText;
                break;
            }
        }

        if (_sentence.sentenceEvent.WhenToPlay == DialogueEvent.PlayWhen.PlayAtEnd)
        {
            StartCoroutine(DelayEvent(_sentence.sentenceEvent.timeOffset, _sentence.sentenceEvent.uEvent));
        }
       target.GetComponent<TextEffect>().enabled = true;
       sm.Stop("Texto");
       sentenceTyped = true;
        
    }

    void EndComment()
    {
        player.canMove = true;
    }

    string EmotionSoundSelector(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Default:
                return null;
            case Emotion.Hablar1:
                return "AbueloHablar1";
            case Emotion.Hablar2:
                return "AbueloHablar2";
            case Emotion.Pregunta:
                return "AbueloPregunta";
            case Emotion.Preocupado:
                return "AbueloPreocupado";
            case Emotion.Asustado:
                return "AbueloAsustado";
            default:
                return null;
        }
    }
    void CanvasLookToScreen()
    {
        if(textDisplayAbuelo.transform.parent != null)
        {
            textDisplayAbuelo.transform.parent.parent.rotation = Camera.main.transform.rotation;
        }
        if(textDisplayLuna.transform.parent != null)
        {
            textDisplayLuna.transform.parent.parent.rotation = Camera.main.transform.rotation;
        }
        
    }

    IEnumerator DelayEvent(float delay, UnityEvent uEvent)
    {
        yield return new WaitForSeconds(delay);
        uEvent?.Invoke();
    }

    public void DialogueSentencePass()
    {
        sentencePass = true;
    }

    //David: Kike por que vas al dentista
    //Kike: []
    //Álvaro: *decepcionado consigo mismo*
    //Victor: Hola
    //Gurt: yo
}

