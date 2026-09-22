using UnityEngine;
using UnityEngine.InputSystem;

public class FallingCatchGame : MonoBehaviour
{
    public GameObject gameUI;
    public GameObject timer;
    public GameObject player;

    public RectTransform playArea;
    public RectTransform basket;

    public GameTimer gameTimer;
    public FallingObjectSpawner spawner;

    public float basketSpeed = 500f;
    public float timerTime = 10f;
    public float winPercentage = 70f;

    public string gameID = "FallingCatchGame";

    int goodCaught;
    int badCaught;
    bool playing;

    player playerController;

    void Awake()
    {
        playerController = player.GetComponent<player>();

        Collider2D collider = basket.GetComponent<Collider2D>();

        if (collider != null)
            collider.isTrigger = true;

        Rigidbody2D body = basket.GetComponent<Rigidbody2D>();

        if (body == null)
            body = basket.gameObject.AddComponent<Rigidbody2D>();

        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0;
    }

    public void StartGame()
    {
        playing = true;
        goodCaught = 0;
        badCaught = 0;

        gameUI.SetActive(true);
        timer.SetActive(true);

        gameTimer.sliderTimer = timerTime;
        gameTimer.StartGameTimer();

        playerController.freeze = true;

        spawner.StartSpawning(this);
    }

    void Update()
    {
        if (!playing)
            return;

        MoveBasket();

        if (gameTimer.stopTimer)
            EndGame();
    }

    void MoveBasket()
    {
        float move = 0;

        if (Keyboard.current.aKey.isPressed)
            move = -1;

        if (Keyboard.current.dKey.isPressed)
            move = 1;

        basket.anchoredPosition +=
            Vector2.right * move * basketSpeed * Time.deltaTime;

        float minX = -playArea.rect.width / 2 + basket.rect.width / 2;
        float maxX = playArea.rect.width / 2 - basket.rect.width / 2;

        basket.anchoredPosition = new Vector2(
            Mathf.Clamp(basket.anchoredPosition.x, minX, maxX),
            basket.anchoredPosition.y
        );

        Physics2D.SyncTransforms();
    }

    public void ObjectCaught(bool good)
    {
        if (good)
            goodCaught++;
        else
            badCaught++;
    }

    void EndGame()
    {
        playing = false;
        spawner.StopSpawning();

        int total = goodCaught + badCaught;

        float percentage = total == 0
            ? 0
            : (float)goodCaught / total * 100f;

        Debug.Log(
            $"Good: {goodCaught} Bad: {badCaught} Percentage: {percentage}"
        );

        gameUI.SetActive(false);
        timer.SetActive(false);

        playerController.freeze = false;

        if (percentage >= winPercentage)
        {
            QuestController.Instance.CompleteMicrogame(gameID);
            Debug.Log("YOU WIN!");
        }
        else
        {
            Debug.Log("YOU LOSE!");
        }

        spawner.ClearObjects();

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
}