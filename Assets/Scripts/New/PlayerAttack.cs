using UnityEngine;

public class PlayerAttackAim : MonoBehaviour
{
    [SerializeField] private float baseAngle = 0f;
    private Collider2D attackCollider;

    private Vector3 baseLocalPos;

    private void Awake()
    {
        baseLocalPos = transform.localPosition;
        attackCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        attackCollider.enabled = WorkingPlayerController.Instance.IsAttacking();
        Vector2 input = GameInput.Instance.GetMovementVector();

        if (input.sqrMagnitude < 0.001f)
        {
            SetAngle(baseAngle);
            return;
        }

        if (input.x < 0f)
            return;

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        SetAngle(angle + baseAngle);
    }

    private void SetAngle(float angle)
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        transform.localPosition = Quaternion.Euler(0f, 0f, angle) * baseLocalPos;
    }
}
