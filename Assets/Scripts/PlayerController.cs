using JetBrains.Annotations;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] int dashSpeed = 4;
    [SerializeField] float dashTime = 0.2f;
    private Rigidbody2D rb;
    private float minMovingSpeed = 0.1f;
    private bool isRunning = false;
    private float _initialMovingSpeed;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        _initialMovingSpeed = movingSpeed;
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;
    }

    private void GameInput_OnPlayerDash(object sender, System.EventArgs e)
    {
        Dash();
    }
    private void Dash()
    {
       StartCoroutine(DashRoutine());

    }
    private IEnumerator DashRoutine()
    {
        movingSpeed *= dashSpeed;
        yield return new WaitForSeconds(dashTime);
        movingSpeed = _initialMovingSpeed;

    }

    void FixedUpdate()
    {
        HandleMovement();

    }
    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        inputVector = inputVector.normalized;
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        if ((Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;

        }


    }
    public bool IsRunning()
    {
        return isRunning;

    }
    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition= Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }
}