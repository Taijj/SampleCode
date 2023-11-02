using Cinemachine;
using System;
using UnityEngine;
using Impulse = Cinemachine.CinemachineImpulseDefinition;
using Envelope = Cinemachine.CinemachineImpulseManager.EnvelopeDefinition;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Handles Camera shaking.
	/// </summary>
	public class CameraShake : MonoBehaviour
	{
		#region LifeCycle
		[Serializable]
		public class Config
		{
			public float strength;
			public float ferocity;
			public float duration;
			public AnimationCurve curve;
		}

		[SerializeField] private CinemachineImpulseSource _source;
		[SerializeField] private Config _weak;
		[SerializeField] private Config _normal;
		[SerializeField] private Config _strong;

		public void Wake()
		{
			Weak = _weak;
			Normal = _normal;
			Strong = _strong;

			Impulse = _source.m_ImpulseDefinition;
			_envelope = Impulse.m_TimeEnvelope;
		}

		public static Config Weak { get; private set; }
		public static Config Normal { get; private set; }
		public static Config Strong { get; private set; }
		#endregion



		#region Main
		public void Do(Config config)
		{
			_envelope.m_DecayTime = config.duration;
			_envelope.m_DecayShape = config.curve;

			Impulse.m_TimeEnvelope = _envelope;
			Impulse.m_AmplitudeGain = config.strength;
			Impulse.m_FrequencyGain = config.ferocity;

			_source.GenerateImpulse();
		}

		private Impulse Impulse { get; set; }
		private Envelope _envelope;
		#endregion



		#if UNITY_EDITOR
		[Button(nameof(ShakeWeak))]
		public bool shakeWeak;
		public void ShakeWeak() => Do(_weak);

		[Button(nameof(ShakeNormal))]
		public bool shakeNormal;
		public void ShakeNormal() => Do(_normal);

		[Button(nameof(ShakeStrong))]
		public bool shakeStrong;
		public void ShakeStrong() => Do(_strong);
		#endif
	}
}
