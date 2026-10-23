using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BookRequestGame : MonoBehaviour
{
    [System.Serializable]
    public class ColorData
    {
        public string colorName;
        public Color actualColor;
    }

    [System.Serializable]
    public class Book
    {
        public string subject;
        public string color;
        public Color actualColor;
    }

    [Header("Quest")]
    public MicrogameId gameID = MicrogameId.BookRequestGame;
    [Header("Subjects")]
    public string[] subjects;

    [Header("Colors")]
    public ColorData[] colors;

    [Header("Dialogue")]
    public DialogSegment requestDialogue;

    [TextArea]
    public string requestText =
        "I am looking for a {color} book about {subject}.";

    [Header("Quest Log")]
    public string questLogText =
        "Find a {color} book about {subject}";

    [Header("Game UI")]
    public GameObject gameUI;
    public TMP_Text requestDescriptionText;
    public Transform gridParent;
    public Button bookButtonPrefab;
    public int amountOfSameColorBooks = 3;
    public int amountOfSameSubjectBooks = 3;

    [Header("Grid")]
    public int columns = 5;
    public int rows = 4;

    [Header("Player")]
    public GameObject playerObj;

    private List<Book> books = new List<Book>();
    private Book requestedBook;
    private bool gameRunning;
    private Quest.QuestProgress currentProgress;

    public void PrepareRequest(Quest.QuestProgress progress)
    {
        GenerateBooks();

        if (books.Count == 0)
        {
            Debug.LogError("No books available.");
            return;
        }

        Book requestedBook = books[Random.Range(0, books.Count)];

        progress.requestedBookSubject = requestedBook.subject;
        progress.requestedBookColor = requestedBook.color;

        Debug.Log(
            "Requested book: " +
            requestedBook.color + " " + requestedBook.subject);
    }
    private void GenerateBooks()
    {
        books.Clear();

        foreach (string subject in subjects)
        {
            foreach (ColorData color in colors)
            {
                books.Add(new Book
                {
                    subject = subject,
                    color = color.colorName,
                    actualColor = color.actualColor
                });
            }
        }
    }

    public string GetRequestDescription()
    {
        if (requestedBook == null)
            return "";

        return requestText
            .Replace("{color}", requestedBook.color)
            .Replace("{subject}", requestedBook.subject);
    }

    public void StartGame()
    {
        currentProgress =
            QuestController.Instance.GetNextMicrogameProgress(gameID);

        if (currentProgress == null)
        {
            Debug.LogWarning("No active book quest is ready to play.");
            return;
        }

        requestedBook = books.Find(book =>
            book.subject == currentProgress.requestedBookSubject &&
            book.color == currentProgress.requestedBookColor);

        if (requestedBook == null)
        {
            Debug.LogError("Could not find the requested book.");
            currentProgress = null;
            return;
        }

        gameRunning = true;

        requestDescriptionText.text = GetRequestDescription();
        gameUI.SetActive(true);

        player.freeze = true;
        GhostScript.gamePaused = true;

        CreateBooks();
    }
    public void CreateBooks()
    {
        ClearBooks();

        int totalBooks = columns * rows;
        List<Book> gridBooks = new List<Book>();

        // Correct book
        gridBooks.Add(requestedBook);

        // Same color
        for (int i = 0; i < amountOfSameColorBooks; i++)
        {
            List<Book> choices = books.FindAll(book =>
                book.color == requestedBook.color &&
                book != requestedBook &&
                !gridBooks.Contains(book)
            );

            if (choices.Count == 0)
                break;

            Book book = choices[Random.Range(0, choices.Count)];
            gridBooks.Add(book);
        }

        // Same subject
        for (int i = 0; i < amountOfSameSubjectBooks; i++)
        {
            List<Book> choices = books.FindAll(book =>
                book.subject == requestedBook.subject &&
                book != requestedBook &&
                !gridBooks.Contains(book)
            );

            if (choices.Count == 0)
                break;

            Book book = choices[Random.Range(0, choices.Count)];
            gridBooks.Add(book);
        }

        // Fill the rest randomly
        while (gridBooks.Count < totalBooks)
        {
            Book book = books[Random.Range(0, books.Count)];

            if (!gridBooks.Contains(book))
                gridBooks.Add(book);
            else if (gridBooks.Count < books.Count)
                continue;
            else
                gridBooks.Add(book);
        }

        // Shuffle
        for (int i = 0; i < gridBooks.Count; i++)
        {
            int randomIndex = Random.Range(i, gridBooks.Count);

            Book temp = gridBooks[i];
            gridBooks[i] = gridBooks[randomIndex];
            gridBooks[randomIndex] = temp;
        }

        // Create books
        foreach (Book book in gridBooks)
        {
            Button button = Instantiate(
                bookButtonPrefab,
                gridParent
            );
            if (book == gridBooks[0])
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }

            button.GetComponent<Image>().color = book.actualColor;

            button.GetComponentInChildren<TMP_Text>().text =
                "<rotate=90>" + book.subject;

            if (book == requestedBook)
                button.onClick.AddListener(WinGame);
            if (book != requestedBook)
                button.onClick.AddListener(LoseGame);
        }
    }
    public void ClearBooks()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(
                gridParent.GetChild(i).gameObject
            );
        }
    }

    public void WinGame()
    {
        if (!gameRunning)
            return;

        gameRunning = false;

        ClearBooks();
        gameUI.SetActive(false);

        player.freeze = false;
        GhostScript.gamePaused = false;

        QuestController.Instance.CompleteMicrogame(currentProgress);
        Debug.Log(
            "Found book: " +
            requestedBook.color + " " + requestedBook.subject
        );
        currentProgress = null;
        requestedBook = null;

        MicroGameStart[] gameStarts =
            FindObjectsByType<MicroGameStart>(
                FindObjectsSortMode.None
            );

        foreach (MicroGameStart gameStart in gameStarts)
        {
            if (gameStart.gameID == gameID)
            {
                gameStart.ResetInteraction();
                break;
            }
        }

        requestedBook = null;
    }
    public void LoseGame()
    {
        if (!gameRunning)
            return;

        gameRunning = false;

        ClearBooks();
        gameUI.SetActive(false);

        player.freeze = false;
        GhostScript.gamePaused = false;
        QuestController.Instance.FailQuest(currentProgress);
        currentProgress = null;
        requestedBook = null;
    }
    public string GetRequestDescription(Quest.QuestProgress progress)
    {
        if (progress == null)
            return "";

        return requestText
            .Replace("{color}", progress.requestedBookColor)
            .Replace("{subject}", progress.requestedBookSubject);
    }
}