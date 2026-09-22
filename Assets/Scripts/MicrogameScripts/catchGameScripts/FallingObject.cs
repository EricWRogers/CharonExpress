using UnityEngine;

public class FallingObject : MonoBehaviour
{
    bool good;

    FallingCatchGame game;
    RectTransform playArea;
    RectTransform rectTransform;

    float fallSpeed;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        Collider2D collider = GetComponent<Collider2D>();

        if (collider != null)
            collider.isTrigger = true;

        Rigidbody2D body = GetComponent<Rigidbody2D>();

        if (body == null)
            body = gameObject.AddComponent<Rigidbody2D>();

        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0;
    }

    public void Setup(
        bool good,
        FallingCatchGame game,
        RectTransform playArea,
        float fallSpeed)
    {
        this.good = good;
        this.game = game;
        this.playArea = playArea;
        this.fallSpeed = fallSpeed;
    }

    void Update()
    {
        rectTransform.anchoredPosition +=
            Vector2.down * fallSpeed * Time.deltaTime;

        Physics2D.SyncTransforms();

        if (rectTransform.anchoredPosition.y <
            -playArea.rect.height / 2)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
            return;

        if (other == game.basket.GetComponent<Collider2D>())
        {
            game.ObjectCaught(good);
            Destroy(gameObject);
        }
    }
}