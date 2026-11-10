using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    public DialogGraph lines;


    public DialogSegment activeSegment;
    public float textSpeed;

    private int index;
    public GameObject typingUI;


    //public PlayerController pc;

    public bool textActive = false;
    public UnityEvent EndDialogueEvent;

    public string[] dialogText;
    public GameObject questGiver;
    public bool conversation = false;
    public bool escortMission = false;
    public AudioClip buttonsound;
    public GhostScript ghost;
    public player playerController;
    public GameObject playerObject;

    public PlayerThings inputActions;

    private void Awake()
    {
        inputActions = new PlayerThings();

        playerController = GameObject.Find("Player").GetComponent<player>();
        playerObject = GameObject.Find("Player").gameObject;

        inputActions.UI.LineSkip.performed += OnSkip;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    //public NPC npc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (EndDialogueEvent == null)
        {
            EndDialogueEvent = new UnityEvent();
        }
        typingUI = FindInactiveObject("TypingUI");
    }

    GameObject FindInactiveObject(string name)
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in objects)
        {
            if (obj.name == name && obj.scene.IsValid())
            {
                return obj;
            }
        }

        return null;
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void startDialogue()
    {
        Debug.Log("Start Text");
        GhostScript.gamePaused = true;
        player.freeze = true;
        foreach (DialogSegment node in lines.nodes)
        {
            if (!node.GetInputPort("input").IsConnected)
            {
                UpdateDialog(node);
            }
        }
        if(activeSegment.speakerName == "Player"){
            TextBoxManager.Instance.nameText.text = activeSegment.speakerName;
            TextBoxManager.Instance.portrait.sprite = activeSegment.portrait;
        }
        else{
            TextBoxManager.Instance.nameText.text = ghost.ghostScriptableObject.ghostName;
            TextBoxManager.Instance.portrait.sprite = ghost.ghostScriptableObject.profileSprite;
        }
        textActive = true;

        TextBoxManager.Instance.textComponent.text = string.Empty;
        index = 0;


        StartCoroutine(TypeLine());
        if (activeSegment.speakerName != "Player") playerController.TalkingSound();
    }

    IEnumerator TypeLine()
    {
        foreach (char c in activeSegment.DialogText[index].ToCharArray())
        {
            TextBoxManager.Instance.textComponent.text += c;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
    }

    public void LineSkip()
    {

        if (TextBoxManager.Instance.textComponent.text == activeSegment.DialogText[index])
        {
            if (index < activeSegment.DialogText.Length - 1)
            {
                NextLine();
            }
            else
            {
                NextNode();
            }
        }
        else
        {
            StopAllCoroutines();
            TextBoxManager.Instance.textComponent.text = activeSegment.DialogText[index];
        }
    }

    public void OnSkip(CallbackContext ctx)
    {
        if (textActive == true)
        {
            Debug.Log("should skip line");
            playerController.buttonBufferPress = true;
            LineSkip();
        }
    }

    public void NextLine()
    {
        index++;
        TextBoxManager.Instance.textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
        if (activeSegment.speakerName != "Player") playerController.TalkingSound();;
    }

    public void NextNode()
    {
        if (activeSegment is DialogAnswerSegments)
        {

            if ((activeSegment as DialogAnswerSegments).Answers.Count > 0)
            {
                int answerIndex = 0;
                foreach (Transform child in TextBoxManager.Instance.buttonParent)
                {
                    Destroy(child.gameObject);
                }

                foreach (string answer in (activeSegment as DialogAnswerSegments).Answers)
                {
                    Debug.Log("should instantiate buttons");
                    GameObject btn = Instantiate(TextBoxManager.Instance.buttonPrefab, TextBoxManager.Instance.buttonParent);
                    btn.GetComponentInChildren<TMP_Text>().text = answer;

                    int index = answerIndex;
                    if (index == 0)
                    {
                        EventSystem.current.SetSelectedGameObject(btn);
                    }

                    btn.GetComponentInChildren<Button>().onClick.AddListener((() => { AnswerClicked(index); }));
                    btn.GetComponentInChildren<Button>().onClick.AddListener((() => { playerObject.GetComponent<AudioSource>().PlayOneShot(buttonsound); }));

                    answerIndex++;
                }
            }
            else
            {
                if (activeSegment.GetPort("output").IsConnected)
                {
                    UpdateDialog(activeSegment.GetPort("output").Connection.node as DialogSegment);
                    TextBoxManager.Instance.textComponent.text = string.Empty;
                    StartCoroutine(TypeLine());
                    if (activeSegment.speakerName != "Player") playerController.TalkingSound();;
                }
                else
                {
                    Debug.Log("no output detected");
                    EndDialogue();
                }
            }

        }
        else if (activeSegment is QuestGiverSegment)
        {
            if ((activeSegment as QuestGiverSegment).quest != null)
            {
                questGiver.GetComponent<QuestGiver>().GiveQuest((activeSegment as QuestGiverSegment).quest);
                Debug.Log("gave questid" + (activeSegment as QuestGiverSegment).quest);
                if ((activeSegment as QuestGiverSegment).conversation)
                {
                    conversation = true;
                }
                if ((activeSegment as QuestGiverSegment).escort)
                {
                    escortMission = true;
                }
                if (ghost != null)
                {
                    ghost.customerTimer += 3;
                }
            }


            if (activeSegment.GetPort("output").IsConnected)
            {
                Debug.Log("new segment");
                UpdateDialog(activeSegment.GetPort("output").Connection.node as DialogSegment);
                TextBoxManager.Instance.textComponent.text = string.Empty;
                StartCoroutine(TypeLine());
                if (activeSegment.speakerName != "Player") playerController.TalkingSound();;
            }
            else
            {
                Debug.Log("no output detected");
                EndDialogue();
            }


        }

        else if (activeSegment is DialogFunctionCallSegment functionSegment)
        {
            ExecuteAction(functionSegment.action);
            FollowOutput();
        }

        else
        {
            if (activeSegment.GetPort("output").IsConnected)
            {
                UpdateDialog(activeSegment.GetPort("output").Connection.node as DialogSegment);
                TextBoxManager.Instance.textComponent.text = string.Empty;
                StartCoroutine(TypeLine());
                if (activeSegment.speakerName != "Player") playerController.TalkingSound();;
            }
            else
            {
                EndDialogue();
            }

        }
    }

    public void AnswerClicked(int clickedIndex)
    {
        XNode.NodePort port = activeSegment.GetPort("Answers " + clickedIndex);
        if (port.IsConnected)
        {
            UpdateDialog(port.Connection.node as DialogSegment);
            if (activeSegment.speakerName != "Player") playerController.TalkingSound();
            LineSkip();
        }

        else
        {
            EndDialogue();
        }

    }

    public void EndDialogue()
    {
        Debug.Log("End here");
        foreach (Transform child in TextBoxManager.Instance.buttonParent)
        {
            Destroy(child.gameObject);
        }
        textActive = false;
        TextBoxManager.Instance.nameTextObj.SetActive(false);
        TextBoxManager.Instance.Objportrait.SetActive(false);
        TextBoxManager.Instance.textComponent.text = string.Empty;
        TextBoxManager.Instance.DialogPanel.SetActive(false);
        TextBoxManager.Instance.NoTalk = false;
        player.freeze = false;
        GhostScript.gamePaused = false;
        if(conversation)
            StartConversation(ghost);
        if (escortMission)
            StartEscort(ghost);
        playerController.buttonBufferPress = true;
        
        EndDialogueEvent.Invoke();
    }

    private void UpdateDialog(DialogSegment newSegment)
    {
        index = 0;
        activeSegment = newSegment;
        dialogText = newSegment.DialogText;
        if(activeSegment.speakerName == "Player"){
            TextBoxManager.Instance.nameText.text = activeSegment.speakerName;
            TextBoxManager.Instance.portrait.sprite = activeSegment.portrait;
        }
        else{
            TextBoxManager.Instance.nameText.text = ghost.ghostScriptableObject.ghostName;
            TextBoxManager.Instance.portrait.sprite = ghost.ghostScriptableObject.profileSprite;
        }
        foreach (Transform child in TextBoxManager.Instance.buttonParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetDialogGraph(DialogGraph graph)
    {
        lines = graph;
    }
    public void StartConversation(GhostScript ghost)
    {
        Debug.Log("shouild start conversation");
        if (conversation)
        {
            Debug.Log("conversation is true");
            typingUI.SetActive(true);
            typingUI.GetComponent<TypingGame>().StartGame(ghost);
            conversation = false;
            //GhostScript.gamePaused = true;
        }
    }
    public void StartEscort(GhostScript ghost)
    {
        ghost.assignedEscort(ChairManager.Instance.chairs[ChairManager.Instance.escortChair].gameObject.transform.position);
        escortMission = false;
    }
    public void SetGhost(GhostScript ghosty)
    {
        ghost = ghosty;
    }


    private void ExecuteAction(DialogueAction action)
    {
        if (action == null)
        {
            Debug.LogWarning("No dialogue action assigned.", this);
            return;
        }

        foreach (ActionBinding binding in actions)
        {
            if (binding != null && binding.action == action)
            {
                binding.onInvoke?.Invoke();
                return;
            }
        }

        Debug.LogWarning(
            $"No event registered for action '{action.name}'.", this);
    }
    private void FollowOutput()
    {
        var output = activeSegment.GetPort("output");

        if (output != null && output.IsConnected)
        {
            DialogSegment nextSegment =
                output.Connection.node as DialogSegment;

            if (nextSegment == null)
            {
                Debug.LogError("Connected node is not a DialogSegment.");
                EndDialogue();
                return;
            }

            UpdateDialog(nextSegment);

            TextBoxManager.Instance.textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
            if (activeSegment.speakerName != "Player") playerController.TalkingSound();;
        }
        else
        {
            EndDialogue();
        }
    }

    [System.Serializable]
    private class ActionBinding
    {
        public DialogueAction action;
        public UnityEvent onInvoke = new UnityEvent();
    }

    [SerializeField]
    private List<ActionBinding> actions = new List<ActionBinding>();

    public void RegisterAction(DialogueAction action, UnityAction callback)
    {
        if (action == null || callback == null)
            return;

        ActionBinding binding = actions.Find(x => x != null && x.action == action);

        if (binding == null)
        {
            binding = new ActionBinding { action = action };
            actions.Add(binding);
        }

        binding.onInvoke.AddListener(callback);
    }

}
