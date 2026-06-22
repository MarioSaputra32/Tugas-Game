using UnityEngine;
using UnityEngine.SceneManagement; // Wajib diimpor untuk memanipulasi scene

public class MainMenuController : MonoBehaviour
{
    // Fungsi universal untuk pindah ke scene mana pun berdasarkan teks nama scene
    public void PindahKeScene(string namaScene)
    {
        SceneManager.LoadScene(namaScene);
    }

    // Fungsi untuk menutup atau keluar dari aplikasi game
    public void KeluarGame()
    {
        Debug.Log("Game Berhasil Keluar (Log aktif di Editor)");
        Application.Quit(); // Berfungsi penuh setelah game di-build (.exe / .apk)
    }
}