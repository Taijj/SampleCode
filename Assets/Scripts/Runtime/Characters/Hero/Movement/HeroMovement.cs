using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Handles the movement behavior of the <see cref="Hero"/>.
	/// </summary>
	public class HeroMovement : PhysicalMovement
	{
		#region Main
		[Space]
		[SerializeField] private Jump _jump;
		[SerializeField] private EventReference _jumpSound;

		public override void Wake(MoveData data)
		{
			base.Wake(data);
			SoundTarget = data.Corpus.transform;
		}

		public override void Push(Vector2 direction, float force)
		{
			base.Push(direction, force);
			_jump.StopJump();
		}

		public override void OnFixedUpdate()
		{
			DetermineState();
			State.Handle(_jump);
			State.Execute();

			UpdateJump();
			Data.Rigidbody.velocity = Data.Velocity;

			UpdateSlipDelta();
		}
		#endregion



		#region State
		protected override void SetStateFromEmptyContacts()
		{
			if (_jump.IsJumping)
			{
				SetState(Airborne);
				return;
			}

			base.SetStateFromEmptyContacts();
		}


		protected override void SetState(MoveState value)
		{
			#if UNITY_EDITOR
				if (State != Grounded && value == Grounded)
					_jump.StopJumpEditor(Data.Rigidbody);
			#endif

			HandleCoyoting(value);
			base.SetState(value);
		}

		private void HandleCoyoting(MoveState newState)
		{
			if (_jump.IsJumping)
				return;

			if (State != Grounded)
				return;

			bool shouldCoyote = newState == Sloped
				|| newState == Walled
				|| newState == Airborne;
			if (shouldCoyote)
				_jump.StartCoyoting();
		}
		#endregion



		#region Jumping
		public void StartJump()
		{
			if(_jump.TryJump())
				Game.Audio.Play(_jumpSound, SoundTarget);

			#if UNITY_EDITOR
				_jump.StartJumpEditor(Data.Rigidbody);
			#endif
		}

		public void UpdateJump()
		{
			_jump.UpdateCoyoting();
			if (false == _jump.IsJumping)
			{
				StopJump();
				return;
			}

			float force = _jump.Force * Game.Updater.AmplifiedFixedDeltaTime;
			Data.Velocity = Data.Velocity.WithY(force);

			#if UNITY_EDITOR
				_jump.UpdateJumpEditor(Data.Rigidbody);
			#endif
		}

		public void StopJump() => _jump.StopJump();

		public void OverrideJumpCount(int value) => _jump.OverrideCount(value);

		private Transform SoundTarget { get; set; }
		#endregion
	}
}
