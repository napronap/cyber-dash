using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float dashDistance = 3f; // ÑÅÑÇÑÄÑÉÑÑÑpÑë ÑtÑyÑÉÑÑÑpÑ~ÑàÑyÑë ÑÇÑçÑrÑ{Ñp

    void Start()
    {
    }

    void Update()
    {
        // ÑÅÑÇÑÄÑÉÑÑÑÄÑz ÑÇÑçÑrÑÄÑ{: ÑÖÑtÑuÑÇÑwÑyÑrÑpÑÑÑé Shift Ñy Ñ~ÑpÑwÑpÑÑÑé ÑÉÑÑÑÇÑuÑ|Ñ{ÑÖ -> Ñ}ÑsÑ~ÑÄÑrÑuÑ~Ñ~ÑÄ ÑÉÑ}ÑuÑÉÑÑÑyÑÑÑéÑÉÑë Ñ~Ñp dashDistance
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (shift && Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.position += Vector3.up * dashDistance;
        }
        else if (shift && Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.position += Vector3.down * dashDistance;
        }
        else if (shift && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left * dashDistance;
        }
        else if (shift && Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += Vector3.right * dashDistance;
        }

        // ÑÄÑqÑçÑâÑ~ÑÄÑu ÑtÑrÑyÑwÑuÑ~ÑyÑu
        float moveAmount = speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.up * moveAmount, Space.World);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.down * moveAmount, Space.World);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * moveAmount, Space.World);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * moveAmount, Space.World);
        }
    }
}
