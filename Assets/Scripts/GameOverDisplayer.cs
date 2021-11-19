using System;
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

        GameBoard.OnGameOver += DisplayOnGameOverText;
    }

    /// <summary>
    /// Removes itself from OnGameOver event
    /// </summary>
    private void ClearEvent()
    {
        GameBoard.OnGameOver -= DisplayOnGameOverText;
    }

    /// <summary>
    /// Displays the game over text if GameOverEvent triggers
    /// </summary>
    private void DisplayOnGameOverText()
    {
        _gameOverText.enabled = true;
    }

    private void Start()
    {
        Setup();
    }

    private void OnDestroy()
    {
        ClearEvent();
    }
}
