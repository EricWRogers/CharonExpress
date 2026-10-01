using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class TypingGame : MonoBehaviour
{
    public GameObject typingPanel;
    public TMP_Text wordOutput;
    public WordBank wordBank;
    public GameObject player;
    player playerController;
    public string gameID = "TypingGame";

    private string remainingWord = string.Empty;
    public string currentWord = "testing a sentence in the typing game";
    public WordObject currentWordObject = null;
    public List<string> letters = null;
    public List<string> copiedLetters;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerController = player.GetComponent<player>();
    }
    void Start()
    {
        targetPosition = pointB.position;
        SetCurrentWord();
        letterText.text = copiedLetters[letterIndex];
        letterIndex++;
    }
    private void SetCurrentWord()
    {
        //Get bank word
        currentWordObject = wordBank.GetWord();
        currentWord = currentWordObject.word;
        letters = currentWordObject.letters;
        Shuffle(letters, copiedLetters);
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
            if (letterIndex < copiedLetters.Count())
            {
                letterText.text = copiedLetters[letterIndex];
                letterIndex++;
                if (letterIndex >= copiedLetters.Count())
                {
                    letterIndex = 0;
                }
            }
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
            targetPosition = pointA.position;
            direction = -1f;
            if (letterIndex < copiedLetters.Count())
            {
                letterText.text = copiedLetters[letterIndex];
                letterIndex++;
                if (letterIndex >= copiedLetters.Count())
                {
                    letterIndex = 0;
                }
            }
        }
    }

    void CheckSuccess()
    {
        // Check if the pointer is within the safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
        {
            Debug.Log("Success!");
            Debug.Log(copiedLetters[letterIndex-1]);
            EnterLetter(copiedLetters[letterIndex-1]);
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

    private void Shuffle(List<string> strings, List<string> tempwords)
    {
        List<string> temp = new List<string>();
        temp.AddRange(strings);

        for (int i = 0; i < strings.Count; i++)
        {
            int index = Random.Range(0, temp.Count - 1);
            tempwords.Add(temp[index]);
            temp.RemoveAt(index);
        }
    }
}
