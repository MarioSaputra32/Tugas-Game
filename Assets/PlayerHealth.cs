using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthSlider;

    private Defeat defeatManager;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        defeatManager = FindFirstObjectByType<Defeat>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.minValue = 0;
            healthSlider.value = currentHealth;
        }

        if (defeatManager == null)
        {
            Debug.LogError("DefeatManager tidak ditemukan!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("PLAYER MATI");

        var controller = GetComponent<TealFalconEnemySeries.DarkKnightController>();

        if (controller != null)
        {
            controller.ActivateDeath();
        }

        // Langsung tampilkan panel Defeat
        if (defeatManager != null)
        {
            defeatManager.ShowDefeat();
        }
        else
        {
            Debug.LogError("DefeatManager NULL");
        }
    }
}