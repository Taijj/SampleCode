
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// State an <see cref="Enemy"/> is in while performing an arbitrary <see cref="Attack"/>.
	/// </summary>
	public class EnemyPerformingAttack : EnemyState
	{
		#region LifeCycle
		[SerializeField] private Attack _attack;
		[SerializeField] private EnemyState _outOfRangeState;

		public override void Wake(StateData data)
		{
			base.Wake(data);
			_attack.Wake();
		}

		public override void CleanUp()
		{
			base.CleanUp();
			_attack.CleanUp();
		}
		#endregion



		#region Attacking
		public override void Enter()
		{
			Pawn.Stop();
			Pawn.Animator.Trigger(AnimatorHashes.ATTACK);
			Pawn.Animator.OnEvent += OnAnimatorEvent;
		}

		private void OnAnimatorEvent(CharacterAnimator.Event e)
		{
			if (e != CharacterAnimator.Event.Default)
				return;

			_attack.TryPerform();
			_attack.Cease();
		}
		public override void Exit() => Pawn.Animator.OnEvent -= OnAnimatorEvent;
		#endregion



		#region Updates
		public override void OnUpdate()
		{
			Detector.OnUpdate();
			if(false == Detector.CanAttack)
			{
				Transit(_outOfRangeState.GetType());
				return;
			}

			if (false == Detector.IsFacingHero)
				Pawn.Face(Vector2.right * Pawn.Facing * -1);
		}

		public override void OnFixedUpdate() => Pawn.OnFixedUpdate();
		#endregion
	}
}