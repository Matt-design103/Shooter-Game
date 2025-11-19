using UnityEngine;

public class MeleeEnemy : EnemyBase
{

    protected override void ExecuteBehavior()
    {
        if (playerInAttackRange)
        {
            // Perform melee attack when in range
            PerformMeleeAttack();
        }
        else
        {
            // Chase the player if not in attack range
            ChasePlayer();
        }
    }


}
