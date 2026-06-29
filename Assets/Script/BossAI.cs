using UnityEngine;

public class BossAI : MonoBehaviour, IDamageable
{
    [Header("Target")]
    private Transform playerTransform;

    [Header("Movement")]
    public float chaseSpeed = 3f;
    public float chaseDistance = 10f;
    public float attackRange = 2.5f;

    [Header("Boss Stats")]
    public int health = 200;
    public int normalDamage = 20;
    public int specialDamage = 50;
    public float attackCooldown = 2f;

    private Animator anim;
    private Rigidbody2D rb;

    private bool isDead = false;
    private bool isFacingRight = false;
    private bool isAttacking = false;

    private float nextAttackTime;

    // 3 Normal -> 1 Special
    private int attackCount = 0;

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

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Attack") ||
            state.IsName("SpecialAttack") ||
            state.IsName("Death"))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= chaseDistance)
        {
            ChasePlayer(distance);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("Run", false);
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void ChasePlayer(float distance)
    {
        FacePlayer();

        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("Run", false);

            if (!isAttacking && Time.time >= nextAttackTime)
            {
                Attack();
            }
        }
        else
        {
            float dir = playerTransform.position.x > transform.position.x ? 1 : -1;

            rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);

            anim.SetBool("Run", true);
        }
    }

    void Attack()
    {
        isAttacking = true;

        attackCount++;

        if (attackCount <= 3)
        {
            anim.SetTrigger("Attack");
            Debug.Log("Normal Attack");
        }
        else
        {
            anim.SetTrigger("SpecialAttack");
            Debug.Log("SPECIAL ATTACK");

            attackCount = 0;
        }

        nextAttackTime = Time.time + attackCooldown;
    }

    // Animation Event
    public void DealDamage()
    {
        if (playerTransform == null)
            return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance > attackRange)
            return;

        IDamageable damageable = playerTransform.GetComponent<IDamageable>();

        if (damageable == null)
            return;

        if (attackCount == 0)
        {
            damageable.TakeDamage(specialDamage);
            Debug.Log("Special Damage : " + specialDamage);
        }
        else
        {
            damageable.TakeDamage(normalDamage);
            Debug.Log("Normal Damage : " + normalDamage);
        }
    }

    // Animation Event di frame terakhir Attack & SpecialAttack
    public void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        health -= damage;

        Debug.Log("Boss HP : " + health);

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

        anim.ResetTrigger("Attack");
        anim.ResetTrigger("SpecialAttack");
        anim.SetBool("Run", false);
        anim.SetTrigger("Death");

        Destroy(gameObject, 2f);
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}