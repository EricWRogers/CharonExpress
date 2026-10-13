using UnityEngine;

public class FallingObjectSpawner : MonoBehaviour
{
    public RectTransform playArea;
    public RectTransform goodPrefab;
    public RectTransform badPrefab;

    [Header("Spawn Settings")]
    public float spawnRate = 1f;

    [Range(0f, 100f)]
    public float goodSpawnPercentage = 75f;

    public float fallSpeed = 300f;

    float spawnTimer;
    bool spawning;

    FallingCatchGame game;

    public void StartSpawning(FallingCatchGame game)
    {
        this.game = game;

        spawnTimer = 0;
        spawning = true;
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
        bool good = Random.value * 100f < goodSpawnPercentage;

        RectTransform prefab = good ? goodPrefab : badPrefab;

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