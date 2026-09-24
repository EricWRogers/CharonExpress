using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.InputSystem;

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
        if (Keyboard.current.spaceKey.wasPressedThisFrame && textActive == true)
        {
            Debug.Log("should skip line");
            LineSkip();
        }

    }

    public void startDialogue()
    {
        Time.timeScale = 0f;
        Debug.Log("Start Text");
        foreach (DialogSegment node in lines.nodes)
        {
            if (!node.GetInputPort("input").IsConnected)
            {
                UpdateDialog(node);
            }
        }
        TextBoxManager.Instance.nameText.text = activeSegment.speakerName;
        TextBoxManager.Instance.portrait.sprite = activeSegment.portrait;
        textActive = true;

        TextBoxManager.Instance.textComponent.text = string.Empty;
        index = 0;

        StartCoroutine(TypeLine());
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


    public void NextLine()
    {
        index++;
        TextBoxManager.Instance.textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
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

                    btn.GetComponentInChildren<Button>().onClick.AddListener((() => { AnswerClicked(index); }));

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
            }


            if (activeSegment.GetPort("output").IsConnected)
            {
                Debug.Log("new segment");
                UpdateDialog(activeSegment.GetPort("output").Connection.node as DialogSegment);
                TextBoxManager.Instance.textComponent.text = string.Empty;
                StartCoroutine(TypeLine());
            }
            else
            {
                Debug.Log("no output detected");
                EndDialogue();
            }


        }
        else
        {
            if (activeSegment.GetPort("output").IsConnected)
            {
                UpdateDialog(activeSegment.GetPort("output").Connection.node as DialogSegment);
                TextBoxManager.Instance.textComponent.text = string.Empty;
                StartCoroutine(TypeLine());
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
        Time.timeScale = 1f;
        EndDialogueEvent.Invoke();
    }

    private void UpdateDialog(DialogSegment newSegment)
    {
        index = 0;
        activeSegment = newSegment;
        dialogText = newSegment.DialogText;
        TextBoxManager.Instance.nameText.text = activeSegment.speakerName;
        TextBoxManager.Instance.portrait.sprite = activeSegment.portrait;
        foreach (Transform child in TextBoxManager.Instance.buttonParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetDialogGraph(DialogGraph graph)
    {
        lines = graph;
    }
    public void StartConversation()
    {
        if (conversation)
        {
            typingUI.SetActive(true);
            typingUI.GetComponent<TypingGame>().StartGame();
            conversation = false;
        }
    }
}
