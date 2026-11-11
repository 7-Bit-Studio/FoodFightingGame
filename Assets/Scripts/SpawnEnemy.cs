using UnityEngine;
using Assets.Globals;

public class SpawnEnemy : MonoBehaviour
{
	[SerializeField] private GameObject[] PossibleEnemies;
    [SerializeField] private bool randomizeSpawnDelay = true;
    [SerializeField] private bool randomizeSpawnPosition = true;
    [SerializeField] private float spawnIntervalMin = 0.5f;
    [SerializeField] private float spawnIntervalMax = 2f;
    [SerializeField] private float spawnRadius = 5f;
    private Functions funcs = new();
	private FunctionsInstance funcInst = new();
	private float nextSpawnTime;

    // Start is called before the first frame update
    void Start()
	{
		if(PossibleEnemies == null || PossibleEnemies.Length == 0)
        {
            Debug.LogError("No possible enemies assigned to SpawnEnemy script.");
            return;
        }
    }

	// Update is called once per frame
	void Update()
	{
        if(Time.time >= nextSpawnTime)
        {
            DoSpawnEnemy();
        }
    }

	void DoSpawnEnemy()
    {
        int randomIndex = Random.Range(0, PossibleEnemies.Length);
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float distance = Random.Range(0f, spawnRadius);

        if (randomizeSpawnDelay)
        {
            nextSpawnTime = Random.Range(spawnIntervalMin, spawnIntervalMax) + Time.time;
        }
        else
        {
            nextSpawnTime = spawnIntervalMax + Time.time;
        }

        if (!randomizeSpawnPosition)
        {
            angle = 0f;
            distance = 0f;
        }

        GameObject enemyPrefab;
        enemyPrefab = Instantiate(PossibleEnemies[randomIndex], transform.position, Quaternion.identity);
        enemyPrefab.tag = "Enemy";
        enemyPrefab.name = $"{PossibleEnemies[randomIndex].name}";
        distance = Mathf.Sqrt(distance) * spawnRadius;
        enemyPrefab.transform.position += new Vector3(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle), 0);
        enemyPrefab.GetComponent<EnemyController>().isStunned = false;
    }
}