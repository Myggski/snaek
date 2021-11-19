using System;
using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour
{
    protected static T instance;
    
    /// <summary>
    /// Making sure that there's only one of this component
    /// </summary>
    private void InitializeManager() {
        if (!ReferenceEquals(instance, null) && !ReferenceEquals(instance, this)) {
            Destroy(gameObject);
        }
        else {
            instance = (T)Convert.ChangeType(this, typeof(T));
        }
    }
    
    /// <summary>
    /// Cleanup the instance when destroyed
    /// </summary>
    private void RemoveInstance() {
        if (ReferenceEquals((T) Convert.ChangeType(this, typeof(T)), instance)) {
            instance = default;
        }
    }
        
    protected virtual void Awake() {
        InitializeManager();
    }
        
    protected virtual void OnDestroy() {
        RemoveInstance();
    }
}
