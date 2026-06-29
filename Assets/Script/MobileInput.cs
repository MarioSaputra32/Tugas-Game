using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static float Horizontal = 0f;
    public static bool Jump = false;
    public static bool Attack = false;

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
        Jump = true;
    }

    public void AttackButton()
    {
        Attack = true;
    }
}