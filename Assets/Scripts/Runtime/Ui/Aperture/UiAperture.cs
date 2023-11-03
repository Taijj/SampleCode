using UnityEngine;
using UnityEngine.UI;

namespace Taijj.SampleCode
{
	/// <summary>
	/// A simple full screen overlay for different screen fading
	/// and flashing effects. (Inspired by Film/Photo Camera apertures)
	/// </summary>
	public class UiAperture : MonoBehaviour
	{
		#region Initialization
		[SerializeField] private Image _defaultFaceplate;
		[SerializeField] private Image _additiveFaceplate;

		public void Wake()
		{
			_defaultFaceplate.gameObject.SetActive(true);
			_additiveFaceplate.gameObject.SetActive(true);
			_defaultFaceplate.color = Color.black.With(0f);
			_additiveFaceplate.color = Color.white.With(0f);

			Fade = new ApertureFade(_defaultFaceplate, _additiveFaceplate);
			Flash = new ApertureFlash(_additiveFaceplate);
		}

		public void Stop()
		{
			Fade.Stop();
			Flash.Stop();
		}

		public void CleanUp()
		{
			_defaultFaceplate.gameObject.SetActive(false);
			_additiveFaceplate.gameObject.SetActive(false);

			Fade.CleanUp();
			Flash.CleanUp();
		}

		public ApertureFade Fade { get; private set; }
		public ApertureFlash Flash { get; private set; }
		#endregion
	}
}