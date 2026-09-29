using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class TypingGame : MonoBehaviour
{
    public GameObject typingPanel;
    public TMP_Text wordOutput;
    public WordBank wordBank;
    public GameObject player;
    player playerController;
    public string gameID = "TypingGame";

    private string remainingWord = string.Empty;
    private string currentWord = "testing a sentence in the typing game";
    private WordObject currentWordObject = null;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerController = player.GetComponent<player>();
    }
    void Start()
    {
        targetPosition = pointB.position;
        
    }
    private void SetCurrentWord()
    {
        //Get bank word
        currentWordObject = wordBank.GetWord();
        currentWord = currentWordObject.word;
        letters = currentWordObject.letters;
        SetRemainingWord(currentWord);
    }

    private void SetRemainingWord(string newWord)
    {
        remainingWord = newWord;
        wordOutput.text = remainingWord;
    }

    // Update is called once per frame
    void Update()
    {
        pointerMovement();

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CheckSuccess();
        }
    }

    void pointerMovement()
    {
        // Move the pointer towards the target position
        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Change direction if the pointer reaches one of the points
        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
        {
            targetPosition = pointB.position;
            direction = 1f;
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
            targetPosition = pointA.position;
            direction = -1f;
        }
    }

    void CheckSuccess()
    {
        // Check if the pointer is within the safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
        {
            Debug.Log("Success!");
            
        }
        else
        {
            Debug.Log("Wrong");
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
    }

    private bool IsCorrectLeetter(string letter)
    {
        return remainingWord.IndexOf(letter) == 0;
    }

    private void RenoveLetter()
    {
        string newWord = remainingWord.Remove(0, 1);
        SetRemainingWord(newWord);
    }

    private bool WordComplete()
    {
        return remainingWord.Length == 0;
    }

    void WinGame()
    {
        typingPanel.SetActive(false);
        playerController.freeze = false;
        GhostScript.gamePaused = false;
        ghost.customerTimer += 10;
        ghost.maxCustomerTimer = ghost.customerTimer;

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
    }
    public void StartGame(GhostScript ghosts)
    {
        SetCurrentWord();
        playerController.freeze = true;
        ghost = ghosts;
        GhostScript.gamePaused = true;
    }
}
