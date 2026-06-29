using UnityEngine;
using System.Collections;

// Menghubungkan skrip dengan IDamageable agar bisa diserang oleh Player
public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("Patrol Settings")]
    public float patrolSpeed = 2f;
    public float walkTime = 3f;          // Waktu bergerak ke satu arah sebelum balik arah
    private float patrolTimer;
    private bool isFacingRight = true;

    [Header("Combat Settings")]
    public float chaseSpeed = 4f;
    public float attackRange = 1.5f;     // Jarak minimal untuk menyerang player
    public int health = 100;
    private bool isChasing = false;      // Status apakah sedang menyerang balik

    [Header("Animation (Opsional)")]
    private Animator anim;               // Pasang jika EnemyAI ini punya komponen Animator

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Mengambil animator jika ada
        patrolTimer = walkTime;
        
        // Mencari objek player berdasarkan Tag secara otomatis
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // PERCABANGAN LOGIKA: Patroli atau Serang Balik
        if (!isChasing)
        {
            Patrol();
        }
        else
        {
            ChaseAndAttack();
        }
    }

    // --- 1. LOGIKA PATROLI (KIRI/KANAN) ---
    void Patrol()
    {
        patrolTimer -= Time.deltaTime;

        // Gerakkan musuh sesuai arah hadap
        float direction = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * patrolSpeed, rb.linearVelocity.y);

        // Update animasi jalan jika ada komponen Animator
        if (anim != null) anim.SetBool("isWalking", true);

        // Jika waktu habis, balik arah
        if (patrolTimer <= 0)
        {
            Flip();
            patrolTimer = walkTime;
        }
    }

    // --- 2. FUNGSI JIKA DISERANG (DIWAJIBKAN OLEH INTERFACE IDAMAGEABLE) ---
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " terluka! Sisa HP: " + health);

        if (health <= 0)
        {
            Die();
            return;
        }

        // SEKARANG DIA MARAH: Aktifkan mode mengejar (Chase)
        isChasing = true;

        // Putar badan secara instan menghadap player saat mendadak dipukul dari belakang
        if ((player.position.x > transform.position.x && !isFacingRight) ||
            (player.position.x < transform.position.x && isFacingRight))
        {
            Flip();
        }
    }

    // --- 3. LOGIKA MENYERANG BALIK ---
    void ChaseAndAttack()
    {
        // Hitung jarak ke player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Jika player masih di luar jarak serang, kejar player
        if (distanceToPlayer > attackRange)
        {
            float direction = player.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);

            if (anim != null) anim.SetBool("isWalking", true);

            // Pastikan arah hadap mata selalu sesuai arah lari mengejar player
            if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
            {
                Flip();
            }
        }
        else
        {
            // Jika sudah masuk radius serang, berhenti lari dan lakukan serangan
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            
            if (anim != null) anim.SetBool("isWalking", false);
            
            Attack();
        }
    }

    void Attack()
    {
        // Tempatkan trigger animasi atau logika pengurangan darah player di sini
        // Contoh: if(anim != null) anim.SetTrigger("AttackTrigger");
        Debug.Log(gameObject.name + " meluncurkan serangan balasan ke Player!");
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " Telah Kalah!");
        // Tempatkan trigger animasi mati jika ada sebelum objek dihancurkan
        Destroy(gameObject);
    }

    // Menampilkan radius area serang di Scene View biar mudah diatur
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}