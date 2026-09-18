using UnityEngine;
using UnityEngine.InputSystem;

public class FallingCatchGame : MonoBehaviour
{
    public GameObject gameUI;
    public GameObject timer;
    public GameObject player;

    public RectTransform playArea;
    public RectTransform basket;
    public RectTransform goodPrefab;
    public RectTransform badPrefab;

    public GameTimer timerscript;

    public float basketSpeed = 500f;
    public float fallSpeed = 300f;
    public float spawnRate = 1f;
    public float timerTime = 10f;

    public float winPercentage = 70f;

    public string gameID = "FallingCatchGame";

    int goodCaught;
    int badCaught;
    float spawnTimer;
    bool playing;

    public void StartGame()
    {
        playing = true;
        goodCaught = 0;
        badCaught = 0;

        gameUI.SetActive(true);
        timer.SetActive(true);

        timerscript.sliderTimer = timerTime;
        timerscript.StartGameTimer();

        player.GetComponent<player>().freeze = true;
    }

    void Update()
    {
        if (!playing)
            return;

        // Move basket
        float move = 0;

        if (Keyboard.current.aKey.isPressed)
            move = -1;

        if (Keyboard.current.dKey.isPressed)
            move = 1;

        basket.anchoredPosition +=
            Vector2.right * move * basketSpeed * Time.deltaTime;

        basket.anchoredPosition = new Vector2(
            Mathf.Clamp(
                basket.anchoredPosition.x,
                -playArea.rect.width / 2 + basket.rect.width / 2,
                playArea.rect.width / 2 - basket.rect.width / 2
            ),
            basket.anchoredPosition.y
        );

        // Spawn objects
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            RectTransform prefab =
                Random.value < 0.75f ? goodPrefab : badPrefab;

            RectTransform obj = Instantiate(prefab, playArea);

            obj.anchoredPosition = new Vector2(
                Random.Range(
                    -playArea.rect.width / 2,
                    playArea.rect.width / 2
                ),
                playArea.rect.height / 2
            );

            spawnTimer = spawnRate;
        }

        // Move objects
        for (int i = playArea.childCount - 1; i >= 0; i--)
        {
            RectTransform obj = playArea.GetChild(i) as RectTransform;

            if (obj == basket)
                continue;

            obj.anchoredPosition +=
                Vector2.down * fallSpeed * Time.deltaTime;

            // Catch object
            if (Vector2.Distance(
                obj.anchoredPosition,
                basket.anchoredPosition
            ) < 80f)
            {
                if (obj.name.Contains("Good"))
                    goodCaught++;
                else
                    badCaught++;

                Destroy(obj.gameObject);
            }

            // Object missed
            else if (obj.anchoredPosition.y < -playArea.rect.height / 2)
            {
                Destroy(obj.gameObject);
            }
        }

        // Timer ended
        if (timerscript.stopTimer)
        {
            playing = false;

            int total = goodCaught + badCaught;
            float percentage = total == 0
                ? 0
                : (float)goodCaught / total * 100f;

            Debug.Log(
                "Good: " + goodCaught +
                " Bad: " + badCaught +
                " Percentage: " + percentage
            );

            gameUI.SetActive(false);
            timer.SetActive(false);
            player.GetComponent<player>().freeze = false;

            if (percentage >= winPercentage)
            {
                QuestController.Instance.CompleteMicrogame(gameID);
                Debug.Log("YOU WIN!");
            }
            else
            {
                Debug.Log("YOU LOSE!");
            }

            foreach (Transform child in playArea)
            {
                if (child != basket)
                    Destroy(child.gameObject);
            }

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
}
