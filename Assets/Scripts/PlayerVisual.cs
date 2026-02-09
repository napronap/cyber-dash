using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private const string IS_JUMP = "IsJump";
    private const string IS_DASH = "IsDash";
    private const string IS_DIE = "IsDie";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        animator.SetBool(IS_JUMP, WorkingPlayerController.Instance.IsJumping());
        animator.SetBool(IS_DASH, WorkingPlayerController.Instance.IsDashing());
        animator.SetInteger("DashType", (int)WorkingPlayerController.Instance.GetDashType());
        // animator.SetBool(IS_DIE, WorkingPlayerController.Instance.IsDashing());
    }
}

