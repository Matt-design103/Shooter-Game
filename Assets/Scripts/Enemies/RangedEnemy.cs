using UnityEngine;
public class RangedEnemy : EnemyBase
{
    [Header("Ranged Specific Settings")]
    public float strafeRange = 8f; // Minimum distance to maintain
    public float strafeSpeed = 3f;
    public float strafeChangeInterval = 2f;
    
    private Vector3 strafeDirection;
    private float strafeTimer;
    private bool isCurrentlyMoving = false;
    
    protected override void ExecuteBehavior()
    {
        if (playerInMeleeRange)
        {
            // Simple melee attack when player gets too close, but don't chase
            PerformMeleeAttack();
        }
        else if (playerInAttackRange)
        {
            // Strafe and shoot at firing range
            StrafeAndShoot();
        }
        else
        {
            // Move to firing range
            ChasePlayer();
        }
        
        // Update animations based on actual movement
        UpdateAnimationFromVelocity();
    }
    
    private void StrafeAndShoot()
    {
        // Maintain distance while strafing
        if (distanceToPlayer < strafeRange)
        {
            // Move away from player
            Vector3 directionAway = (transform.position - playerPos.position).normalized;
            agent.SetDestination(transform.position + directionAway * 2f);
        }
        else
        {
            // Strafe perpendicular to player
            strafeTimer += Time.deltaTime;
            if (strafeTimer >= strafeChangeInterval)
            {
                strafeTimer = 0f;
                // Random strafe direction
                Vector3 directionToPlayer = (playerPos.position - transform.position).normalized;
                Vector3 perpendicular = Vector3.Cross(directionToPlayer, Vector3.up);
                strafeDirection = (Random.value > 0.5f ? perpendicular : -perpendicular).normalized;
            }
            
            Vector3 strafePos = transform.position + strafeDirection * strafeSpeed * Time.deltaTime;
            agent.SetDestination(strafePos);
        }
        
        // Face player and shoot
        FacePlayer();
        PerformRangedAttack();
    }
    
    private void UpdateAnimationFromVelocity()
    {
           if (agent != null && animator != null)
    {
        float speed = agent.velocity.magnitude;
        // Smooth the speed value
        float currentSpeed = animator.GetFloat("speed");
        float smoothedSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime * 5f);
        Debug.Log("Smoothed Speed: " + smoothedSpeed);
        animator.SetFloat("speed", smoothedSpeed);
        animator.Update(0);
    }
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // Strafe range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, strafeRange);
    }
}