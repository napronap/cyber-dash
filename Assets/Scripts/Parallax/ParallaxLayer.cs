using UnityEngine;
using UnityEngine.UIElements;

public class ParallaxLayer : MonoBehaviour
{
    public float speed = 1f;
    public bool isActive = true;
    public float resetPositionX = -20f;
    public float startPositionX = 20f;

    private float[] lengths;

    private Transform[] sprites;

    void Start()
    {
        sprites = GetComponentsInChildren<Transform>();
        var tiles = new Transform[transform.childCount];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = transform.GetChild(i);
        }

        var width = tiles[0].GetComponent<SpriteRenderer>().bounds.size.x;

        for (int i = 0; i < tiles.Length; i++)
        {
            Vector3 prevPos = tiles[i - 1].position;
            tiles[i].position = new Vector3(prevPos.x + width, prevPos.y, prevPos.z);
        }

        // for (int i = 0; i < sprites.Length; i++)
        // {
        //     lengths[i] = sprites[i].GetComponent<SpriteRenderer>().bounds.size.x;

        // }
    }

    void Update()
    {
        if (!isActive) return;

        foreach (var sprite in sprites)
        {
            if (sprite == transform) continue;

            sprite.position += speed * Time.deltaTime * Vector3.left;

            if (sprite.position.x <= resetPositionX)
            {
                float width = sprite.GetComponent<SpriteRenderer>().bounds.size.x;
                sprite.position = new Vector3(startPositionX + width, sprite.position.y, sprite.position.z);
            }
        }        
    }
}
