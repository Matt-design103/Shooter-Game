using UnityEngine;

public class Level : MonoBehaviour
{
    public string levelName;
    public int levelIndex;
    public GameObject[] enemies;
    public Transform[] enemySpawnPoints;
    public Transform entryPoint;
    public Transform exitPoint;
    public float enemySpawnBudget;
    private float budgetUsed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        budgetUsed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemies()
    {
        foreach (Transform spawnPoint in enemySpawnPoints)
        {
            foreach (GameObject enemy in enemies)
            {
                float enemyCost = enemy.GetComponent<EnemyHealth>().spawnCost;
                if (budgetUsed + enemyCost <= enemySpawnBudget)
                {
                    Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
                    budgetUsed += enemyCost;
                }
            }
        }
    }
}
