using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// State the <see cref="Hero"/> is in while
	/// moving freely.
	/// </summary>
	public class HeroMoving : HeroState
    {
		#region Main
		[SerializeField] private float _speed;
		[SerializeField] private EventReference _landSound;

		public override void Enter()
		{
			Input.OnJumpPress = Pawn.TryStartJump;
			Input.OnJumpRelease = Pawn.TryStopJump;
			Input.OnShootFire = ShootFire;
			Input.OnShootIce = ShootIce;
			Input.OnSpecial = Arsenal.PerformSpecial;

			LastAnimation = Animation.Idle;
			Pawn.Animator.Trigger(AnimatorHashes.IDLE);
		}

		public override void OnUpdate()
		{
			Input.OnUpdate();

			Vector2 input = Input.Move;
			Pawn.Face(input);
			Pawn.Move(input, _speed);
			
			bool canDuck = Input.Move.x == 0f && Input.IsDownHeld && false == Arsenal.IsBreathing && Pawn.IsGrounded;
			if (canDuck)
			{
				Transit(typeof(HeroDucked));
				return;
			}

			AnimateMovement();
			UpdateBreathing();
		}

		public override void OnFixedUpdate() => Pawn.OnFixedUpdate();

		public override void Exit()
		{
			Input.OnJumpPress = null;
			Input.OnJumpRelease = null;
			Input.OnShootFire = null;
			Input.OnShootIce = null;
			Input.OnSpecial = null;
		}
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
			Pawn.Animator.Set(IS_MOVING_HORIZONTALLY_HASH, false == Pawn.IsStandingStill);
			if (Pawn.IsGrounded)
				AnimateGrounded();
			else
				AnimateAirborne();
		}

		private void AnimateGrounded()
		{
			if (LastAnimation.IsEither(Animation.Jump, Animation.Fall))
				Game.Audio.Play(_landSound);
			
			bool shouldWalk = Pawn.Rigidbody.velocity.x != 0f && Input.Move.x != 0;
			if (shouldWalk && LastAnimation != Animation.Walk)
			{
				Pawn.Animator.Trigger(AnimatorHashes.WALK);
				LastAnimation = Animation.Walk;
				return;
			}

			if(false == shouldWalk && LastAnimation != Animation.Idle)
			{
				Pawn.Animator.Trigger(AnimatorHashes.IDLE);
				LastAnimation = Animation.Idle;
			}			
		}

		private void AnimateAirborne()
		{
			bool shouldJump = Pawn.Rigidbody.velocity.y > 0f;
			if(shouldJump && LastAnimation != Animation.Jump)
			{
				Pawn.Animator.Trigger(AnimatorHashes.JUMP);
				LastAnimation = Animation.Jump;
				return;
			}

			bool shouldFall = Pawn.Rigidbody.velocity.y <= 0f;
			if(shouldFall && LastAnimation != Animation.Fall)
			{
				Pawn.Animator.Trigger(AnimatorHashes.FALL);
				LastAnimation = Animation.Fall;
			}
		}

		public static readonly int IS_MOVING_HORIZONTALLY_HASH = Animator.StringToHash("IsMovingHorizontally");
		[field: SerializeField, ReadOnly] private Animation LastAnimation { get; set; }
		#endregion
	}
}