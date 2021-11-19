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

    private void Start()
    {
        Setup();
    }
}