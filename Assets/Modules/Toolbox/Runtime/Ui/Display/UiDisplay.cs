using UnityEngine;

/// <summary>
/// Base for a transitioned type of UI display, fullscreen or otherwise.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class UiDisplay : MonoBehaviour
{
    #region Lifecycle
    [Space, Header("Base")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private DisplayTransition _transition;
    [SerializeField, HideInInspector] private bool _hasTransition;

    public virtual void Wake()
    {
        if(_hasTransition)
        {
            _transition.Wake();
            _transition.OnShown += OnShown;
            _transition.OnHidden += OnHidden;
        }
        IsVisible = false;
    }

    public virtual void CleanUp()
    {
        if (false == _hasTransition)
            return;

        _transition.CleanUp();
        _transition.OnShown -= OnShown;
        _transition.OnHidden -= OnHidden;
    }
    #endregion




    #region Visibility
    public virtual void Show()
    {
        IsVisible = true;
        if (_hasTransition)
            _transition.Show();
        else
            OnShown();
    }

    protected virtual void OnShown() {}

    public virtual void Hide()
    {
        if (_hasTransition)
            _transition.Hide();
        else
            OnHidden();
    }

    protected virtual void OnHidden() => IsVisible = false;



    // NOTE: Dis/Enabling the Canvas, instead of the entire GameObject,
    // saves performance and creates significantly less garbage!
    public bool IsVisible
    {
        get => _canvas.enabled;
        set => _canvas.enabled = value;
    }

    public bool IsInTransition => _hasTransition ? _transition.IsTransitioning : false;
    #endregion



    #if UNITY_EDITOR
    public void OnValidate()
    {
        _canvas = GetComponent<Canvas>();
        _transition = GetComponent<DisplayTransition>();
        _hasTransition = _transition != null;
    }
    #endif
}