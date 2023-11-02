using UnityEngine;

/// <summary>
/// Assigns a random Sprite from the given array to a SpriteRenderer in OnEnable.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRandomizer : MonoBehaviour
{
    #region Default
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite[] _sprites;

    public void OnEnable() => _renderer.sprite = _sprites.Random();
    #endregion



#if UNITY_EDITOR
    public void OnValidate()
    {
        if (_renderer == null)
            _renderer = GetComponent<SpriteRenderer>();
    }
#endif
}