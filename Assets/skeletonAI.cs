using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    private Transform playerTransform;
    private Animator anim;
    private Rigidbody2D rb;
    
    [Header("Jangkauan & Kecepatan")]
    public float chaseRange = 5f;        // Jarak musuh mulai mendeteksi & mengejar player
    public float attackRange = 1.5f;     // Jarak minimal untuk menyerang player
    public float moveSpeed = 2f;         // Kecepatan jalan Skeleton
    
    [Header("Jeda & Damage Serangan")]
    public float attackCooldown = 2f;    // Jeda waktu antar tebasan (detik)
    public int damageAmount = 10;        // Jumlah pengurangan darah player
    private float nextAttackTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        // Mencari objek dengan tag "Player" secara otomatis
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Cek jika sedang memutar animasi menyerang, musuh harus fokus menyelesaikan ayunannya dan tidak bergeser/berjalan
        if (IsAttacking())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("isWalking", false);
            return;
        }

        // Hitung jarak antara Skeleton dan Player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange)
        {
            // 1. KONDISI MENYERANG: Berhenti jalan dan tebas player
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("isWalking", false); // Matikan animasi jalan sesuai gambar image_d6e001.png

            HadapPlayer(); // Pastikan selalu menghadap player saat bersiap dan menyerang

            if (Time.time >= nextAttackTime)
            {
                LakukanSerangan();
                nextAttackTime = Time.time + attackCooldown; // Setel ulang cooldown
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // 2. KONDISI MENGEJAR: Dekati posisi player
            KejarPlayer();
        }
        else
        {
            // 3. KONDISI DIAM (IDLE): Player berada di luar area deteksi
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("isWalking", false); 
        }
    }

    // Fungsi helper untuk membalik orientasi Skeleton agar menghadap player
    void HadapPlayer()
    {
        float arahX = playerTransform.position.x - transform.position.x;
        if (arahX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Hadap kanan
        }
        else if (arahX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Hadap kiri
        }
    }

    // Mengecek apakah Skeleton sedang dalam status memutar animasi serangan
    bool IsAttacking()
    {
        if (anim == null) return false;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Attack") || stateInfo.IsName("SpecialAttack");
    }

    void KejarPlayer()
    {
        // Cari tahu arah posisi player (kanan atau kiri)
        float arahX = playerTransform.position.x - transform.position.x;
        
        if (arahX > 0)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }
        else if (arahX < 0)
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
        }

        HadapPlayer(); // Balik badan menghadap arah player

        // Aktifkan kotak centang isWalking di Animator
        anim.SetBool("isWalking", true); 
    }

    void LakukanSerangan()
    {
        // Mengacak jenis serangan agar variatif
        int acakSerangan = Random.Range(0, 2);

        if (acakSerangan == 0)
        {
            // Memicu AttackTrigger sesuai gambar image_d6e001.png
            anim.SetTrigger("AttackTrigger"); 
            Debug.Log("Skeleton memicu animasi menyerang biasa!");
        }
        else
        {
            // Memicu SpecialATrigger (menyesuaikan typo bawaan aset di gambar)
            anim.SetTrigger("SpecialATrigger"); 
            Debug.Log("Skeleton memicu animasi jurus spesial!");
        }
    }

    // =========================================================================
    // TAMBAHAN FUNGSI BARU DI SINI:
    // Otomatis dipanggil oleh Animation Event bawaan aset saat pedang mengayun hit player
    // =========================================================================
    public void OnAttackHitEvent()
    {
        if (playerTransform == null) return;

        // Cek kembali jarak saat pedang mengenai target untuk menghindari hit "hantu" ketika player menghindar
        float jarakSekarang = Vector2.Distance(transform.position, playerTransform.position);
        
        if (jarakSekarang <= attackRange)
        {
            // Ambil komponen PlayerHealth yang terpasang pada objek Player
            PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount); // Kurangi darah player
                Debug.Log("OnAttackHitEvent: Tebasan Berhasil! Player terkena damage sebesar " + damageAmount);
            }
            else
            {
                Debug.LogWarning("OnAttackHitEvent: Objek Player ditemukan, tetapi komponen 'PlayerHealth' tidak ada!");
            }
        }
    }

    // Menampilkan radius area deteksi dan serang di Scene View Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange); // Radius kejar (Kuning)

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Radius serang (Merah)
    }
}