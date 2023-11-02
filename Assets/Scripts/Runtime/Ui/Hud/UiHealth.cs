using UnityEngine;
using UnityEngine.UI;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Handles Health Ui changes
	/// </summary>
    public class UiHealth : MonoBehaviour
    {
		#region LifeCycle
		[SerializeField] private Image[] _hearts;
		[SerializeField] private float _healthPointsPerHeart;

		public void Wake()
		{
			Level.Hero.Health.OnValueChanged += UpdateHealthUi;
			UpdateHealthUi(Level.Hero.Health.Normalized);
		}
		#endregion



		#region Logic
		private void UpdateHealthUi(float normalizedHealth)
		{
			float heroHealth = normalizedHealth * Level.Hero.Health.Default;

			for (int i = 0; i < Level.Hero.Health.Default / _healthPointsPerHeart; i++)
			{
				_hearts[i].fillAmount = heroHealth;
				heroHealth -= _healthPointsPerHeart;
			}
		}
		#endregion
	}
}
