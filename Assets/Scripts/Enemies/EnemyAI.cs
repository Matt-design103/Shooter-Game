using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection")]
    public float activationRange = 12f;
    public float fieldOfViewAngle = 60f;
    public LayerMask obstacleLayer = 1;
    
    [Header("Combat")]
    public float attackRange = 8f;
    public float attackCooldown = 2f;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public float combatSpeed = 4f;
    
    // Components
    private NavMeshAgent agent;
    private Transform player;
    //private Animator animator;
    
    // State Management
    public enum AIState { Inactive, Combat }
    public AIState currentState;
    
    // Combat Variables
    private float lastAttackTime = 0f;
    private bool hasLineOfSight = false;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
        
        // Find player
        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
            player = playerGO.transform;
        
        // Initialize state
        currentState = AIState.Inactive;
        agent.speed = 0f; // Don't move when inactive
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Check for activation
        if (currentState == AIState.Inactive)
        {
            if (distanceToPlayer <= activationRange && CanSeePlayer())
            {
                ActivateEnemy();
            }
        }
        else if (currentState == AIState.Combat)
        {
            HandleCombatState();
        }
        
        // Update animator parameters
        //UpdateAnimator();
    }
    
    void HandleCombatState()
    {
        hasLineOfSight = HasLineOfSight(player.position);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // If we have line of sight and are in range, attack
        if (hasLineOfSight && distanceToPlayer <= attackRange)
        {
            // Stop moving and face player
            agent.SetDestination(transform.position);
            FaceTarget(player.position);
            
            // Attack if cooldown is ready
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            // Move to a position where we can attack the player
            Vector3 targetPosition = FindAttackPosition();
            if (targetPosition != Vector3.zero)
            {
                agent.SetDestination(targetPosition);
            }
        }
    }
    
    bool CanSeePlayer()
    {
        if (player == null) return false;
        
        // Check field of view
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        
        if (angle > fieldOfViewAngle / 2f) return false;
        
        // Check line of sight
        return HasLineOfSight(player.position);
    }
    
    bool HasLineOfSight(Vector3 targetPosition)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f; // Eye level
        Vector3 directionToTarget = (targetPosition - rayOrigin).normalized;
        float distanceToTarget = Vector3.Distance(rayOrigin, targetPosition);
        
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, directionToTarget, out hit, distanceToTarget, obstacleLayer))
        {
            return false; // Obstacle in the way
        }
        
        return true;
    }
    
    Vector3 FindAttackPosition()
    {
        Vector3 playerPos = player.position;
        
        // Try to find a position within attack range that has line of sight
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 testPosition = playerPos + direction * (attackRange * 0.8f);
            
            // Check if position is on navmesh
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(testPosition, out navHit, 2f, NavMesh.AllAreas))
            {
                // Check if we would have line of sight from this position
                if (HasLineOfSightFromPosition(navHit.position, playerPos))
                {
                    return navHit.position;
                }
            }
        }
        
        // If no good position found, just move towards player
        Vector3 directionToPlayer = (playerPos - transform.position).normalized;
        return playerPos - directionToPlayer * (attackRange * 0.7f);
    }
    
    bool HasLineOfSightFromPosition(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 direction = (toPosition - fromPosition).normalized;
        float distance = Vector3.Distance(fromPosition, toPosition);
        
        RaycastHit hit;
        return !Physics.Raycast(fromPosition + Vector3.up * 1.5f, direction, out hit, distance, obstacleLayer);
    }
    
    void Attack()
    {
        lastAttackTime = Time.time;
        
        if (projectilePrefab != null && firePoint != null)
        {
            // Create projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            // Add velocity to projectile
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
                rb.linearVelocity = directionToPlayer * projectileSpeed;
            }
            
            // Destroy projectile after 5 seconds
            Destroy(projectile, 5f);
        }
        
        Debug.Log("Enemy attacking player!");
    }
    
    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Keep enemy upright
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    
    void ActivateEnemy()
    {
        currentState = AIState.Combat;
        agent.speed = combatSpeed;
        Debug.Log("Enemy activated - engaging player!");
    }
    
    /*void UpdateAnimator()
    {
        if (animator == null) return;
        
        // Update speed parameter
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
        
        // Update state parameters
        animator.SetBool("IsInactive", currentState == AIState.Inactive);
        animator.SetBool("IsInCombat", currentState == AIState.Combat);
        
        // Trigger attack animation
        if (currentState == AIState.Combat && Time.time <= lastAttackTime + 0.1f)
        {
            animator.SetTrigger("Attack");
        }
    }*/
}