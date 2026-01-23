using UnityEngine;

public class ParallaxFullscreenLayer : ParallaxLayerBase
{
    private Transform leftTile;
    private Transform rightTile;
    private float width;

    void Start()
    {
        // only 2 children
        leftTile = transform.GetChild(0);
        rightTile = transform.GetChild(1);

        width = leftTile.GetComponent<SpriteRenderer>().bounds.size.x;

        float xOffset = 1.3f;

        leftTile.localPosition = new Vector3(-xOffset, 0.5f, 0);
        rightTile.localPosition = new Vector3(width - xOffset, 0.5f, 0);
    }

    protected override void Tick(float dt)
    {
        float move = speed * dt;

        if (Input.GetKeyDown(KeyCode.P))
        {
            toggleActive();
        }

        if (isActive)
        {
            leftTile.position += Vector3.left * move;
            rightTile.position += Vector3.left * move;

            if (leftTile.position.x <= -width)
            {
                leftTile.position = new Vector3(rightTile.position.x + width, leftTile.position.y, leftTile.position.z);
                SwapTiles();
            }

            if (rightTile.position.x <= -width)
            {
                rightTile.position = new Vector3(leftTile.position.x + width, rightTile.position.y, rightTile.position.z);
                SwapTiles();
            }
        }
    }

    void SwapTiles()
    {
        var temp = leftTile;
        leftTile = rightTile;
        rightTile = temp;
    }
}
