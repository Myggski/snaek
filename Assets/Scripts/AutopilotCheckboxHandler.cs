using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class AutopilotCheckboxHandler : MonoBehaviour
{
    private static Toggle _toggle;

    public static bool IsOn => _toggle.isOn;

    /// <summary>
    /// Setting up toggle component
    /// </summary>
    private void Setup()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(GameBoard.SetAutopilot);
    }

    /// <summary>
    /// Removes checkbox listener
    /// </summary>
    private void ClearListener()
    {
        _toggle.onValueChanged.RemoveListener(GameBoard.SetAutopilot);
    }

    private void Start()
    {
        Setup();
    }

    private void OnDestroy()
    {
        ClearListener();
    }
}