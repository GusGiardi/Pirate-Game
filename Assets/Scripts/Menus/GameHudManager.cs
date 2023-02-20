using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class GameHudManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _gameTimeText;
    [SerializeField] TextMeshProUGUI _scoreText;

    private StringBuilder sb = new StringBuilder();

    private void Update()
    {
        int minutes = Mathf.FloorToInt(GameManager.instance.gameTimeCounter / 60);
        int seconds = (int)(GameManager.instance.gameTimeCounter - minutes * 60);

        sb.Append(minutes.ToString("00"));
        sb.Append(":");
        sb.Append(seconds.ToString("00"));
        _gameTimeText.text = sb.ToString();
        sb.Clear();

        _scoreText.text = GameManager.instance.playerScore.ToString();
    }
}
