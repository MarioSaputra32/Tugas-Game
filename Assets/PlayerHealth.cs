using UnityEngine;
using UnityEngine.UI; // WAJIB ADA: Agar Unity mengenali tipe data 'Slider'

public class PlayerHealth : MonoBehaviour
{
    [Header("Pengaturan Darah")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Komponen UI Slider")]
    // Kolom untuk memasukkan UI Slider di Inspector
    public Slider healthSlider; 

    void Start()
    {
        // Penuhi darah saat game baru dimulai
        currentHealth = maxHealth;

        // Inisialisasi nilai batas maksimal slider sesuai dengan maxHealth
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.minValue = 0f;
        }

        UpdateHealthBar();
    }

    void Update()
    {
        // =========================================================================
        // TOMBOL TES DARURAT (Hapus atau beri komentar jika game sudah jadi)
        // =========================================================================
        // Tekan tombol K di Keyboard saat Play Mode untuk mengurangi darah sebesar 10
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Tombol K ditekan: Mengurangi darah sebesar 10");
            TakeDamage(10f); 
        }

        // Tekan tombol H di Keyboard saat Play Mode untuk menambah/heal darah sebesar 10
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Tombol H ditekan: Menambah darah sebesar 10");
            Heal(10f);
        }
    }

    // Fungsi utama untuk mengurangi darah (dipanggil dari tombol tes atau script musuh)
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

    // Fungsi untuk memperbarui visual isi Slider
    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            // Slider langsung diisi dengan nilai currentHealth
            healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("Health Slider belum di-drag ke Inspector PlayerHealth!");
        }
    }

    void Die()
    {
        Debug.Log("Player telah mati!");
        
        // Memanggil fungsi mati bawaan DarkKnightController agar animasi kematian aktif
        var controller = GetComponent<TealFalconEnemySeries.DarkKnightController>();
        if (controller != null)
        {
            controller.ActivateDeath();
        }
    }
    
}