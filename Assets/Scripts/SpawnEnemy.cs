using Assets.Globals;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

    private GameObject player;
    private CraftingMenu cMenu;
    private UIText uiMenu;

    // Start is called before the first frame update
    void Start()
	{
		if(PossibleEnemies == null || PossibleEnemies.Length == 0)
        {
            Debug.LogError("No possible enemies assigned to SpawnEnemy script.");
            return;
        }

        player = funcInst.GetSiblingGameObject("Player");
        cMenu = player.GetComponent<CraftingMenu>();
        uiMenu = player.GetComponent<UIText>();
    }

	// Update is called once per frame
	void Update()
	{
        if (uiMenu.GameOverScreen.activeSelf || cMenu.showCraftingMenu) return;
        if (Time.time >= nextSpawnTime)
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
            nextSpawnTime = spawnIntervalMin + Time.time;
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
        enemyPrefab.GetComponent<EnemyController>().enemyAttack = PossibleEnemies[randomIndex].GetComponent<EnemyController>().enemyAttack;
    }
}