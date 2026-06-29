using UnityEngine;

public class SkeletonAI : MonoBehaviour, IDamageable
{
    private Transform playerTransform;
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float walkTime = 3f;

    [Header("Combat")]
    public int health = 30;
    public int damageAmount = 7;
    public float attackRange = 4f;
    public float chaseDistance = 8f;
    public float attackCooldown = 3f;

    private float patrolTimer;
    private float nextAttackTime;

    private bool firstAttack = true;
    private bool isFacingRight = false;
    private bool isAgro = false;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        patrolTimer = walkTime;

        if (anim != null)
            anim.SetBool("isWalking", false);

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

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Attack") || state.IsName("Death"))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isAgro)
            ChasePlayer();
        else
            Patrol();
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerTransform = player.transform;
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;

        float dir = isFacingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);

        anim.SetBool("isWalking", true);

        if (patrolTimer <= 0f)
        {
            Flip();
            patrolTimer = walkTime;
        }
    }

    void ChasePlayer()
    {
        if (playerTransform == null)
            return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        FacePlayer();

        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);

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
        else if (distance <= chaseDistance)
        {
            float dir = playerTransform.position.x > transform.position.x ? 1f : -1f;

            rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);

            anim.SetBool("isWalking", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);

            isAgro = false;
            firstAttack = true;
            patrolTimer = walkTime;
        }
    }

    void Attack()
    {
        if (isDead)
            return;

        Debug.Log("Skeleton menyerang");
        anim.SetTrigger("AttackTrigger");
    }

    public void OnAttackHitEvent()
    {
        if (isDead)
            return;

        if (playerTransform == null)
            return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= attackRange)
        {
            PlayerHealth player = playerTransform.GetComponent<PlayerHealth>();

            if (player != null)
            {
                Debug.Log("Player terkena damage");
                player.TakeDamage(damageAmount);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        health -= damage;

        Debug.Log("Skeleton terkena damage. HP : " + health);

        if (health <= 0)
        {
            health = 0;
            Die();
            return;
        }

        isAgro = true;

        if (playerTransform == null)
            FindPlayer();

        FacePlayer();

        if (playerTransform != null)
        {
            float dir = playerTransform.position.x > transform.position.x ? 1f : -1f;

            rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);

            anim.SetBool("isWalking", true);
        }
    }
        void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isAgro = false;
        firstAttack = false;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        anim.ResetTrigger("AttackTrigger");
        anim.SetBool("isWalking", false);
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
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}