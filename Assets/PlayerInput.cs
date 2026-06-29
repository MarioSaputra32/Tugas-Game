using UnityEngine;
using TealFalconEnemySeries;

public class PlayerInput : MonoBehaviour
{
    [Header("References")]
    public DarkKnightController darkKnightController;
    private Rigidbody2D rb;
    private Collider2D col;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 7f;
    public float jumpForce = 5f; 

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackCooldown = 0.5f; 
    private float nextAttackTime = 0f; 

    [Header("Hitbox Senjata Menyatu")]
    public Vector2 attackRange = new Vector2(1.5f, 1f); // Ukuran kotak area serang (Lebar, Tinggi)
    public float attackOffsetForward = 1f; // Jarak kotak serang ke depan karakter
    public LayerMask enemyLayer; // Pilih layer "Enemy" di Inspector

    [Header("Ground Check Settings")]
    public LayerMask groundLayer; 
    public float extraCastDistance = 0.05f; 

    private bool isGrounded;

    void Start()
    {
        if (darkKnightController != null)
        {
            rb = darkKnightController.GetComponent<Rigidbody2D>();
            col = darkKnightController.GetComponent<Collider2D>();
        }

        Debug.Log("PlayerInput Initialized.");
    }

    void Update()
    {
        if (darkKnightController == null || rb == null || col == null)
            return;

        if (darkKnightController.CurrentFightingState == DarkKnightController.FightingState.Hurt ||
            darkKnightController.CurrentFightingState == DarkKnightController.FightingState.Death)
        {
            return;
        }

        // ==========================================
        // 1. DETEKSI TANAH (DIPERBAIKI)
        // ==========================================
        // Mengecilkan lebar (X) kotak deteksi sebesar 20% agar tidak tersangkut di dinding/sudut platform
        Vector2 boxSize = new Vector2(col.bounds.size.x * 0.8f, 0.05f); 
        // Menggeser titik mulai kotak tepat di batas bawah kaki collider karakter
        Vector2 boxCenter = new Vector2(col.bounds.center.x, col.bounds.min.y); 

        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, extraCastDistance, groundLayer);
        isGrounded = hit.collider != null;

        // ==========================================
        // 2. LOGIKA INPUT SERANG + DETEKSI HITBOX
        // ==========================================
        if ((Input.GetMouseButtonDown(0) || MobileInput.Attack) && Time.time >= nextAttackTime)
    {
            MobileInput.Attack = false;

            darkKnightController.ActivateAttack();
            nextAttackTime = Time.time + attackCooldown;

            CheckAttackHitbox();
    }

        // ==========================================
        // 3. LOGIKA INPUT LONCAT
        // ==========================================
      if ((Input.GetKeyDown(KeyCode.Space) || MobileInput.Jump) && isGrounded)
    {
        MobileInput.Jump = false;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

        // ==========================================
        // 4. LOGIKA INPUT GERAKAN & PENGUNCIAN DASH
        // ==========================================
        float keyboardInput = Input.GetAxisRaw("Horizontal");
        float moveInput = keyboardInput != 0 ? keyboardInput : MobileInput.Horizontal;
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        bool isAttacking = darkKnightController.CurrentFightingState == DarkKnightController.FightingState.Attacking;

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * targetSpeed, rb.linearVelocity.y);
        }

        // Mengatur State Animasi Jalan/Lari/Idle
        if (moveInput != 0 && !isAttacking)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                darkKnightController.ActivateRun();
            }
            else
            {
                darkKnightController.ActivateWalk();
            }

            float currentScaleX = darkKnightController.transform.localScale.x;
            if ((moveInput > 0 && currentScaleX < 0) || (moveInput < 0 && currentScaleX > 0))
            {
                darkKnightController.Flip();
            }
        }
        else
        {
            if (!isAttacking)
            {
                darkKnightController.ActivateIdle();
            }
        }
    }

    // Fungsi pengganti OnTriggerEnter2D jika senjata menyatu dengan badan
    void CheckAttackHitbox()
    {
        float direction = darkKnightController.movingRight ? 1f : -1f;
        Vector2 attackPosition = (Vector2)transform.position + new Vector2(attackOffsetForward * direction, 0f);
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPosition, attackRange, 0f, enemyLayer);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                // Mengambil komponen IDamageable yang diselipkan di bawah file ini
                IDamageable damageableEnemy = enemyCollider.GetComponent<IDamageable>();
                
               if (damageableEnemy != null)
                    {
                        Debug.Log("Menebas musuh: " + enemyCollider.name + " sebesar " + (int)attackDamage);
                        damageableEnemy.TakeDamage((int)attackDamage);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (darkKnightController == null) return;
        
        // 1. Gambar Hitbox Serangan (Warna Biru)
        Gizmos.color = Color.blue;
        float direction = darkKnightController.movingRight ? 1f : -1f;
        Vector3 attackPosition = transform.position + new Vector3(attackOffsetForward * direction, 0f, 0f);
        Gizmos.DrawWireCube(attackPosition, new Vector3(attackRange.x, attackRange.y, 1f));

        // 2. Gambar Hitbox Deteksi Tanah (Warna Hijau jika menapak, Merah jika melayang)
        if (col != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector2 boxSize = new Vector2(col.bounds.size.x * 0.8f, 0.05f);
            Vector3 boxCenter = new Vector3(col.bounds.center.x, col.bounds.min.y - (extraCastDistance / 2f), 0f);
            Gizmos.DrawWireCube(boxCenter, new Vector3(boxSize.x, boxSize.y + extraCastDistance, 1f));
        }
    }
}

// ============================================================================
// KONTRAK INTERFACE GLOBAL (Bisa dibaca oleh SkeletonAI dan EnemyAI dari mana saja)
// ============================================================================
public interface IDamageable
{
    void TakeDamage(int damage);
}