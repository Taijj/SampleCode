using System;
using UnityEngine;

/// <summary>
/// Base class for a controller to handle in/out transitions of a <see cref="UiDisplay"/>.
/// </summary>
[RequireComponent(typeof(UiDisplay))]
public abstract class DisplayTransition : MonoBehaviour
{
    #region Lifecycle
    public virtual void Wake() {}
    public virtual void CleanUp() {}

    public bool IsTransitioning { get; protected set; }
    #endregion



    #region Show
    public void Show()
    {
        PlayShow();
        IsTransitioning = true;
    }

    protected virtual void PlayShow() => OnShowingCompleted();
    protected void OnShowingCompleted()
    {
        IsTransitioning = false;
        OnShown?.Invoke();
    }

    public event Action OnShown;
    #endregion



    #region Hide
    public void Hide()
    {
        PlayHide();
        IsTransitioning = true;
    }
    protected virtual void PlayHide() => OnHidingCompleted();

    protected void OnHidingCompleted()
    {
        IsTransitioning = false;
        OnHidden?.Invoke();
    }

    public event Action OnHidden;
    #endregion



    #if UNITY_EDITOR
    public virtual void OnValidate() => GetComponent<UiDisplay>().OnValidate();
    #endif
}