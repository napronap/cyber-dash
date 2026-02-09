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
}


