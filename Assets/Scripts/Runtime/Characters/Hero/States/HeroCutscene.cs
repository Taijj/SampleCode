using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// This state is only a dummy for the prototype and should be discarded as soon as a full-fledged cutscene system is available. 
	/// 
	/// Handles hero movement during a Cutscene
	/// </summary>
	public class HeroCutscene : HeroState
    {
		#region LiefCycle
		[SerializeField] private Vector2 _moveDirection;
		[SerializeField] private float _moveSpeed;

		public override void Enter()
		{
			Input.Disable();

			LastAnimation = Animation.Idle;
			Pawn.Animator.Trigger(AnimatorHashes.IDLE);
		}

		public override void OnUpdate()
		{
			Pawn.Face(_moveDirection);
			Pawn.Move(_moveDirection, _moveSpeed);

			AnimateMovement();
		}

		public override void OnFixedUpdate() => Pawn.OnFixedUpdate();

		public override void Exit() => Input.Enable();
		#endregion



		#region Animations
		private enum Animation
		{
			Idle = 0,

			Walk,
			Jump,
			Fall
		}

		private void AnimateMovement()
		{
			if (Pawn.IsGrounded)
				AnimateGrounded();
			else
				AnimateAirborne();
		}

		private void AnimateGrounded()
		{
			bool shouldWalk = Pawn.Rigidbody.velocity.x != 0f;
			if (shouldWalk && LastAnimation != Animation.Walk)
			{
				Pawn.Animator.Trigger(AnimatorHashes.WALK);
				LastAnimation = Animation.Walk;
				return;
			}

			if (false == shouldWalk && LastAnimation != Animation.Idle)
			{
				Pawn.Animator.Trigger(AnimatorHashes.IDLE);
				LastAnimation = Animation.Idle;
			}
		}

		private void AnimateAirborne()
		{
			bool shouldJump = Pawn.Rigidbody.velocity.y > 0f;
			if (shouldJump && LastAnimation != Animation.Jump)
			{
				Pawn.Animator.Trigger(AnimatorHashes.JUMP);
				LastAnimation = Animation.Jump;
				return;
			}

			bool shouldFall = Pawn.Rigidbody.velocity.y <= 0f;
			if (shouldFall && LastAnimation != Animation.Fall)
			{
				Pawn.Animator.Trigger(AnimatorHashes.FALL);
				LastAnimation = Animation.Fall;
			}
		}

		[field: SerializeField, ReadOnly] private Animation LastAnimation { get; set; }
		#endregion
	}
}
