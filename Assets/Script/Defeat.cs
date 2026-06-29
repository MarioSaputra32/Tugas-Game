using UnityEngine;
using UnityEngine.SceneManagement;

public class Defeat : MonoBehaviour
{
    public GameObject defeatPanel;

    private bool isDefeat = false;

    void Start()
    {
        if (defeatPanel != null)
            defeatPanel.SetActive(false);
    }

    public void ShowDefeat()
    {
        if (isDefeat) return;

        isDefeat = true;

        defeatPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Ganti sesuai nama scene menu
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}