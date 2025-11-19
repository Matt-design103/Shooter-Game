using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public GameObject player;
    public Transform playerPos;
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Attack Settings")]
    public GameObject enemyBullet;
    public Transform bulletSpawnPos;
    public float timeBetweenAttacks = 1f;
    private bool alreadyAttacked;

    [Header("Ranges")]
    public float activationRange;
    public float attackRange;

    private bool playerInActivationRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
      
        playerInActivationRange = Physics.CheckSphere(transform.position, activationRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInAttackRange && playerInActivationRange)
        {
            AttackPlayer();
        }
        else if (playerInActivationRange)
        {
            ChasePlayer();
        }
        else
        {
            agent.SetDestination(transform.position); // idle
        }

    }

    private void ChasePlayer()
    {
        if (agent.enabled && playerPos != null)
        {
            agent.SetDestination(playerPos.position); //  fixed parameter
        }
    }

    private void AttackPlayer()
    {
        // Stop moving
        agent.SetDestination(transform.position);

        // Face the player
        transform.LookAt(playerPos);

        if (!alreadyAttacked)
        {
            //  Fire bullet
            GameObject bullet = Instantiate(enemyBullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
            
          

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
