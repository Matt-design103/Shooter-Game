using UnityEngine;
using System.Collections;

public class GenericDecay : MonoBehaviour
{
    public float lifeTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Decay());
    }

    IEnumerator Decay()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
