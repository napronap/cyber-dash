using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private const string IS_RUN = "IsRun";
    private const string IS_JUMP = "IsJump";
    private const string IS_DASH = "IsDash";
    private const string IS_DASHUP = "IsDashUp";
    private const string IS_DASHBACK = "IsDashBack";
    private const string IS_DIE = "IsDie";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        animator.SetBool(IS_RUN, PlayerController.Instance.IsRunning());
        animator.SetBool(IS_JUMP, PlayerController.Instance.IsJumping());
        animator.SetBool(IS_DASH, PlayerController.Instance.IsDashing());
        animator.SetBool(IS_DASHUP, PlayerController.Instance.IsDashUp());
        animator.SetBool(IS_DASHBACK, PlayerController.Instance.IsDashBackwards());
    }
}

