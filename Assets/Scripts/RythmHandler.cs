using System;
using UnityEngine;

public class RythmHandler : SingletonBase<RythmHandler>
{
    [SerializeField]
    [Tooltip("How long until it moves to new position. In seconds")]
    private float updateFrequency = .5f;
    
    private float _currentTime = 0;
    
    public static event Action OnRythmPlay;

    /// <summary>
    /// Triggers RythmActivated event to let other things know when it's time to update position and such
    /// </summary>
    private void TryTriggerEvent()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= updateFrequency)
        {
            OnRythmPlay?.Invoke();
            _currentTime = 0;
        }
    }
    
    private void Update()
    {
        TryTriggerEvent();
    }
}
