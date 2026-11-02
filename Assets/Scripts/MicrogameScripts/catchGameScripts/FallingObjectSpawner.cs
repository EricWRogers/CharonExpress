using UnityEngine;

public class FallingObjectSpawner : MonoBehaviour
{
    public RectTransform playArea;
    public RectTransform goodPrefab;
    public RectTransform badPrefab;

    [Header("Spawn Settings")]
    public float spawnRate = 1f;

    [Range(0f, 100f)]
    public float sameTypeChance = 15f;

    public float fallSpeed = 300f;

    float spawnTimer;
    bool spawning;

    bool lastWasGood;
    bool hasSpawned;
    int consecutiveCount;

    FallingCatchGame game;

    public void StartSpawning(FallingCatchGame game)
    {
        this.game = game;

        spawnTimer = 0;
        spawning = true;

        hasSpawned = false;
        consecutiveCount = 0;
    }

    public void StopSpawning()
    {
        spawning = false;
    }

    void Update()
    {
        if (!spawning)
            return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer > 0)
            return;

        SpawnObject();
        spawnTimer = spawnRate;
    }

    void SpawnObject()
    {
        bool good;

        if (!hasSpawned)
        {
            good = Random.value < 0.5f;
        }
        else if (consecutiveCount >= 2)
        {
            good = !lastWasGood;
        }
        else if (Random.value * 100f < sameTypeChance)
        {
            good = lastWasGood;
        }
        else
        {
            good = !lastWasGood;
        }

        if (hasSpawned && good == lastWasGood)
            consecutiveCount++;
        else
            consecutiveCount = 1;

        lastWasGood = good;
        hasSpawned = true;

        RectTransform prefab = good ? goodPrefab : badPrefab;

        if (prefab == null)
        {
            Debug.LogError("Missing falling object prefab.", this);
            return;
        }

        RectTransform objRect = Instantiate(prefab, playArea);
        FallingObject obj = objRect.GetComponent<FallingObject>();

        obj.Setup(
            good,
            game,
            playArea,
            fallSpeed
        );

        Vector2 size = new Vector2(
            objRect.rect.width * Mathf.Abs(objRect.localScale.x),
            objRect.rect.height * Mathf.Abs(objRect.localScale.y)
        );

        float minX = playArea.rect.xMin + size.x * objRect.pivot.x;
        float maxX = playArea.rect.xMax - size.x * (1f - objRect.pivot.x);

        float spawnY = playArea.rect.yMax + size.y * (1f - objRect.pivot.y);

        objRect.localPosition = new Vector3(
            Random.Range(minX, maxX),
            spawnY,
            objRect.localPosition.z
        );

        Physics2D.SyncTransforms();
    }

    public void ClearObjects()
    {
        FallingObject[] objects =
            playArea.GetComponentsInChildren<FallingObject>();

        foreach (FallingObject obj in objects)
            Destroy(obj.gameObject);
    }
}