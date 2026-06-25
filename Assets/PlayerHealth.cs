using UnityEngine;
using UnityEngine.UI; // 1. WAJIB ADA: Agar Unity mengenali tipe data 'Image'

public class PlayerHealth : MonoBehaviour
{
    [Header("Pengaturan Darah")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Komponen UI")]
    // 2. Kolom untuk memasukkan Image isi bar merah di Inspector
    public Image healthFillImage; 

    void Start()
    {
        // Penuhi darah saat game baru dimulai
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Fungsi untuk mengurangi darah (bisa dipanggil saat kena musuh)
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Cegah darah minus

        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // Fungsi untuk menambah darah (misal saat ambil potion/item)
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();
    }

    // Fungsi utama untuk menggerakkan isi bar merah secara visual
    void UpdateHealthBar()
    {
        if (healthFillImage != null)
        {
            // Mengubah nilai fillAmount (0.0 sampai 1.0) berdasarkan persentase darah
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Player telah mati!");
        // Kamu bisa memanggil fungsi mati bawaan DarkKnightController di sini:
        // GetComponent<TealFalconEnemySeries.DarkKnightController>().ActivateDeath();
    }
}