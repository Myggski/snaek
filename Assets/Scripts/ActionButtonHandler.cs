using System;
using AStar;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ActionButtonHandler : MonoBehaviour
{
    [SerializeField]
    private string resumeText = "RESUME";
    [SerializeField]
    private string retryText = "RETRY";
    [SerializeField]
    private string pauseText = "PAUSE";

    private Button _actionButton;
    private TextMeshProUGUI _actionButtonText;
    

    /// <summary>
    /// Setting up the button and adding click event
    /// </summary>
    private void Setup() {
        _actionButton = GetComponent<Button>();
        _actionButtonText = _actionButton.GetComponentInChildren<TextMeshProUGUI>();
        _actionButton.onClick.AddListener(OnClickActionButton);
        
        GameBoard.GameOverEvent += (() => ChangeActionButtonText(retryText));
        ChangeActionButtonText(pauseText);
    }

    /// <summary>
    /// Click-event depending on the state of the game
    /// If the game is still going, it's a pause/resume-button
    /// If the game is over, it's a retry-button
    /// </summary>
    private void OnClickActionButton() {
        if (GameBoard.IsGameOver)
        {
            SceneManager.LoadScene(0);
            return;
        }

        GameBoard.TogglePause();
        ChangeActionButtonText(GameBoard.IsGamePaused ? resumeText : pauseText);
    }

    /// <summary>
    /// Changes the text on the button
    /// </summary>
    /// <param name="buttonText">The new text that the button will get</param>
    private void ChangeActionButtonText(string buttonText)
    {
        if (ReferenceEquals(_actionButtonText, null))
        {
            return;
        }
        
        _actionButtonText.text = buttonText;
    }

    private void Start()
    {
        Setup();
    }
}
