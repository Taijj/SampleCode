using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// The Inspector of Unity Animation Events cannot handle FMOD <see cref="EventReference"/>s.
	/// This Wrapper wraps around the FMOD reference and can be injected into the Animation Event
	/// instead.
	/// </summary>
	[CreateAssetMenu(fileName = "NewAudio", menuName = "Ble/AnimationSound")]
	public class AnimationAudio : ScriptableObject
	{
		[field: SerializeField] public EventReference Event { get; private set; }
	}
}