using UnityEngine;
using TealFalconEnemySeries;

public class PlayerInput : MonoBehaviour
{
    public DarkKnightController darkKnightController;
    public float jumpForce = 5f;

    void Update()
    {
        if (darkKnightController.CurrentFightingState == DarkKnightController.FightingState.Attacking)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (darkKnightController.CurrentFightingState != DarkKnightController.FightingState.OnGuard)
            {
                darkKnightController.ActivateGuard();
            }
            darkKnightController.ActivateAttack();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rigidbody2D rb = darkKnightController.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            if (darkKnightController.CurrentFightingState == DarkKnightController.FightingState.OnGuard)
            {
                darkKnightController.ActivateIdle();
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                darkKnightController.ActivateRun();
            }
            else
            {
                darkKnightController.ActivateWalk();
            }

            if ((moveInput > 0 && transform.localScale.x < 0) ||
                (moveInput < 0 && transform.localScale.x > 0))
            {
                darkKnightController.Flip();
            }
        }
        else
        {
            if (darkKnightController.CurrentFightingState != DarkKnightController.FightingState.OnGuard)
            {
                darkKnightController.ActivateIdle();
            }
        }
    }
}