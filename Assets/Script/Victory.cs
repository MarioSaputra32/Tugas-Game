using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    public GameObject victoryPanel;
    
    // Tentukan level saat ini di Inspector Unity (misal: Level 1 tulis 1, Level 2 tulis 2)
    public int currentLevel; 

    private void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;

        // PANGGIL FUNGSI SAVE SAAT MENANG
        SaveProgress();
    }

    private void SaveProgress()
    {
        int nextLevel = currentLevel + 1;
        int highestUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Hanya update jika level berikutnya lebih tinggi dari yang sudah pernah terbuka
        if (nextLevel > highestUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save(); // Memastikan data tersimpan
            Debug.Log("Progress disimpan! Level berikutnya yang terbuka: " + nextLevel);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene("Home"); 
}

    public void NextLevel(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}