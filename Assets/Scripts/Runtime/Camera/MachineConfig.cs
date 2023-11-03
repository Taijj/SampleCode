using Cinemachine;
using System;
using UnityEngine;
using Blend = Cinemachine.CinemachineBlendDefinition;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Contains a set of settings for different Camera behavior. Uses a bunch
	/// of <see cref="MachineConfig.Property"/>s. These are injections extended by an "enabled"
	/// field, to be able to only set the values a <see cref="CameraZone"/> uses.
	/// Default values are used, defined in <see cref="Cameraman"/>, for disabled properties.
	/// </summary>
	[Serializable]
	public class MachineConfig
	{
		#region Properties
		public abstract class Property
		{
			public abstract bool Enabled { get; }
			public abstract object Injection { get; }

			public T Get<T>(Property defaultProperty)
			{
				return Enabled ? (T)Injection : (T)defaultProperty.Injection;
			}
		}

		[Serializable]
		public class Property<T> : Property
		{
			[SerializeField] private bool _enabled;
			[SerializeField] private T _injection;

			public override bool Enabled => _enabled;
			public override object Injection => _injection;
		}

		[Serializable]
		public class BlendProperty : Property
		{
			[SerializeField] private bool _enabled;
			[CinemachineBlendDefinitionProperty]
			[SerializeField] private Blend _injection;

			public override bool Enabled => _enabled;
			public override object Injection => _injection;
		}
		#endregion



		#region Main
		public BlendProperty blend;
		public Property<Transform> target;
		public Property<CinemachinePath> path;
		[Space]
		public Property<Vector2> offset;
		public Property<Vector2> deadzone;
		public Property<float> lookAhead;
		public Property<bool> canLookAround;
		#endregion



		#if UNITY_EDITOR
		[Space, Header("Editor")]
		[ReadOnly] public string name;
		#endif
	}
}