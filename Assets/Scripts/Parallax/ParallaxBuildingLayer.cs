using UnityEngine;

public class ParallaxBuildingsLayer : ParallaxLayerBase
{
    [Header("Building Sprites")]
    public Sprite[] buildingSprites;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float despawnXOffset = 3f;

    private float timer;

    protected override void Tick(float dt)
    {
        timer += dt;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBuilding();
        }

        MoveBuildings(dt);
    }

    void SpawnBuilding()
    {
        // TODO: add random interval, not quantity
        // if interval < elapsedTime * dt??
        // TODO: take input system back
        if (buildingSprites == null || buildingSprites.Length == 0)
        {
            Debug.LogWarning("no sprites on layer");
            return;
        }

        Sprite sprite = buildingSprites[Random.Range(0, buildingSprites.Length)];

        // create sprite
        GameObject go = new GameObject("Building", typeof(SpriteRenderer));
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;

        // spawn
        float spawnX = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x + sr.bounds.extents.x;

        // TODO: add offset?
        float spawnY = 0f;

        go.transform.position = new Vector3(spawnX, spawnY, 0);
        go.transform.SetParent(transform);
    }

    void MoveBuildings(float dt)
    {
        float move = speed * dt;

        float leftEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x - despawnXOffset;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            child.position += Vector3.left * move;

            if (child.position.x < leftEdgeX)
                Destroy(child.gameObject);
        }
    }
}
