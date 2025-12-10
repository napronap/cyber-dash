using UnityEngine;

public class ParallaxBuildingsLayer : ParallaxLayerBase
{
    [Header("Layer Sprite")]
    public Sprite[] sprite;

    [Header("Spawn Settings")]
    public float spawnIntervalMin = 2f;
    public float spawnIntervalMax = 30f;
    public string sortingLayer;
    public float spawnY = -1f;

    private float timer;

    protected override void Tick(float dt)
    {
        timer += dt;

        if (timer >= Random.Range(spawnIntervalMin, spawnIntervalMax))
        {
            timer = 0f;

            Sprite selectedSprite = sprite[Random.Range(0, sprite.Length)];

            SpawnBuilding(selectedSprite);
        }

        MoveBuildings(dt);
    }

    void SpawnBuilding(Sprite selectedSprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("no sprites on layer");
            return;
        }

        

        // create sprite
        GameObject go = new GameObject($"Building_{selectedSprite.name}", typeof(SpriteRenderer));
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = selectedSprite;
        sr.sortingLayerName = sortingLayer;

        // spawn
        float spawnX = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x + sr.bounds.extents.x;

        float height = selectedSprite.bounds.extents.y;

        go.transform.position = new Vector3(spawnX, -height , 0);
        go.transform.SetParent(transform);
    }

    void MoveBuildings(float dt)
    {
        float move = speed * dt;


        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            float leftEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x - child.GetComponent<SpriteRenderer>().sprite.bounds.extents.x;

            child.position += Vector3.left * move;

            if (child.position.x < leftEdgeX)
                Destroy(child.gameObject);
        }
    }
}
