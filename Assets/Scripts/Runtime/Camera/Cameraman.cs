using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using System;
using Machine = Cinemachine.CinemachineVirtualCamera;
using Blend = Cinemachine.CinemachineBlendDefinition;

#if UNITY_EDITOR
using System.Reflection;
#endif

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Main controller for the game's camera and Unity's Cinemachine
	/// components.
	/// </summary>
	public class Cameraman : MonoBehaviour
	{
		#region LifeCycle
		[Header("General")]
		[SerializeField] private Camera _camera;
		[SerializeField, ReadOnly] private CinemachineBrain _brain;
		[SerializeField] private CameraShake _shake;
		[Space, Header("Machines")]
		[SerializeField] private Machine _mainTransposer;
		[SerializeField, ReadOnly] private Machine _altTransposer;
		[SerializeField] private Machine _mainDolly;
		[SerializeField, ReadOnly] private Machine _altDolly;
		[SerializeField] private MachineConfig _defaultConfig;
		[SerializeField, ReadOnly] private MachineConfig _currentConfig;
		[Space, Header("Look Around")]
		[SerializeField] private Machine _lookTransposer;
		[SerializeField] private float _lookDistance;

		public void Wake()
		{
			WakeMachines();
			_shake.Wake();

			BrainTransform = _brain.transform;
			Configs = new List<MachineConfig>(CONFIGS_CAPACITY);
			Configs.Add(_defaultConfig);
			_currentConfig = _defaultConfig;
			SwitchCamera(new Blend(Blend.Style.Cut, 0f));

			Game.Updater.AddUpdate(OnUpdate);
		}

		private void OnUpdate() => UpdateLookAround();

		public void CleanUp() => Game.Updater.RemoveUpdate(OnUpdate);

		private const int CONFIGS_CAPACITY = 5;
		public Camera Camera => _camera;
		public CameraShake Shake => _shake;
		#endregion



		#region Zoning
		public void Add(MachineConfig config)
		{
			Configs.Add(config);
			_currentConfig = Configs.Last();

			SwitchCamera(_currentConfig.blend.Get<Blend>(_defaultConfig.blend));
		}

		public void Remove(MachineConfig config)
		{
			MachineConfig prevConfig = _currentConfig;
			Configs.Remove(config);
			_currentConfig = Configs.Last();

			if (prevConfig != _currentConfig)
				SwitchCamera(prevConfig.blend.Get<Blend>(_defaultConfig.blend));
		}

		private List<MachineConfig> Configs { get; set; }

		#endregion



		#region Switch
		private void WakeMachines()
		{
			Transform container = _mainTransposer.transform.parent;
			_altTransposer = Instantiate(_mainTransposer, container);
			_altDolly = Instantiate(_mainDolly, container);

			#if UNITY_EDITOR
				_altTransposer.name = "AltTransposer";
				_altDolly.name = "AltDolly";
			#endif
		}

		private void SwitchCamera(Blend blend)
		{
			_brain.m_DefaultBlend = blend;
			Target = _currentConfig.target.Get<Transform>(_defaultConfig.target);
			Offset = _currentConfig.offset.Get<Vector2>(_defaultConfig.offset);
			CanLookAround = _currentConfig.canLookAround.Get<bool>(_defaultConfig.canLookAround);

			Vector3 pos = CurrentMachine == null ? Target.position : CurrentMachine.transform.position;

			bool isDolly = _currentConfig.path.Enabled && _currentConfig.path.Injection != null;
			if (isDolly)
				SwitchToDolly();
			else
				SwitchToTransposer();

			CurrentMachine.Follow = Target;
			CurrentMachineTransform = CurrentMachine.transform;
			ActivateCurrentMachine();
		}

		private void ActivateCurrentMachine()
		{
			_mainTransposer.Priority = 0;
			_altTransposer.Priority = 0;
			_mainDolly.Priority = 0;
			_altDolly.Priority = 0;

			// This prevents weird jumping when switching between different Cameras.
			CurrentMachine.Deactivate();
			CurrentMachineTransform.Activate();
			CurrentMachine.Priority = 1;
		}

		private void SwitchToDolly()
		{
			CurrentMachine = CurrentMachine == _mainDolly ? _altDolly : _mainDolly;
			CinemachineTrackedDolly dolly = CurrentMachine.GetCinemachineComponent<CinemachineTrackedDolly>();
			dolly.m_Path = _currentConfig.path.Get<CinemachinePath>(_defaultConfig.path);
		}

		private void SwitchToTransposer()
		{
			CurrentMachine = CurrentMachine == _mainTransposer ? _altTransposer : _mainTransposer;

			CinemachineFramingTransposer poser = CurrentMachine.GetCinemachineComponent<CinemachineFramingTransposer>();
			poser.m_TrackedObjectOffset = Offset;

			Vector2 deadzone = _currentConfig.deadzone.Get<Vector2>(_defaultConfig.deadzone);
			poser.m_DeadZoneWidth = deadzone.x;
			poser.m_DeadZoneHeight = deadzone.y;
			poser.m_LookaheadTime = _currentConfig.lookAhead.Get<float>(_defaultConfig.lookAhead);
		}



		private Machine CurrentMachine { get; set; }
		private Transform CurrentMachineTransform { get; set; }
		private Transform BrainTransform { get; set; }

		private Transform Target { get; set; }
		private Vector2 Offset { get; set; }
		private bool CanLookAround { get; set; }
		#endregion



		#region Look Around
		private void UpdateLookAround()
		{
			Vector2 machinePos = CurrentMachineTransform.position;
			if(CanLookAround && LookInput != Vector2.zero)
			{
				Vector2 lookPos = (Vector2)Target.position + LookInput*_lookDistance;
				_lookTransposer.Follow.position = lookPos;
				_lookTransposer.Priority = 2;
			}
			else
			{
				_lookTransposer.Follow.position = machinePos;
				_lookTransposer.Priority = 0;
			}
		}

		public Vector2 LookInput { set; private get; }
		#endregion



		#if UNITY_EDITOR
		[Space, Header("Editor")]
		[SerializeField] private MachineConfig _dummyConfig;
		[Button(nameof(ToggleDummyConfig))] public bool toggleDummyButton;
		public void ToggleDummyConfig()
		{
			if (Configs.Contains(_dummyConfig))
				Remove(_dummyConfig);
			else
				Add(_dummyConfig);
		}



		public void OnValidate()
		{
			this.TryAssign(ref _camera);
			this.TryAssign(ref _brain);
			this.TryAssign(ref _shake);

			ValidateDefaultConfig();
		}

		/// <summary>
		/// Enables all the fields of _defaultConfig, and assigns the Hero Pawn's transform
		/// as default follow target.
		/// </summary>
		private void ValidateDefaultConfig()
		{
			if (_defaultConfig == null)
				_defaultConfig = new MachineConfig();

			Type type = _defaultConfig.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo field = fields[i];
				if (false == field.FieldType.IsSubclassOf(typeof(MachineConfig.Property)))
					continue;

				object value = field.GetValue(_defaultConfig);
				if (value == null)
				{
					value = Activator.CreateInstance(field.FieldType);
					field.SetValue(_defaultConfig, value);
				}

				FieldInfo[] subfields = value.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
				subfields[0].SetValue(value, true); // enabled

				if (subfields[1].FieldType == typeof(Transform)) // target
				{
					Hero hero = FindObjectOfType<Hero>();
					if (hero != null)
						subfields[1].SetValue(value, hero.CameraAnchor); // injection
				}
			}

			_defaultConfig.name = "Default";
			_dummyConfig.name = "Dummy";
		}
		#endif
	}
}
