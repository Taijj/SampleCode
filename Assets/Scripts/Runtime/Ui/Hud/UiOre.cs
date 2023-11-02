using TMPro;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Handles Ore Ui changes
	/// </summary>
	public class UiOre : MonoBehaviour
	{
		[SerializeField] private TMP_Text _bronzeText;
		[SerializeField] private TMP_Text _silverText;
		[SerializeField] private TMP_Text _goldText;

		public void Wake()
		{
			Game.Catalog.Treasure.OnTreasureChanged += UpdateOreUi;
			UpdateOreUi();
		}

		private void UpdateOreUi()
		{
			_bronzeText.text = $"x{Game.Catalog.Treasure.oreBronze}";
			_silverText.text = $"x{Game.Catalog.Treasure.oreSilver}";
			_goldText.text = $"x{Game.Catalog.Treasure.oreGold}";
		}
	}
}
