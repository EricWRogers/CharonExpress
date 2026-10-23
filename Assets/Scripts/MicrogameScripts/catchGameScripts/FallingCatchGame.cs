using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class FallingCatchGame : MonoBehaviour
{
    public GameObject gameUI;


    public RectTransform playArea;
    public RectTransform basket;

    public FallingObjectSpawner spawner;

    public float basketSpeed = 500f;
    //public float winPercentage = 70f;
    public int winAmount = 5;

    public MicrogameId gameID = MicrogameId.FallingCatchGame;
    int goodCaught;
    bool playing;
    public PlayerThings inputActions;
    public float x;

    player playerController;

    void Awake()
    {
        inputActions = new PlayerThings();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        Collider2D collider = basket.GetComponent<Collider2D>();

        if (collider != null)
            collider.isTrigger = true;

        Rigidbody2D body = basket.GetComponent<Rigidbody2D>();

        if (body == null)
            body = basket.gameObject.AddComponent<Rigidbody2D>();

        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void StartGame()
    {
        playing = true;
        goodCaught = 0;
        //badCaught = 0;

        gameUI.SetActive(true);

        player.freeze = true;
        GhostScript.gamePaused = true;

        basket.anchoredPosition = new Vector2(0, basket.anchoredPosition.y);

        spawner.StartSpawning(this);
    }

    void Update()
    {
        if (!playing)
            return;

        MoveBasket();
    }

    void MoveBasket()
    {
        float move = 0;

        if (x < 0)
        {
            move = -1;
        }
        else if (x > 0)
        {
            move = 1;
        }
        else
        {
            move = 0;
        }

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
            //badCaught++;
            EndGame();

        if (goodCaught >= winAmount)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        playing = false;
        spawner.StopSpawning();
        /*
        int total = goodCaught + badCaught;

        
        float percentage = total == 0
            ? 0
            : (float)goodCaught / total * 100f;

        Debug.Log(
            $"Good: {goodCaught} Bad: {badCaught} Percentage: {percentage}"
        ); 
        */

        gameUI.SetActive(false);

        player.freeze = false;
        GhostScript.gamePaused = false;


        /*
        if (percentage >= winPercentage)
        {
            QuestController.Instance.CompleteMicrogame(gameID);
            Debug.Log("YOU WIN!");
        }
        else
        {
            Debug.Log("YOU LOSE!");
        }
        */

        if (goodCaught >= winAmount)
        {
            QuestController.Instance.CompleteMicrogame(gameID);
        }
        else
        {
            QuestController.Instance.FailQuest(gameID);
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
    
    public void OnMove(CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        x = input.x;
    }
}