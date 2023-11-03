using System;
using UnityEngine;
using UnityEngine.Events;

namespace Taijj.SampleCode
{
	[Serializable]
	public class AnimatorEvent : UnityEvent {}


	/// <summary>
	/// Helper for Relaying AnimatorEvents to another object.
	/// </summary>
	public class AnimatorEventRelay : MonoBehaviour
    {
		[SerializeField] private AnimatorEvent _event;
		public void OnAnimationEvent() => _event.Invoke();
    }
}