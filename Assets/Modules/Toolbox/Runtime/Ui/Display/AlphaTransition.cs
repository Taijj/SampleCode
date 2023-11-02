
using UnityEngine;

/// <summary>
/// Display Transition that animates a CanvasGroup's alpha value over time.
///
/// Note: Also handles transitioning between intermediate states, e.g. if
/// a transition is started while another is still in progress.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class AlphaTransition : DisplayTransition
{
    #region Start
    [Tooltip("The Duration of the transition animation in seconds.")]
    [SerializeField, Range(0f, 10f)] private float _duration = 1f;
    [SerializeField] private CanvasGroup _canvasGroup;

    public override void Wake() => _canvasGroup.alpha = 0f;

    protected override void PlayShow()
    {
        IsAppearing = true;
        UpdateTimes();
    }

    protected override void PlayHide()
    {
        IsAppearing = false;
        UpdateTimes();
    }

    private void UpdateTimes()
    {
        _canvasGroup.alpha = Mathf.Clamp01(_canvasGroup.alpha);
        StartAlpha = _canvasGroup.alpha;

        CurrentDuration = IsAppearing
            ? _duration * (1f - _canvasGroup.alpha)
            : _duration * _canvasGroup.alpha;
        CompletedTime = Time.time + CurrentDuration;
    }

    private float CurrentDuration { get; set; }
    private float CompletedTime { get; set; }
    private float StartAlpha { get; set; }

    private bool IsAppearing { get; set; }
    #endregion



    #region Transition
    public void Update()
    {
        if (false == IsTransitioning)
            return;

        if (CurrentDuration <= 0f)
        {
            Complete();
            return;
        }

        float parameter = 1f - (CompletedTime-Time.time) / CurrentDuration;
        if (IsAppearing)
            _canvasGroup.alpha = Easing.EaseInSine(StartAlpha, 1f, parameter);
        else
            _canvasGroup.alpha = Easing.EaseOutSine(StartAlpha, 0f, parameter);

        if (parameter >= 1f)
            Complete();
    }

    private void Complete()
    {
        if (IsAppearing)
        {
            _canvasGroup.alpha = 1f;
            OnShowingCompleted();
        }
        else
        {
            _canvasGroup.alpha = 0f;
            OnHidingCompleted();
        }
    }
    #endregion



    #if UNITY_EDITOR
    public override void OnValidate()
    {
        base.OnValidate();
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    #endif
}