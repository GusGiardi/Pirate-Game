using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class GameEndScreen : MonoBehaviour
{
    [Header("Finish")]
    [SerializeField] GameObject _finishScreen;
    [SerializeField] float _timeToShowResults = 2;
    private float _showResultsTimeCounter;

    [Header("Results")]
    [SerializeField] GameObject _resultScreen;
    [SerializeField] TextMeshProUGUI _resultText;
    [SerializeField] TextMeshProUGUI _scoreText;

    [SerializeField] string _alivePlayerResultText;
    [SerializeField] string _deadPlayerResultText;

    [SerializeField] string _mainMenuScene;

    private StringBuilder sb = new StringBuilder();

    private void Update()
    {
        if (_showResultsTimeCounter > 0)
        {
            _showResultsTimeCounter -= Time.unscaledDeltaTime;
            if (_showResultsTimeCounter <= 0)
            {
                ShowResults();
            }
        }
    }

    public void CallGameEndScreen()
    {
        _resultText.text = GameManager.instance.playerShip.alive ?
            _alivePlayerResultText
            :
            _deadPlayerResultText;

        sb.Append("Score: ");
        sb.Append(GameManager.instance.playerScore);
        _scoreText.text = sb.ToString();
        sb.Clear();

        _finishScreen.SetActive(true);
        _resultScreen.SetActive(false);
        gameObject.SetActive(true);

        _showResultsTimeCounter = _timeToShowResults;
    }

    private void ShowResults()
    {
        _finishScreen.SetActive(false);
        _resultScreen.SetActive(true);
    }

    public void PlayAgainButton()
    {
        GameManager.instance.StartCoroutine(PlayAgainRoutine());
    }

    private IEnumerator PlayAgainRoutine()
    {
        yield return GameManager.instance.StartCoroutine(GameManager.instance.DiscardLastGame());
        gameObject.SetActive(false);
        yield return GameManager.instance.StartCoroutine(GameManager.instance.StartNewGame());
    }

    public void MainMenuButton()
    {
        GameManager.instance.StartCoroutine(GameManager.instance.ReturnToMainMenu(_mainMenuScene));
    }
}
