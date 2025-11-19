using UnityEngine;
using System.Collections;

public class BasicRangedEnemy : RangedEnemy
{
    public GameObject chargedEnemyBullet;
   
    protected override void PerformRangedAttack()
    {
    
        if (!alreadyAttacked)
        {
        int attackType = Random.Range(0, 4);
        if (attackType == 0)
        {
            RangedAttack2();
        }
        else
        {
            RangedAttack1();
        }
        }
        
    }

    protected override void RangedAttack2()
    {
        //charged shot attack
        StartCoroutine(ChargedShotAttack());
    }

    IEnumerator ChargedShotAttack()
    {
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            yield return new WaitForSeconds(1f); //wait for charge time

            Instantiate(chargedEnemyBullet, bulletSpawnPos.position, bulletSpawnPos.rotation);
            // schedule the reset after the configured attack cooldown
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
}
