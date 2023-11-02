using UnityEngine;

/// <summary>
/// DisplayTransition that utilizes an Animator.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimatorTransition : DisplayTransition
{
    #region Main
    public static readonly int SHOW_HASH = Animator.StringToHash("Show");
    public static readonly int HIDE_HASH = Animator.StringToHash("Hide");

    [SerializeField] private Animator _animator;

    public override void Wake()
    {
        // Instantly jumps to the end of the Hide animation.
        _animator.TriggerInstantly(HIDE_HASH, out float _);
        _animator.Play(0, 0, 1f);
    }

    protected override void PlayShow() => Play(SHOW_HASH);
    protected override void PlayHide() => Play(HIDE_HASH);

    private void Play(int hash)
    {
        IsAppearing = hash == SHOW_HASH;

        _animator.TriggerInstantly(hash, out float duration);
        CompletedTime = Time.time + duration;
    }


    public void Update()
    {
        if (false == IsTransitioning)
            return;

        if (Time.time < CompletedTime)
            return;

        if (IsAppearing)
            OnShowingCompleted();
        else
            OnHidingCompleted();
    }


    private float CompletedTime { get; set; }
    private bool IsAppearing { get; set; }
    #endregion



    #if UNITY_EDITOR
    public override void OnValidate()
    {
        base.OnValidate();
        _animator = GetComponent<Animator>();
    }
    #endif
}
