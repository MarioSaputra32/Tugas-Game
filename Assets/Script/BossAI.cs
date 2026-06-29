using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour, IDamageable
{
    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerHealth playerHealth;

    [Header("Movement")]
    public float chaseSpeed = 3f;
    public float chaseDistance = 12f;
    public float attackRange = 3f;

    [Header("Combat")]
    public int maxHealth = 200;
    public int currentHealth;
    public int damage = 20;
    public float attackCooldown = 1.5f;
    public float damageDelay = 0.5f;

    private float nextAttackTime;

    private bool isFacingRight = false;
    private bool isAgro = false;
    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        FindPlayer();
    }

    void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseDistance)
            isAgro = true;

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isAgro)
            ChasePlayer(distance);
        else
            rb.linearVelocity = Vector2.zero;

        FacePlayer();
    }

    void FindPlayer()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
        {
            player = obj.transform;

            playerHealth = obj.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                playerHealth = obj.GetComponentInChildren<PlayerHealth>();
        }
    }

    void ChasePlayer(float distance)
    {
        if (player == null) return;

        if (distance > chaseDistance)
        {
            isAgro = false;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // ATTACK
        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(AttackRoutine());
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            float dir = player.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        rb.linearVelocity = Vector2.zero;

        anim.SetTrigger("AttackTrigger");

        yield return new WaitForSeconds(damageDelay);

        DealDamage();

        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }

    void DealDamage()
    {
        if (playerHealth == null || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Debug.Log("Boss hit player!");
            playerHealth.TakeDamage(damage);
        }
    }

    // =========================
    // IDAMAGEABLE IMPLEMENTATION
    // =========================
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        Debug.Log("Boss kena damage: " + damageAmount);
        Debug.Log("HP Boss: " + currentHealth);

        isAgro = true;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        isAgro = false;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        anim.SetTrigger("DeathTrigger");

        Destroy(gameObject, 2f);
    }

    void FacePlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && !isFacingRight)
            Flip();
        else if (player.position.x < transform.position.x && isFacingRight)
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}