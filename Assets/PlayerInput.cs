using UnityEngine;
using TealFalconEnemySeries;

public class PlayerInput : MonoBehaviour
{
    public DarkKnightController darkKnightController;
    public float jumpForce = 5f;
    public float walkSpeed = 3f;
    public float runSpeed = 7f;

    private Rigidbody2D rb;

    void Start()
    {
        if (darkKnightController != null)
        {
            rb = darkKnightController.GetComponent<Rigidbody2D>();
        }

        Debug.Log("PlayerInput Initialized");
        Debug.Log("DarkKnightController = " + darkKnightController);
        Debug.Log("Rigidbody2D = " + rb);
    }

    void Update()
    {
        if (darkKnightController == null || rb == null)
            return;

        if (darkKnightController.CurrentFightingState ==
            DarkKnightController.FightingState.Attacking)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (darkKnightController.CurrentFightingState !=
                DarkKnightController.FightingState.OnGuard)
            {
                darkKnightController.ActivateGuard();
            }

            darkKnightController.ActivateAttack();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            {
                rb.linearVelocity =
                    new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        float moveInput = Input.GetAxisRaw("Horizontal");

        Debug.Log("Horizontal Input = " + moveInput);

        float speed = Input.GetKey(KeyCode.LeftShift)
            ? runSpeed
            : walkSpeed;

        rb.linearVelocity =
            new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (moveInput != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                darkKnightController.ActivateRun();
            }
            else
            {
                darkKnightController.ActivateWalk();
            }

            Debug.Log(
                "FightState = " + darkKnightController.CurrentFightingState +
                " | MoveState = " + darkKnightController.CurrentMovementState +
                " | Velocity = " + rb.linearVelocity
            );

            float currentScaleX =
                darkKnightController.transform.localScale.x;

            if ((moveInput > 0 && currentScaleX < 0) ||
                (moveInput < 0 && currentScaleX > 0))
            {
                darkKnightController.Flip();
            }
        }
        else
        {
            darkKnightController.ActivateIdle();
        }
    }
}