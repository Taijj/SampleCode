using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Top Level Manager for all Hud Uis
	/// </summary>
    public class UiHud : MonoBehaviour
    {
		#region LifeCycle
		[SerializeField] private UiHealth _uiHealth;
		[SerializeField] private UiMana _uiMana;
		[SerializeField] private UiOre _uiOre;

		[SerializeField] private UiSpecialSelector _uiSpecialSelector;

		public void Wake()
		{
			_uiHealth.Wake();
			_uiMana.Wake();
			_uiOre.Wake();

			_uiSpecialSelector.Wake();
		}

		public void CleanUp()
		{
			_uiSpecialSelector.CleanUp();
		}
		#endregion



		#if UNITY_EDITOR
		private void OnValidate()
		{
			_uiHealth = GetComponentInChildren<UiHealth>(true);
			_uiMana = GetComponentInChildren<UiMana>(true);
			_uiOre = GetComponentInChildren<UiOre>(true);

			_uiSpecialSelector = GetComponentInChildren<UiSpecialSelector>(true);
		}
		#endif
	}
}
