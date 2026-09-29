using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookRequestGame : MonoBehaviour
{
    [System.Serializable]
    public class Book
    {
        public string title;
        public string subject;
        public string color;
        public Sprite sprite;
    }

    [Header("Quest")]
    public string gameID = "BookRequestGame";

    [Header("Books")]
    public Book[] books;

    [Header("Request")]
    [TextArea]
    public string requestText =
        "I am looking for a {color} book about {subject}.";

    [Header("Game UI")]
    public GameObject gameUI;
    public TMP_Text requestDescriptionText;
    public Transform gridParent;
    public Button bookButtonPrefab;

    [Header("Grid")]
    public int columns = 5;
    public int rows = 4;

    [Header("Player")]
    public GameObject player;

    private Book requestedBook;
    private bool gameRunning;

    public void PrepareRequest()
    {
        if (books == null || books.Length < 2)
        {
            Debug.LogError("BookRequestGame needs at least 2 books.");
            return;
        }

        requestedBook = books[Random.Range(0, books.Length)];

        Debug.Log("Requested book: " + requestedBook.title);
    }

    public void StartGame()
    {
        if (requestedBook == null)
        {
            Debug.LogError("No book has been requested. Call PrepareRequest() first.");
            return;
        }

        gameRunning = true;

        requestDescriptionText.text = requestText
            .Replace("{color}", requestedBook.color)
            .Replace("{subject}", requestedBook.subject);

        gameUI.SetActive(true);

        player.GetComponent<player>().freeze = true;
        GhostScript.gamePaused = true;

        CreateBooks();
    }

    public void CreateBooks()
    {
        ClearBooks();

        int totalBooks = columns * rows;
        int correctPosition = Random.Range(0, totalBooks);

        for (int i = 0; i < totalBooks; i++)
        {
            Button button = Instantiate(bookButtonPrefab, gridParent);

            Book book;

            if (i == correctPosition)
            {
                book = requestedBook;
            }
            else
            {
                book = books[Random.Range(0, books.Length)];

                while (book == requestedBook)
                {
                    book = books[Random.Range(0, books.Length)];
                }
            }

            button.image.sprite = book.sprite;

            if (book == requestedBook)
            {
                button.onClick.AddListener(WinGame);
            }
        }
    }

    public void ClearBooks()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }
    }

    public void WinGame()
    {
        if (!gameRunning)
            return;

        gameRunning = false;

        ClearBooks();

        gameUI.SetActive(false);

        player.GetComponent<player>().freeze = false;
        GhostScript.gamePaused = false;

        QuestController.Instance.CompleteMicrogame(gameID);

        Debug.Log("Found book: " + requestedBook.title);

        MicroGameStart[] gameStarts =
            FindObjectsByType<MicroGameStart>(FindObjectsSortMode.None);

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
}