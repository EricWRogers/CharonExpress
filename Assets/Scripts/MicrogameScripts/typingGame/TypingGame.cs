using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        playerController = player.GetComponent<player>();
    }
    private void SetCurrentWord()
    {
        //Get bank word
        currentWord = wordBank.GetWord();
        SetRemainingWord(currentWord);
    }

    private void SetRemainingWord(string newWord)
    {
        remainingWord = newWord;
        wordOutput.text = remainingWord;
    }

    // Update is called once per frame
    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.anyKeyDown)
        {
            string keysPressed = Input.inputString;

            if (keysPressed.Length == 1)
            {
                EnterLetter(keysPressed);
            }
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
        Time.timeScale = 1f;

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
    public void StartGame()
    {
        SetCurrentWord();
        playerController.freeze = true;
        Time.timeScale = 0f;
    }
}
