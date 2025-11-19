using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    protected GameObject player;
    protected Transform playerPos;
    public LayerMask whatIsGround, whatIsPlayer;
    public Animator animator;

    [Header("Attack Settings")]
    public GameObject enemyBullet;
    public bool canShoot = true;
    public Transform bulletSpawnPos;
    public float timeBetweenAttacks = 1f;
    protected bool alreadyAttacked;
    
    [Header("Melee Attack Settings")]
    public float meleeAttackRange = 2f;
    public int meleeDamage = 10;
    public float meleeKnockback = 5f;
    public GameObject meleeAttackHitbox;
    
    [Header("Ranges")]
    public float activationRange = 15f;
    public float attackRange = 10f;
    
    [Header("State")]
    protected bool isActivated = false;
    protected bool playerInActivationRange;
    protected bool playerInAttackRange;
    protected bool playerInMeleeRange;
    protected float distanceToPlayer;
    
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerPos = player.transform;
        }
        agent = GetComponent<NavMeshAgent>();
        
        // Start deactivated
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }
    
    protected virtual void Update()
    {
        if (playerPos == null) return;
        
        // Update distance and range checks
        distanceToPlayer = Vector3.Distance(transform.position, playerPos.position);
        playerInActivationRange = Physics.CheckSphere(transform.position, activationRange, whatIsPlayer);
        playerInAttackRange = distanceToPlayer <= attackRange;
        playerInMeleeRange = distanceToPlayer <= meleeAttackRange;
        
        // Activate if player enters range
        if (!isActivated && playerInActivationRange)
        {
            Activate();
        }
        
        // Once activated, stay active
        if (!isActivated)
        {
            return;
        }
        
        // Let derived classes handle behavior
        ExecuteBehavior();
    }
    
    protected virtual void Activate()
    {
        animator.SetBool("isAwake", true);
        isActivated = true;
        if (agent != null)
        {
            agent.isStopped = false;
            
        }
    }
    
    // Abstract method - each enemy type implements their own behavior
    protected abstract void ExecuteBehavior();
    
    #region Attack Methods
    // Virtual methods - can be overridden in derived classes for custom attacks
    protected virtual void PerformRangedAttack()
    {
        if (!alreadyAttacked && enemyBullet != null && bulletSpawnPos != null)
        {
            // Random attack variation
            int attackVariation = Random.Range(0, 2);
            
            switch (attackVariation)
            {
                case 0:
                    RangedAttack1();
                    break;
                case 1:
                    RangedAttack2();
                    break;
            }
            
         
        }
    }
    
    protected virtual void PerformMeleeAttack()
    {
        StopMovement();
        FacePlayer();
        
        if (!alreadyAttacked)
        {
            // Random melee attack variation
            int attackVariation = Random.Range(0, 2);
            
            switch (attackVariation)
            {
                case 0:
                    MeleeAttack1();
                    break;
                case 1:
                    MeleeAttack2();
                    break;
            }
            
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    // Individual attack methods - override these for custom attacks
    protected virtual void RangedAttack1()
    {
        animator.SetTrigger("shoot");
        // Default: Single shot

    }
    
    protected virtual void ShootAndReset()
    {
         Instantiate(enemyBullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
        alreadyAttacked = true;
        // schedule reset after cooldown
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
    
    protected virtual void RangedAttack2()
    {
        // Default: Burst fire
        StartCoroutine(BurstFire(3, 0.1f));
        alreadyAttacked = true;
        // schedule reset after cooldown
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
    
    protected virtual void MeleeAttack1()
    {
        // spawn hitbox for now
        Instantiate(meleeAttackHitbox, transform.position + transform.forward * (meleeAttackRange / 2), transform.rotation);
        
    }
    
    protected virtual void MeleeAttack2()
    {
        Instantiate(meleeAttackHitbox, transform.position + transform.forward * (meleeAttackRange / 2), transform.rotation);
    }
    
    protected void MeleeAttackHit(int damage, float knockback)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, meleeAttackRange, whatIsPlayer);
        foreach (var hitCollider in hitColliders)
        {
           
            // Apply knockback
            var playerRb = hitCollider.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 knockbackDir = (hitCollider.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDir * knockback, ForceMode.Impulse);
            }
        }
    }
    
    protected System.Collections.IEnumerator BurstFire(int shots, float delay)
    {
        for (int i = 0; i < shots; i++)
        {
            if (enemyBullet != null && bulletSpawnPos != null)
            {
                Instantiate(enemyBullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
            }
            yield return new WaitForSeconds(delay);
        }
    }
    #endregion
    
    #region Movement Methods
    protected void ChasePlayer()
    {
        if (agent != null && agent.enabled && playerPos != null)
        {
            agent.SetDestination(playerPos.position);
        }
    }
    
    protected void StopMovement()
    {
        if (agent != null && agent.enabled)
        {
            agent.SetDestination(transform.position);
        }
    }
    
    protected void FacePlayer()
    {
        if (playerPos != null)
        {
            Vector3 direction = (playerPos.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                    Quaternion.LookRotation(direction), Time.deltaTime * 10f);
            }
        }
    }
    #endregion
    
    // changed ResetAttack from IEnumerator to void so Invoke can call it
    protected virtual void ResetAttack()
    {
        alreadyAttacked = false;
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        // Activation range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);
        
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Melee range
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
    }
}