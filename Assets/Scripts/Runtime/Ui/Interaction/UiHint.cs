using System.Text;
using TMPro;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Helper for swapping the hint text 
	/// </summary
	public class UiHint : MonoBehaviour
	{
		#region LifeCycle
		[SerializeField] private TMP_Text _hintText;
		[SerializeField] private TMP_Text _hintPageCounter;

		[SerializeField] private Color _highlightColor;



		public void ShowHint(string hint)
		{
			_hintText.text = Parse(hint);
		}

		public void SetHintPageCounter(int currentPage, int maxPage)
		{
			_hintPageCounter.text = $"{currentPage}/{maxPage}";
		}
		#endregion



		#region
		private const string HIGHLIGHT_START_CUSTOM = "<c>";
		private const string HIGHLIGHT_END_CUSTOM = "</c>";
		private const string HIGHLIGHT_START_RICH = "<color=#{0}>";
		private const string HIGHLIGHT_END_RICH = "</color>";

		private string Parse(string rawText)
		{
			StringBuilder builder = new StringBuilder(rawText);
			builder.Replace(HIGHLIGHT_START_CUSTOM, string.Format(HIGHLIGHT_START_RICH,ColorUtility.ToHtmlStringRGBA(_highlightColor)));
			builder.Replace(HIGHLIGHT_END_CUSTOM, HIGHLIGHT_END_RICH);
			return builder.ToString();
		}
		#endregion
	}
}
