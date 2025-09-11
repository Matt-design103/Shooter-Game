using UnityEngine;

public class Shotty : Weapon
{
    public int pellets = 8;
    public float spreadRange = 5f;
    float RandomGaussian(float mean = 0f, float stdDev = 1f)
{
    float u1 = 1.0f - Random.value; // avoid log(0)
    float u2 = 1.0f - Random.value;
    float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                          Mathf.Sin(2.0f * Mathf.PI * u2);
    return mean + stdDev * randStdNormal;
}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Fire()
    {
       for (int i = 0; i < pellets; i++)
        {
            float spreadX = RandomGaussian(0f, spreadRange * 0.3f);
            float spreadY = RandomGaussian(0f, spreadRange * 0.3f);

            Quaternion rotation = bulletSpawnPos.rotation * Quaternion.Euler(spreadY, spreadX, 0f);

            GameObject pellet = Instantiate(bulletPrefab, bulletSpawnPos.position, rotation);

        }

    }
}
