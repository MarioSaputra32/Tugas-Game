using UnityEngine;

public class ArcherAI : MonoBehaviour, IDamageable
{
    private Transform playerTransform;
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Combat")]
    public int health = 20;
    public float shootRange = 8f;
    public float attackCooldown = 2f;

    [Header("Arrow")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowSpeed = 10f;

    private bool isFacingRight = false;
    private bool isDead = false;
    private bool firstAttack = true;

    private float nextAttackTime;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        FindPlayer();
    }

    void Update()
    {
        if (isDead)
            return;

        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        // Jalankan fungsi logika pertempuran setiap frame
        Combat();
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerTransform = player.transform;
    }

    void Combat()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // Archer selalu diam
        rb.linearVelocity = Vector2.zero;

        // Player di luar jangkauan tembak
        if (distance > shootRange)
        {
            firstAttack = true;
            return;
        }

        // Menghadap ke arah player
        FacePlayer();

        // Cek apakah saat ini sedang memutar animasi Attack atau Death
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Attack") || state.IsName("Death"))
        {
            // Jangan mengeksekusi trigger serangan baru sampai animasi selesai
            return; 
        }

        // Menyerang menggunakan kalkulasi cooldown waktu yang benar
        if (firstAttack)
        {
            Attack();
            firstAttack = false;
            nextAttackTime = Time.time + attackCooldown;
        }
        else if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        if (isDead)
            return;

        anim.SetTrigger("AttackTrigger");
    }

    // Dipanggil melalui Animation Event di Unity
    public void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null)
            return;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

        if (arrowRb != null)
        {
            float dir = isFacingRight ? 1f : -1f;
            arrowRb.linearVelocity = new Vector2(dir * arrowSpeed, 0);
        }

        // Membalik arah grafik panah menyesuaikan arah hadap Archer
        Vector3 scale = arrow.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1 : -1);
        arrow.transform.localScale = scale;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        health -= damage;
        Debug.Log("Archer terkena damage. HP : " + health);

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        anim.ResetTrigger("AttackTrigger");
        anim.SetTrigger("DeathTrigger");

        Destroy(gameObject, 1.5f);
    }

    void FacePlayer()
    {
        if (playerTransform == null)
            return;

        if (playerTransform.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (playerTransform.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}