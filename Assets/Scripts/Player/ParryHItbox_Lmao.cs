using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryHItbox_Lmao : MonoBehaviour
{
    public float parryDuration = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyAfterTime(parryDuration));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            Debug.Log("Parried an attack!");
            Destroy(other.gameObject); // Destroy the enemy attack object
        }
    }

    IEnumerator DestroyAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Destroy(gameObject);
        Debug.Log("You did it bro");
        
    }
}
