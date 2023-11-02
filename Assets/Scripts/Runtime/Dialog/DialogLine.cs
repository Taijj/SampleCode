using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Helper class to hold data for a line of dialog
	/// </summary>
	[System.Serializable]
    public class DialogLine
    {
		public SpeakerData speakerData;
		public Sprite speakerPortraitOverride;
		[TextArea] public string dialogLine;
    }
}
