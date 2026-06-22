using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 2f;         // Kecepatan jalan musuh
    public float chaseRange = 7f;        // Jarak musuh mulai mengejar Player
    public float attackRange = 1.5f;     // Jarak musuh mulai memukul Player
    public float attackCooldown = 1.5f;  // Jeda waktu antar serangan

    private Transform player;
    private Animator anim;
    private float nextAttackTime;

    void Start()
    {
        // 1. Mencari objek Player di Scene berdasarkan Tag "Player"
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 2. Mengambil komponen Animator dari tubuh musuh
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Jika Player tidak ditemukan atau sudah mati, musuh diam saja
        if (player == null) return;

        // 3. Menghitung jarak antara posisi musuh dengan posisi Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Jika berada di dalam jarak serang, musuh berhenti dan memukul
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // Jika Player berada di jarak kejar (tetapi di luar jarak serang), musuh mengejar
            ChasePlayer();
        }
        else
        {
            // Jika Player terlalu jauh, musuh kembali ke animasi Diam (Idle)
            anim.SetBool("isWalking", false);
        }
    }

    void ChasePlayer()
    {
        // Aktifkan animasi berjalan musuh
        anim.SetBool("isWalking", true);

        // Menggerakkan posisi musuh mendekati koordinat X Player
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Membalikkan arah hadap musuh (Flip) sesuai posisi Player
        if (player.position.x > transform.position.x)
            transform.localRotation = Quaternion.Euler(0, 0, 0); // Hadap kanan
        else
            transform.localRotation = Quaternion.Euler(0, 180, 0); // Hadap kiri
    }

    void AttackPlayer()
    {
        // Hentikan animasi berjalan saat memukul
        anim.SetBool("isWalking", false);

        // 4. Memicu Trigger Animasi Menyerang milik Skeleton
        anim.SetTrigger("Attack");

        // 5. Mengirim perintah kurangi darah ke script PlayerHealth
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Jalankan fungsi pengurangan darah kamu di sini, contoh:
            // playerHealth.TakeDamage(10); 
        }
    }
}