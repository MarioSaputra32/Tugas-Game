using UnityEngine;

public class GlobalPopupManager : MonoBehaviour
{
    // Kita buat variabel penampung tombol luar secara global di script,
    // jadi fungsi di bawahnya tidak perlu meminta banyak parameter lagi.
    public GameObject semuaTombolLuar; 

    // Sekarang fungsinya hanya meminta 1 parameter (Pasti Terbaca!)
    public void OpenPopup(GameObject targetPopup)
    {
        if (targetPopup != null) targetPopup.SetActive(true);
        if (semuaTombolLuar != null) semuaTombolLuar.SetActive(false); // Otomatis mati
    }

    public void ClosePopup(GameObject targetPopup)
    {
        if (targetPopup != null) targetPopup.SetActive(false);
        if (semuaTombolLuar != null) semuaTombolLuar.SetActive(true); // Otomatis muncul lagi
    }

    public void ExitApp()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}