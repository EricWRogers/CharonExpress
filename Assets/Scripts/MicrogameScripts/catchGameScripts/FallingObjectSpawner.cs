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

        RectTransform prefab = good
            ? goodPrefab
            : badPrefab;

        FallingObject obj =
            Instantiate(prefab, playArea)
            .GetComponent<FallingObject>();

        obj.Setup(
            good,
            game,
            playArea,
            fallSpeed
        );

        obj.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(
                Random.Range(
                    -playArea.rect.width / 2,
                    playArea.rect.width / 2
                ),
                playArea.rect.height / 2
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