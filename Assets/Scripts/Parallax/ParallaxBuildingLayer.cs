using UnityEngine;

public class ParallaxBuildingsLayer : ParallaxLayerBase
{
    [Header("Layer Sprite")]
    public Sprite[] spriteList;

    [Header("Spawn Settings")]
    public float spawnIntervalMin = 2f;
    public float spawnIntervalMax = 30f;
    public string sortingLayer;
    public float spawnHeight = 0f;
    public float spriteScale = 1f;
    public bool shouldPrefill = true;

    private float timer;

    void Start()
    {
        if (shouldPrefill) PrefillBuildings();
    }

    protected override void Tick(float dt)
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            toggleActive();
        }

        if (isActive)
        {
            timer += dt;

            if (timer >= Random.Range(spawnIntervalMin, spawnIntervalMax))
            {
                timer = 0f;

                Sprite selectedSprite = spriteList[Random.Range(0, spriteList.Length - 1)];

                SpawnBuilding(selectedSprite);
            }

            MoveBuildings(dt);
        }
    }

    void SpawnBuilding(Sprite selectedSprite)
    {
        if (spriteList == null || spriteList.Length == 0)
        {
            Debug.LogWarning("no sprites on layer");
            return;
        }

        // create sprite
        GameObject go = new GameObject($"Building_{selectedSprite.name}", typeof(SpriteRenderer));
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = selectedSprite;
        sr.sortingLayerName = sortingLayer;
        Transform tr = go.GetComponent<Transform>();
        tr.localScale = new Vector3(spriteScale, spriteScale, spriteScale);

        // spawn
        float spawnX = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x + sr.bounds.extents.x;

        go.transform.position = new Vector3(spawnX, spawnHeight, 0);
        go.transform.SetParent(transform);
    }

    void MoveBuildings(float dt)
    {
        float move = speed * dt;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            float leftEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x - sr.bounds.extents.x;

            child.position += Vector3.left * move;

            if (child.position.x < leftEdgeX)
                Destroy(child.gameObject);
        }
    }

    void PrefillBuildings()
    {
        if (spriteList == null || spriteList.Length == 0 || Camera.main == null)
            return;

        float leftEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        float rightEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;
        float x = leftEdgeX;

        while (x < rightEdgeX)
        {
            Sprite selectedSprite = spriteList[Random.Range(0, spriteList.Length)];
            GameObject go = new GameObject($"Building_{selectedSprite.name}", typeof(SpriteRenderer));
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = selectedSprite;
            sr.sortingLayerName = sortingLayer;
            Transform tr = go.GetComponent<Transform>();
            tr.localScale = new Vector3(spriteScale, spriteScale, spriteScale);

            float halfWidth = sr.bounds.extents.x;
            x += halfWidth;
            go.transform.position = new Vector3(x, spawnHeight, 0);
            go.transform.SetParent(transform);
            x += halfWidth;
        }
    }
}
