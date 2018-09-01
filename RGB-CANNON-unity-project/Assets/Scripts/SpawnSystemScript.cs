using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystemScript : MonoBehaviour {

    public float screenbuffer;
    public GameObject enemy;
    public float spawnSpeedDivider;
    public float enemySpeedIncreaser;
    public float maxTimeTillSpawn;
    public float minTimeTillSpawn;
    public float keepMaxTimeTillSpawnAbove;
    public StatsLoggerScript _GM;


    public float startDelay;

    public Transform[] spawnLocations;

    enum SpawnState
    {
        spawning,
        waiting,
    }

    SpawnState spawnState;

    int numOfSpawnedEnemies;

	// Use this for initialization
	void Start () {
        _GM = GameObject.FindGameObjectWithTag("Stats").GetComponent<StatsLoggerScript>();
        spawnState = SpawnState.waiting;
        _GM.startedSpawning = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (!_GM.startedSpawning)
        {
            StartCoroutine(StartDelay());
        }

        if (_GM.startedSpawning & _GM.gameIsPlaying & spawnState == SpawnState.waiting)
        {
            StartCoroutine(SpawnEnemy());
        }
	}

    IEnumerator SpawnEnemy()
    {
        spawnState = SpawnState.spawning;
        Transform newEnemyTransform = spawnLocations[Random.Range(0, spawnLocations.Length)].transform;
        GameObject newEnemy = Instantiate(enemy, newEnemyTransform.position, enemy.transform.rotation);
        newEnemy.GetComponent<EnemyScript>().speed += enemySpeedIncreaser*numOfSpawnedEnemies;
        yield return new WaitForSeconds(Random.Range(minTimeTillSpawn, maxTimeTillSpawn));

        numOfSpawnedEnemies++;
        if (maxTimeTillSpawn > keepMaxTimeTillSpawnAbove)
        {

            maxTimeTillSpawn *= Mathf.Exp(-1 / spawnSpeedDivider);
        }
        spawnState = SpawnState.waiting;
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelay);
        
    }

    public int GetNumOfSpawnedEnemies
    {
        get { return numOfSpawnedEnemies;}
    }
}
