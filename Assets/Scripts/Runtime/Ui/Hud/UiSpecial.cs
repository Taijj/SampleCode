using UnityEngine;
using UnityEngine.UI;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Special skill UI that shows its according Skill and locked/active state
	/// </summary>
	public class UiSpecial : MonoBehaviour
	{
		#region LifeCycle
		[field: SerializeField] public SpecialKind SpecialKind { get; private set; }

		[Header("References")]
		[SerializeField] private Image _skillImage;
		[SerializeField] private Image _backGroundImage;
		[SerializeField] private RectTransform _rectTransform;

		[Header("Visuals")]
		[SerializeField] private Sprite _selectedSkillIcon;
		[SerializeField] private Sprite _deselectedSkillIcon;
		[SerializeField] private Sprite _selectedBackGround;
		[SerializeField] private Sprite _deselectedBackGround;

		[SerializeField] private float _selectedScale;
		[SerializeField] private float _deselectedScale;

		private float _standardHeight;
		private float _standardWidth;

		private Vector2 SelectedSize => new Vector2(_standardWidth * _selectedScale, _standardHeight * _selectedScale);
		private Vector2 DeselectedSize => new Vector2(_standardWidth * _deselectedScale, _standardHeight * _deselectedScale);

		private Special.Skill _skill;

		public void Wake()
		{
			_standardHeight = _rectTransform.rect.height;
			_standardWidth = _rectTransform.rect.width;

			_skill = Game.Catalog.Special.GetSkill(SpecialKind);
			this.Deactivate();
		}
		#endregion



		#region Logic
		public void Refresh(bool isSelected)
		{
			this.Activate();
			if(_skill.isUnlocked == false)
				SetLocked();
			else
				SetSelected(isSelected);
		}

		private void SetLocked()
		{
			_skillImage.enabled = false;
			_rectTransform.sizeDelta = DeselectedSize;
		}

		private void SetSelected(bool isSelected)
		{
			_skillImage.enabled = true;
			_skillImage.sprite = isSelected ? _selectedSkillIcon : _deselectedSkillIcon;
			_backGroundImage.sprite = isSelected ? _selectedBackGround : _deselectedBackGround;
			_rectTransform.sizeDelta = isSelected ? SelectedSize : DeselectedSize;
		}
		#endregion



		#if UNITY_EDITOR
		private void OnValidate()
		{
			_rectTransform = GetComponent<RectTransform>();
		}
		#endif
	}
}
