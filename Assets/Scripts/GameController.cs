using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    // Outlets
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;
    public int[] numEnemies = new int[4];

    // State Tracking
    public float timeElapsed;
    public float enemyDelay;
    public float maxBasicEnemies = 10;
    private int spawnPoint;

    // Configuration
    public float minEnemyDelay = 5f;
    public float maxEnemyDelay = 1;

    // Methods
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("EnemySpawnTimer");
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        //Compute Enemy Delay
        float decreaseDelayOverTime = maxEnemyDelay - ((maxEnemyDelay - minEnemyDelay) / 30f * timeElapsed);
        enemyDelay = Mathf.Clamp(decreaseDelayOverTime, minEnemyDelay, maxEnemyDelay);

        // If enemies > 20, stop coroutine
        // Else, start
    }
    void SpawnEnemy()
    {
        for (int i = 0; i < numEnemies.Length; i++)
        {
            if (numEnemies[i] <= maxBasicEnemies)
            {
                GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                numEnemies[i]+=1;
                
                var go = Instantiate(randomEnemyPrefab, spawnPoints[i].position, Quaternion.identity);
                go.GetComponent<Enemy>().spawnPoint = i;
            }
        }
    }
    public void DeSpawn(int point)
    {
        Debug.Log(point);
        Debug.Log(numEnemies.Length);
        numEnemies[point] -= 1;
    }
    IEnumerator EnemySpawnTimer()
    {
        // Wait
        yield return new WaitForSeconds(maxEnemyDelay);

        // Spawn
        SpawnEnemy();

        //Repeat
        StartCoroutine("EnemySpawnTimer");
    }
}
