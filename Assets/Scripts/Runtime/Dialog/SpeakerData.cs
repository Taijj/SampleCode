using UnityEngine;

namespace Taijj.SampleCode
{
	public enum Character
	{
		Lynx,
		Penguin
	}

	[CreateAssetMenu(fileName = "NewSpeakerData", menuName = "Ble/Dialog/SpeakerData")]
	public class SpeakerData : ScriptableObject
	{
		public Character character;
		public Sprite portrait;
		public Sprite labelBg;
		public Sprite textBoxBg;
	}
}
