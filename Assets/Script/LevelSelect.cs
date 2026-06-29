using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public Button[] levelButtons;
    public GameObject[] lockIcons;

    void Start()
    {
        // TEST: buka sampai level 3
        int unlockedLevel = 1;

        Debug.Log("Unlocked Level = " + unlockedLevel);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool unlocked = (i + 1) <= unlockedLevel;

            // Tombol bisa diklik jika terbuka
            levelButtons[i].interactable = unlocked;

            // Hilangkan lock jika level terbuka
            if (lockIcons[i] != null)
            {
                lockIcons[i].SetActive(!unlocked);
            }
        }
    }
}