
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class TypingGame : MonoBehaviour
{
    public GameObject typingPanel;
    public TMP_Text wordOutput;
    public WordBank wordBank;
    public Dialogue dialogue;
    public DialogInteraction text;

    public MicrogameId gameID = MicrogameId.TypingGame;
    private string remainingWord = string.Empty;
    public string currentWord = "testing a sentence in the typing game";
    public WordObject currentWordObject = null;
    public List<string> letters = null;
    public GhostScript ghost;

    [Header("quick time event stuff")]
    private float direction = 1f; // 1 for moving towards B, -1 for moving towards A
    public RectTransform pointerTransform;
    private Vector3 targetPosition;
    public RectTransform safeZone; // Reference to the safe zone RectTransform
    public float moveSpeed = 100f;
    public Transform pointA; // Reference to the starting point
    public Transform pointB;
    public TMP_Text letterText;
    public int letterIndex = 0;
    public int wordIndex = 0;
    public int loseIndex = 0;
    public bool hitLetter = false;
    public bool canHitPointer = true;
    public Image pointerImg;
    public GhostScript ghosty;
    public PlayerThings inputActions;
    public player player;
    public AudioClip goodSound;
    public AudioClip badSound;
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        inputActions = new PlayerThings();

        inputActions.UI.Submit.performed += OnClick;
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
    void Start()
    {
        targetPosition = pointB.position;
        SetCurrentWord();
        letterText.text = letters[letterIndex];
    }
    private void SetCurrentWord()
    {
        //Get bank word
        currentWordObject = wordBank.GetWord();
        currentWord = currentWordObject.word;
        letters = currentWordObject.letters;
        SetRemainingWord(currentWord);
        letterIndex = 0;
        letterText.text = letters[letterIndex];
        canHitPointer = true;
        hitLetter = false;
        loseIndex = 0;
        pointerImg.color = new Color32(0, 62, 245, 255);
    }

    private void SetRemainingWord(string newWord)
    {
        remainingWord = newWord;
        wordOutput.text = remainingWord;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Interact.interactionsLocked)
            return;

        pointerMovement();

        if (loseIndex >= 3)
        {
            LoseGame();
        }
        
    }

    void pointerMovement()
    {
        if (letterIndex > letters.Count()-1)
        {
            letterIndex = 0;
        }
        // Move the pointer towards the target position
        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Change direction if the pointer reaches one of the points
        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
        {
            targetPosition = pointB.position;
            direction = 1f;
            if (letterIndex < letters.Count() && hitLetter)
            {
                letterText.text = letters[letterIndex];
                canHitPointer = true;
                hitLetter = false;
                pointerImg.color = new Color32(0, 62, 245, 255);
                      
            }
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
            targetPosition = pointA.position;
            direction = -1f;
            if (letterIndex < letters.Count() && hitLetter)
            {
                letterText.text = letters[letterIndex];
                canHitPointer = true;
                hitLetter = false;
                pointerImg.color = new Color32(0, 62, 245, 255);
            }
        }
    }

    void CheckSuccess()
    {
        // Check if the pointer is within the safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
        {
            Debug.Log("Success!");
            audioSource.PlayOneShot(goodSound);
            Debug.Log(letters[letterIndex]);
            EnterLetter(letters[letterIndex]);
            hitLetter = true;
            canHitPointer = false;
            letterIndex++;
            pointerImg.color = new Color32(24, 41, 87, 255);
        }
        else
        {
            Debug.Log("Wrong");
            audioSource.PlayOneShot(badSound);
            loseIndex++;
        }
    }

    private void EnterLetter(string TypedLetter)
    {
        if (IsCorrectLeetter(TypedLetter))
        {
            RenoveLetter();

            if (WordComplete())
            {
                WinGame();
            }
        }
        else
        {
            loseIndex++;
        }
    }

    private bool IsCorrectLeetter(string letter)
    {
        for (int i = 0; i <= remainingWord.Length; i++)
        {
            if (remainingWord[i] == letter[0])
            {
                wordIndex = i;
                return true;
            }
        }
        return false;
    }

    private void RenoveLetter()
    {
        string newWord = remainingWord.Remove(wordIndex, 1);
        SetRemainingWord(newWord);
    }

    private bool WordComplete()
    {
        return remainingWord.Length == 0;
    }

    void WinGame()
    {
        dialogue.SetDialogGraph(currentWordObject.goodResult);
        dialogue.SetGhost(ghosty);
        typingPanel.SetActive(false);
        player.freeze = false;
        GhostScript.gamePaused = false;

        Interact.SetInteractionLock(false);

        ghosty.customerTimer += 10;
        ghosty.maxCustomerTimer = ghosty.customerTimer;

        QuestController.Instance.CompleteMicrogame(gameID);
        foreach (MicroGameStart gameStart in
            FindObjectsByType<MicroGameStart>(FindObjectsSortMode.None))
        {
            if (gameStart.gameID == gameID)
            {
                gameStart.ResetInteraction();
                break;
            }
        }
        text.StartDiologue();
    }
    public void StartGame(GhostScript ghost)
    {
        Interact.SetInteractionLock(true);

        SetCurrentWord();
        player.freeze = true;
        ghosty = ghost;
        //GhostScript.gamePaused = true;
    }

    public void LoseGame()
    {
        dialogue.SetDialogGraph(currentWordObject.badResult);
        dialogue.SetGhost(ghosty);
        typingPanel.SetActive(false);
        player.freeze = false;
        GhostScript.gamePaused = false;

        Interact.SetInteractionLock(false);

        QuestController.Instance.FailQuest(QuestController.Instance.GetNextMicrogameProgress(gameID), false);
        text.StartDiologue();
    }
    void OnClick(CallbackContext ctx)
    {
        if (!Interact.interactionsLocked)
            return;

        player.buttonBufferPress = true;

        if (canHitPointer)
        {
            CheckSuccess();
        }
    }
}
