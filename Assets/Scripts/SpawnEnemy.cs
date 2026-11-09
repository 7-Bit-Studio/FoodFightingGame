using UnityEngine;
using Assets.Globals;

public class SpawnEnemy : MonoBehaviour
{
	[SerializeField] private GameObject[] PossibleEnemies;
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
        GameObject enemyPrefab = new GameObject();
        enemyPrefab = Instantiate(PossibleEnemies[randomIndex], transform.position, Quaternion.identity);
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        float randomDistance = Random.Range(0f, 1f);
        randomDistance = Mathf.Sqrt(randomDistance) * spawnRadius;
        enemyPrefab.transform.position += new Vector3(randomDistance * Mathf.Cos(randomAngle), randomDistance * Mathf.Sin(randomAngle), 0);
        enemyPrefab.GetComponent<EnemyController>().isStunned = false;
        nextSpawnTime = Random.Range(spawnIntervalMin, spawnIntervalMax) + Time.time;
    }
}