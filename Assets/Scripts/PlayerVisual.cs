using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private const string IS_RUNNING = "IsRunning";
    private const string IS_JUMPING = "IsJumping";
    private const string IS_DASHING = "IsDashing";
    private const string IS_DIE = "IsDie";
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, PlayerController.Instance.IsRunning());
        AdjustPlayerFacingDirection();

    }
    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos=GameInput.Instance.GetMousePosition();
        Vector3 playerPosition=PlayerController.Instance.GetPlayerScreenPosition();
        if (mousePos.x < playerPosition.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

    }

}
