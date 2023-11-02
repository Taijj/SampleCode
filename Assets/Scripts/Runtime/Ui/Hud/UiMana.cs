using UnityEngine;
using UnityEngine.UI;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Handles Mana Ui changes
	/// </summary>
    public class UiMana : MonoBehaviour
    {
		#region LifeCycle
		[SerializeField] private Image _manaFill;

		public void Wake()
		{
			Level.Hero.Mana.OnValueChanged += UpdateManaUi;
			UpdateManaUi(Level.Hero.Mana.Normalized);
		}
		#endregion

		#region Logic
		private void UpdateManaUi(float mana)
		{
			_manaFill.fillAmount = mana;
		}
		#endregion
	}
}
