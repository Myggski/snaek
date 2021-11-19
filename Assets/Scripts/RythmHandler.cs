using System;
using UnityEngine;

public class RythmHandler : SingletonBase<RythmHandler>
{
    [SerializeField]
    [Tooltip("How long until it moves to new position. In seconds")]
    private float updateFrequency = .5f;
    
    private float _currentTime = 0;
    
    public static event Action RythmEvent;

    /// <summary>
    /// Triggers RythmActivated event to let other things know when it's time to update position and such
    /// </summary>
    private void TryTriggerEvent()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= updateFrequency)
        {
            RythmEvent?.Invoke();
            _currentTime = 0;
        }
    }

    /// <summary>
    /// Sets the event to null when destroyed
    /// </summary>
    private void ClearEvent() {
        RythmEvent = null;
    }
    
    private void Update()
    {
        TryTriggerEvent();
    }

    protected override void OnDestroy() {
        base.OnDestroy();

        ClearEvent();
    }
}
