using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] _enemyPrefabs;

    private float _enemySpawnTimeCounter;

    private void Awake()
    {
        _enemySpawnTimeCounter = GameManager.enemySpawnRate;
    }

    private void Update()
    {
        _enemySpawnTimeCounter -= Time.deltaTime;
        if (_enemySpawnTimeCounter <= 0) 
        {
            SpawnEnemy();
            _enemySpawnTimeCounter = GameManager.enemySpawnRate;
        }
    }

    private void SpawnEnemy() 
    {
        int rnd = Random.Range(0, _enemyPrefabs.Length);
    }
}
