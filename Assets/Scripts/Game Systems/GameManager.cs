using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance => _instance;

    #region Game Properties
    private static float _gameTime = 60;
    private static float _enemySpawnRate = 5;
    public static float gameTime { get => _gameTime; set => _gameTime = value; }
    public static float enemySpawnRate { get => _enemySpawnRate; set => _enemySpawnRate = value; }
    #endregion

    #region Game Elements
    [SerializeField] PirateShip _playerShip;
    [SerializeField] PlayerCamera _playerCamera;
    [SerializeField] GameMapGenerator _mapGenerator;
    [SerializeField] float _mapLimitsBorderSize;
    [SerializeField] float _enemySpawnBorderSize = 10f;
    [SerializeField] EnemySpawner _enemySpawner;
    [SerializeField] LoadingFade _loadingFade;

    public PirateShip playerShip => _playerShip;
    public Transform playerTransform => _playerShip.transform;
    public Vector2 cameraPosition => _playerCamera.transform.position;
    public Camera gameCamera => _playerCamera.camera;
    public GameMapGenerator mapGenerator => _mapGenerator;
    public float mapLimitsBorderSize => _mapLimitsBorderSize;
    public float enemySpawnBorderSize => _enemySpawnBorderSize;
    #endregion

    #region Game State
    private float _gameTimeCounter;
    public float gameTimeCounter => _gameTimeCounter;
    private int _playerScore;
    public int playerScore => _playerScore;

    [SerializeField] UnityEvent _onGameStart;
    [SerializeField] UnityEvent _onGameEnd;

    public delegate void OnGameEnd();
    public OnGameEnd onGameEnd;
    #endregion

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartNewGame());
    }

    private void Update()
    {
        if (_gameTimeCounter > 0)
        {
            _gameTimeCounter -= Time.deltaTime;
            if (_gameTimeCounter <= 0)
            {
                _gameTimeCounter = 0;
                EndGame();
            }
        }
    }

    #region Game Flow
    public IEnumerator StartNewGame() 
    {
        ResetScore();

        _mapGenerator.CreateNewMap();

        float playerMinPosition = _mapGenerator.mapSize / 4f;
        float playerMaxPosition = _mapGenerator.mapSize * 3f / 4f;
        playerTransform.position = new Vector2(
            Mathf.Lerp(playerMinPosition, playerMaxPosition, Random.value),
            Mathf.Lerp(playerMinPosition, playerMaxPosition, Random.value));
        playerTransform.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        _gameTimeCounter = _gameTime;
        _loadingFade.Open();
        _enemySpawner.enabled = true;
        _onGameStart.Invoke();
    }

    public void EndGame() 
    {
        _enemySpawner.enabled = false;
        GameTimeManager.CreateTimeScaleMultiplier(this, 0);
        _onGameEnd.Invoke();
        onGameEnd?.Invoke();
    }

    public IEnumerator DiscardLastGame() 
    {
        _loadingFade.Close();

        yield return new WaitUntil(() => _loadingFade.currentOpenValue <= 0);
        GameTimeManager.DestroyTimeScaleMultiplier(this);

        _enemySpawner.RemoveAllEnemies();

        _mapGenerator.ClearMap();

        playerTransform.gameObject.SetActive(false);
    }

    public IEnumerator ReturnToMainMenu(string mainMenuScene)
    {
        _loadingFade.Close();

        yield return new WaitUntil(() => _loadingFade.currentOpenValue <= 0);
        GameTimeManager.DestroyTimeScaleMultiplier(this);

        SceneManager.LoadScene(mainMenuScene);
    }
    #endregion

    #region Score
    public void AddScore(int scoreToAdd)
    {
        _playerScore += scoreToAdd;
    }

    private void ResetScore()
    {
        _playerScore = 0;
    }
    #endregion
}
