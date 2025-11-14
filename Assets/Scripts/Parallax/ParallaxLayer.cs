using UnityEngine;
using UnityEngine.UIElements;

public class ParallaxLayer : MonoBehaviour
{
    public float speed = 2f;   // velocidad del scroll
    public bool isActive = true;

    private Transform leftTile;
    private Transform rightTile;
    private float width;

    void Start()
    {
        // asume 2 hijos exactos
        leftTile = transform.GetChild(0);
        rightTile = transform.GetChild(1);

        width = leftTile.GetComponent<SpriteRenderer>().bounds.size.x;

        // acomodar al inicio
        leftTile.localPosition = Vector3.zero;
        rightTile.localPosition = new Vector3(width, 0, 0);
    }

    void Update()
    {
        if (!isActive) return;

        float move = speed * Time.deltaTime;

        leftTile.position += Vector3.left * move;
        rightTile.position += Vector3.left * move;

        // si left salió por completo a la izquierda → mandarlo a la derecha
        if (leftTile.position.x <= -width)
        {
            leftTile.position = new Vector3(rightTile.position.x + width, leftTile.position.y, leftTile.position.z);
            SwapTiles();
        }

        // si right salió por completo → mandarlo a la derecha de left
        if (rightTile.position.x <= -width)
        {
            rightTile.position = new Vector3(leftTile.position.x + width, rightTile.position.y, rightTile.position.z);
            SwapTiles();
        }
    }

    void SwapTiles()
    {
        var temp = leftTile;
        leftTile = rightTile;
        rightTile = temp;
    }
}
