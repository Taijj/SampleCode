using FMODUnity;
using System;
using UnityEngine;
using Parameter = UnityEngine.AnimatorControllerParameter;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Wrapper for Unity's Mecanim <see cref="Animator"/> system.
	/// </summary>
	[RequireComponent(typeof(Animator))]
    public class CharacterAnimator : MonoBehaviour
    {
		#region LifeCycle
		[SerializeField] private Animator _animator;

		public void Wake()
		{
			AudioTransform = transform;
			Parameters = _animator.parameters;
			IsReady = true;
		}

		public void ResetSelf()
		{
			_animator.ResetParameters(Parameters);
			_animator.Reset();
		}

		private Transform AudioTransform { get; set; }
		private Parameter[] Parameters { get; set; }

		private bool IsReady { get; set; }
		#endregion



		#region Triggering
		public void Trigger(int hash, bool resetParameters = true)
		{
			if(resetParameters)
				ResetTriggers(Parameters);
			_animator.SetTrigger(hash);
		}

		public void Set(int hash, bool value) => _animator.SetBool(hash, value);
		public void Set(int hash, int value) => _animator.SetInteger(hash, value);

		public void ModifySpeed(float mulitplier)
		{
			_animator.GetCurrentAnimatorStateInfo(0);
			_animator.speed = mulitplier;
		}

		public void ResetTriggers(Parameter[] parameters)
		{
			foreach (Parameter param in parameters)
			{
				if (param.type == AnimatorControllerParameterType.Trigger)
					_animator.ResetTrigger(param.nameHash);
			}
		}
		#endregion



		#region Events
		public enum Event
		{
			None = 0,

			Default
		}

		// Unity Event
		public void OnSoundEvent(AnimationAudio audio)
		{
			if(IsReady)
				Game.Audio.Play(audio.Event, AudioTransform);
		}

		// Unity Event
		public void OnAnimationEvent(Event e) => OnEvent?.Invoke(e);

		public event Action<Event> OnEvent;
		#endregion



		#if UNITY_EDITOR
		public void OnValidate() => this.TryAssign(ref _animator);
		#endif
	}
}