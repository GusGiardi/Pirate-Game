using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;

public class MainMenu : MonoBehaviour
{
    [SerializeField] LoadingFade _loadingFade;

    [SerializeField] string _gameScene;

    [SerializeField] AudioClip _menuMusic;

    #region Settings
    [Header("Settings")]
    [SerializeField] GameObject _settingsScreen;
    [SerializeField] TextMeshProUGUI _gameTimeValueText;
    [SerializeField] TextMeshProUGUI _enemySpawnTimeValueText;
    private List<float> _gameTimeOptions = new List<float>
    {
        60,
        120,
        180
    };
    private List<float> _enemySpawnTimeOptions = new List<float>
    {
        1,
        2,
        3,
        4,
        5
    };
    #endregion

    private StringBuilder sb = new StringBuilder();

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitForSeconds(2);
        _loadingFade.Open();
        MusicManager.instance.PlayMusic(_menuMusic);
    }

    public void StartGameButton()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        _loadingFade.Close();

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(_gameScene);
    }

    #region Settings
    public void OpenSettingsScreen()
    {
        UpdateGameTimeText();

        UpdateEnemySpawnTimeText();

        _settingsScreen.SetActive(true);
    }

    public void AddGameTime()
    {
        int currentGameTimeIndex = _gameTimeOptions.IndexOf(GameManager.gameTime);
        currentGameTimeIndex++;
        if (currentGameTimeIndex >= _gameTimeOptions.Count)
            currentGameTimeIndex = 0;

        GameManager.gameTime = _gameTimeOptions[currentGameTimeIndex];

        UpdateGameTimeText();
    }

    public void SubtractGameTime()
    {
        int currentGameTimeIndex = _gameTimeOptions.IndexOf(GameManager.gameTime);
        currentGameTimeIndex--;
        if (currentGameTimeIndex < 0)
            currentGameTimeIndex = _gameTimeOptions.Count - 1;

        GameManager.gameTime = _gameTimeOptions[currentGameTimeIndex];

        UpdateGameTimeText();
    }

    private void UpdateGameTimeText()
    {
        int minutes = Mathf.FloorToInt(GameManager.gameTime / 60);
        int seconds = (int)(GameManager.gameTime - minutes * 60);
        sb.Append(minutes.ToString("00"));
        sb.Append(":");
        sb.Append(seconds.ToString("00"));
        _gameTimeValueText.text = sb.ToString();
        sb.Clear();
    }

    public void AddEnemySpawnTime()
    {
        int currentEnemySpawnTimeIndex = _enemySpawnTimeOptions.IndexOf(GameManager.enemySpawnRate);
        currentEnemySpawnTimeIndex++;
        if (currentEnemySpawnTimeIndex >= _enemySpawnTimeOptions.Count)
            currentEnemySpawnTimeIndex = 0;

        GameManager.enemySpawnRate = _enemySpawnTimeOptions[currentEnemySpawnTimeIndex];

        UpdateEnemySpawnTimeText();
    }

    public void SubtractEnemySpawnTime()
    {
        int currentEnemySpawnTimeIndex = _enemySpawnTimeOptions.IndexOf(GameManager.enemySpawnRate);
        currentEnemySpawnTimeIndex--;
        if (currentEnemySpawnTimeIndex < 0)
            currentEnemySpawnTimeIndex = _enemySpawnTimeOptions.Count - 1;

        GameManager.enemySpawnRate = _enemySpawnTimeOptions[currentEnemySpawnTimeIndex];

        UpdateEnemySpawnTimeText();
    }

    private void UpdateEnemySpawnTimeText()
    {
        sb.Append(GameManager.enemySpawnRate);
        sb.Append("s");
        _enemySpawnTimeValueText.text = sb.ToString();
        sb.Clear();
    }
    #endregion
}
