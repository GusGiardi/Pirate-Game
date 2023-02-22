using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private static EnemySpawner _instance;
    public static EnemySpawner instance => _instance;

    [SerializeField] GameObject[] _enemyPrefabs;

    private float _enemySpawnTimeCounter;
    private List<GameObject> _spawnedEnemies = new List<GameObject>();

    private void Awake()
    {
        _instance = this;
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

        float mapSize = GameManager.instance.mapGenerator.mapSize;
        float cameraHeight = GameManager.instance.gameCamera.orthographicSize;
        float cameraWidth = (cameraHeight * Screen.width) / Screen.height;
        Vector2 cameraPosition = GameManager.instance.cameraPosition;

        float minPos = - GameManager.instance.enemySpawnBorderSize;
        float maxPos = mapSize + GameManager.instance.enemySpawnBorderSize;

        float screenMinX = cameraPosition.x - cameraWidth - GameManager.instance.enemySpawnBorderSize;
        float screenMaxX = cameraPosition.x + cameraWidth + GameManager.instance.enemySpawnBorderSize;
        float screenMinY = cameraPosition.y - cameraHeight - GameManager.instance.enemySpawnBorderSize;
        float screenMaxY = cameraPosition.y + cameraHeight + GameManager.instance.enemySpawnBorderSize;

        float posX;
        float posY;
        do
        {
            posX = Mathf.Lerp(minPos, maxPos, Random.value);
            posY = Mathf.Lerp(minPos, maxPos, Random.value);
        } while (posX >= screenMinX && posX <= screenMaxX && posY >= screenMinY && posY >= screenMaxY);

        GameObject newEnemy = ObjectPoolManager.instance.InstantiateInPool(_enemyPrefabs[rnd], new Vector2(posX, posY), Quaternion.identity);
        _spawnedEnemies.Add(newEnemy);
    }

    public void RemoveEnemy(GameObject enemyToRemove) 
    {
        if (!_spawnedEnemies.Contains(enemyToRemove))
            return;

        enemyToRemove.SetActive(false);
        _spawnedEnemies.Remove(enemyToRemove);
    }

    public void RemoveAllEnemies() 
    {
        foreach(GameObject enemy in _spawnedEnemies) 
        {
            enemy.SetActive(false);
        }

        _spawnedEnemies.Clear();
    }
}
