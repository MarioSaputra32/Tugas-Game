using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // TAMBAHKAN INI untuk berpindah scene

public class LevelSelect : MonoBehaviour
{
    public Button[] levelButtons;
    public GameObject[] lockIcons;
    
    // Tentukan awalan nama scene Anda di Inspector, misalnya "Level_" atau "Scene_"
    // Jika nama scene Anda langsung angka (1, 2, 3), kosongkan saja tulisan ini di Inspector ("")
    public string sceneNamePrefix = "Level_"; 

    void Start()
    {
        // Mengambil level yang terbuka dari PlayerPrefs.
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        Debug.Log("Unlocked Level saat ini = " + unlockedLevel);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1; // Level 1, Level 2, dst.
            bool unlocked = levelNumber <= unlockedLevel;

            // Tombol bisa diklik jika terbuka
            levelButtons[i].interactable = unlocked;

            // Hilangkan lock jika level terbuka
            if (i < lockIcons.Length && lockIcons[i] != null)
            {
                lockIcons[i].SetActive(!unlocked);
            }

            // MODIFIKASI: Memberikan fungsi klik otomatis ke setiap tombol
            if (unlocked)
            {
                // Kita gunakan Listener agar saat diklik, fungsi LoadLevel() dijalankan
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelNumber));
            }
        }
    }

    // Fungsi untuk memuat scene berdasarkan nomor level
    void LoadLevel(int levelIndex)
    {
        // Menggabungkan prefix dan angka, misal: "Level_" + 1 menjadi "Level_1"
        string targetScene = sceneNamePrefix + levelIndex; 
        
        Debug.Log("Memuat Scene: " + targetScene);
        SceneManager.LoadScene(targetScene);
    }
}