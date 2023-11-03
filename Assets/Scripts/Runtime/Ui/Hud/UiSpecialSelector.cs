using UnityEngine;
using UnityEngine.UI;

namespace Taijj.SampleCode
{
	/// <summary>
	/// UI Controller that changes the UI to reflect the currently selectet Special Skill
	/// </summary>
	public class UiSpecialSelector : MonoBehaviour
	{
		#region LifeCycle
		[SerializeField] private UiSpecial[] _specials;
		[SerializeField] private HorizontalLayoutGroup _layoutGroup;

		public void Wake()
		{
			InputProvider.OnSelectSpecial += SetActiveSpecial;
			Game.Catalog.Special.OnUnlockChanged += UpdateUnlockedSkills;

			_selectedSpecial = SpecialKind.None;
			for (int i = 0; i < _specials.Length; i++)
				_specials[i].Wake();
		}

		public void CleanUp()
		{
			InputProvider.OnSelectSpecial -= SetActiveSpecial;
			Game.Catalog.Special.OnUnlockChanged -= UpdateUnlockedSkills;
		}
		#endregion



		#region Logic
		public void SetActiveSpecial(SpecialKind specialKind)
		{
			RefreshSpecials(specialKind);

			_selectedSpecial = specialKind;
			_layoutGroup.SetLayoutHorizontal();
		}

		public void UpdateUnlockedSkills()
		{
			RefreshSpecials(_selectedSpecial);
		}

		private void RefreshSpecials(SpecialKind specialKind)
		{
			for (int i = 0; i < _specials.Length; i++)
			{
				bool isSelected = _specials[i].SpecialKind == specialKind;
				_specials[i].Refresh(isSelected);
			}
		}

		private SpecialKind _selectedSpecial;
		#endregion



		#if UNITY_EDITOR
		private void OnValidate()
		{
			_layoutGroup = GetComponent<HorizontalLayoutGroup>();
			_specials = GetComponentsInChildren<UiSpecial>();
		}
		#endif
	}
}
