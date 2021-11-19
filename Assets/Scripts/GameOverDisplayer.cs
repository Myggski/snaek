using TMPro;
using UnityEngine;

public class GameOverDisplayer : MonoBehaviour
{
    private TextMeshProUGUI _gameOverText;

    /// <summary>
    /// Setting the Textmesh pro component and listens to GameOverEvent
    /// </summary>
    private void Setup()
    {
        _gameOverText = GetComponentInChildren<TextMeshProUGUI>();
        _gameOverText.enabled = false;

        GameBoard.GameOverEvent += DisplayGameOverText;
    }

    /// <summary>
    /// Displays the game over text if GameOverEvent triggers
    /// </summary>
    private void DisplayGameOverText()
    {
        _gameOverText.enabled = true;
    }

    private void Start()
    {
        Setup();
    }
}
