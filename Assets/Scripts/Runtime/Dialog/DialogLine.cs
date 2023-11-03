using UnityEngine;

namespace Taijj.SampleCode
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
