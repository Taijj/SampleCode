using UnityEngine;
using UnityEngine.U2D.Animation;

#if UNITY_EDITOR
using System.Linq;
#endif

/// <summary>
/// Optimization Component for Skinned 2D Characters.
///
/// SpriteSkin OnEnable/Disable generates a lot of garbage. This component
/// enables/disables everything except the SpriteSkins, relative to the
/// distance to the Camera.
/// </summary>
public class CharacterSkin : MonoBehaviour
{
    #region Main
    [SerializeField] private SpriteRenderer[] _renderers;
    [SerializeField] private SpriteSkin[] _skins;
    [SerializeField] private Behaviour[] _behaviors;

	public void Activate()
	{
		IsActive = true;
		UpdateComponents();
	}

	public void Deactivate()
	{
		IsActive = false;
		UpdateComponents();
	}

	private void UpdateComponents()
	{
		for (int i = 0; i < _renderers.Length; i++)
			_renderers[i].enabled = IsActive;

		for (int i = 0; i < _behaviors.Length; i++)
			_behaviors[i].enabled = IsActive;

		for (int i = 0; i < _skins.Length; i++)
			_skins[i].alwaysUpdate = IsActive;
	}

	public bool IsActive { get; private set; }
    #endregion



    #if UNITY_EDITOR
    public void OnValidate()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>(true);
        _skins = GetComponentsInChildren<SpriteSkin>(true);
        _behaviors = GetComponentsInChildren<Behaviour>(true)
            .Except(_skins)
            .Where(b => b != this)
            .ToArray();
    }
    #endif
}