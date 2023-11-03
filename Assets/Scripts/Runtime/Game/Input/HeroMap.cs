using Taijj.Input;
using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Taijj.SampleCode
{
	[CreateAssetMenu(fileName = "HeroMap", menuName = "Ble/Input/Maps/Hero")]
	public class HeroMap : ActionMap<InputActions>
	{
		#region Lifecycle
		public override void Wake(object actions)
		{
			base.Wake(actions);
			Actions = Base.Hero;
			AddListeners();
		}

		public override void CleanUp() => RemoveListeners();

		public override bool IsEnabled
		{
			get => Actions.enabled;

			set
			{
				if (value)
					Actions.Enable();
				else
					Actions.Disable();
			}
		}

		private InputActions.HeroActions Actions { get; set; }
		#endregion



		#region Fields
		public Vector2 MoveValue => Actions.Move.ReadValue<Vector2>();
		public Vector2 SelectSpecialValue => Actions.SelectSpecial.ReadValue<Vector2>();

		public Vector2 LookValue => Actions.Look.ReadValue<Vector2>();

		public bool IsBreathingFire => Actions.BreathFire.IsPressed();
		public bool IsBreathingIce => Actions.BreathIce.IsPressed();
		public bool IsStandingBreath => Actions.Stand.IsPressed();
		#endregion



		#region Events
		public event Action OnJumpPress;
		public event Action OnJumpRelease;

		public event Action OnShootFire;
		public event Action OnShootIce;
		public event Action OnSpecial;
		public event Action<SpecialKind> OnSelectSpecial;

		public event Action OnPause;

		public event Action<DevCheat> OnCheat;

		private void AddListeners()
		{
			Actions.Jump.started += OnJumpPressInput;
			Actions.Jump.canceled += OnJumpReleaseInput;

			Actions.ShootFire.performed += OnShootFireInput;
			Actions.ShootIce.performed += OnShootIceInput;
			Actions.Special.performed += OnSpecialInput;
			Actions.SelectSpecial.performed += OnSelectSpecialInput;

			Actions.Pause.performed += OnPauseInput;
			
			if (Game.IsDebugMode)
				Actions.Cheat.performed += OnCheatInput;
		}

		private void RemoveListeners()
		{
			Actions.Jump.started -= OnJumpPressInput;
			Actions.Jump.canceled -= OnJumpReleaseInput;

			Actions.ShootFire.performed -= OnShootFireInput;
			Actions.ShootIce.performed -= OnShootIceInput;
			Actions.Special.performed -= OnSpecialInput;
			Actions.SelectSpecial.performed -= OnSelectSpecialInput;

			Actions.Pause.performed -= OnPauseInput;

			if (Game.IsDebugMode)
				Actions.Cheat.performed -= OnCheatInput;
		}

		private void OnJumpPressInput(CallbackContext _) => OnJumpPress?.Invoke();
		private void OnJumpReleaseInput(CallbackContext _) => OnJumpRelease?.Invoke();

		private void OnShootFireInput(CallbackContext _) => OnShootFire?.Invoke();
		private void OnShootIceInput(CallbackContext _) => OnShootIce?.Invoke();
		private void OnSpecialInput(CallbackContext _) => OnSpecial?.Invoke();
		private void OnSelectSpecialInput(CallbackContext context)
		{
			Vector2 selection = context.ReadValue<Vector2>();
			SpecialKind skillKind = SpecialKind.None;

			if (selection.x < 0)
				skillKind = SpecialKind.IceBomb;
			else if (selection.y > 0)
				skillKind = SpecialKind.FireDash;
			else if (selection.x > 0)
				skillKind = SpecialKind.FireBomb;
			else if (selection.y < 0)
				skillKind = SpecialKind.IceStomp;

			OnSelectSpecial?.Invoke(skillKind);
		} 

		private void OnPauseInput(CallbackContext _) => OnPause?.Invoke();

		private void OnCheatInput(CallbackContext context)
		{
			string buttonName = context.control.name;
			string number = buttonName.Substring(1, buttonName.Length-1);

			if (int.TryParse(number, out int value))
				OnCheat?.Invoke((DevCheat)value);
			else
				Note.LogError("Cheat button control doesn't seem to be one of the 'F' buttons!");
		}
		#endregion
	}
}
