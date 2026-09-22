using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class Typer : MonoBehaviour
{
    public GameObject typingPanel;
    public TMP_Text wordOutput;
    public WordBank wordBank;

    private string remainingWord = string.Empty;
    private string currentWord = "testing a sentence in the typing game";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SetCurrentWord();
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
                typingPanel.SetActive(false);
                Time.timeScale = 1f;
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
    public void StartGame()
    {
        SetCurrentWord();
        typingPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
