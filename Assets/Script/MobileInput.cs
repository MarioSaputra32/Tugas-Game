using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static float Horizontal = 0f;
    
    // Variabel versi lama agar PlayerInput.cs TIDAK ERROR
    public static bool Jump = false;
    public static bool Attack = false;

    private static bool jumpPressed = false;
    private static bool attackPressed = false;

    // Fungsi versi baru untuk DarkKnightController (Anti-Double Input)
    public static bool GetJumpPressed()
    {
        bool current = jumpPressed;
        jumpPressed = false; 
        return current;
    }

    public static bool GetAttackPressed()
    {
        bool current = attackPressed;
        attackPressed = false; 
        return current;
    }

    public void LeftDown()
    {
        Horizontal = -1f;
    }

    public void LeftUp()
    {
        if (Horizontal == -1f)
            Horizontal = 0f;
    }

    public void RightDown()
    {
        Horizontal = 1f;
    }

    public void RightUp()
    {
        if (Horizontal == 1f)
            Horizontal = 0f;
    }

    public void JumpButton()
    {
        jumpPressed = true;
        Jump = true; // Mengisi variabel lama
    }

    public void AttackButton()
    {
        attackPressed = true;
        Attack = true; // Mengisi variabel lama
    }

    // Mengosongkan otomatis variabel lama di akhir frame agar tidak nyangkut
    private void LateUpdate()
    {
        Jump = false;
        Attack = false;
    }
}