using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private const string IS_JUMP = "IsJump";
    private const string IS_DASH = "IsDash";
    private const string IS_DIE = "IsDie";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        var controller = WorkingPlayerController.Instance;

        animator.SetBool(IS_JUMP, controller.IsJumping());
        animator.SetBool(IS_DASH, controller.IsDashing());
        animator.SetBool(IS_DIE, controller.IsDead());
        animator.SetInteger("DashType", (int)controller.GetDashType());

        // parpadeo rojo por invulnerabilidad
        if (controller.IsInvulnerable())
        {
            float t = Mathf.PingPong(Time.time * 10f, 1f);
            spriteRenderer.color = Color.Lerp(originalColor, Color.red, t);
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }
}
