using UnityEngine;

public class GlobalPopupManager : MonoBehaviour
{
    public GameObject semuaTombolLuar; 
    public GameObject panelSelectLevel; // <-- TAMBAHKAN INI: Tarik objek 'SelectLevel' dari Hierarchy ke sini

    void Start()
    {
        // Cek apakah ada perintah dari scene Victory untuk langsung membuka select level
        if (PlayerPrefs.GetInt("OpenSelectLevel", 0) == 1)
        {
            // Buka panel select level
            OpenPopup(panelSelectLevel);

            // Reset kembali penandanya menjadi 0 agar saat game pertama kali dibuka tidak langsung lompat ke sini
            PlayerPrefs.SetInt("OpenSelectLevel", 0);
            PlayerPrefs.Save();
        }
    }

    public void OpenPopup(GameObject targetPopup)
    {
        if (targetPopup != null) targetPopup.SetActive(true);
        if (semuaTombolLuar != null) semuaTombolLuar.SetActive(false); 
    }

    public void ClosePopup(GameObject targetPopup)
    {
        if (targetPopup != null) targetPopup.SetActive(false);
        if (semuaTombolLuar != null) semuaTombolLuar.SetActive(true); 
    }

    public void ExitApp()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}