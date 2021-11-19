using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreDisplayer : MonoBehaviour
{
    private TextMeshProUGUI _scoreText;

    /// <summary>
    /// Listening to score event to know when the score should update
    /// </summary>
    private void Setup() {
        _scoreText = GetComponent<TextMeshProUGUI>();

        SnakeHead.OnSnakeScore += UpdateEventScore;
    }

    /// <summary>
    /// When ScoreUpdateEvent triggers, it updates the score text
    /// </summary>
    private void UpdateEventScore(int totalScore) {
        _scoreText.text = totalScore.ToString();
    }

    private void Start() {
        Setup();
    }
}
