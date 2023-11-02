using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// State an <see cref="Enemy"/> is in while performing a looping animation.
	/// Also used for melee attacks.
	/// </summary>
    public class EnemyAttackAnimation : EnemyState
    {
		[SerializeField] private EnemyState _outOfRangeState;

		public override void Enter()
		{
			Pawn.Stop();
			Pawn.Animator.Set(AnimatorHashes.IS_ATTACKING, true);
		}

		public override void OnUpdate()
		{
			Detector.OnUpdate();
			if (false == Detector.CanAttack)
			{
				Transit(_outOfRangeState.GetType());
				return;
			}

			if (false == Detector.IsFacingHero)
				Pawn.Face(Vector2.right * Pawn.Facing * -1);
		}

		public override void OnFixedUpdate() => Pawn.OnFixedUpdate();

		public override void Exit() => Pawn.Animator.Set(AnimatorHashes.IS_ATTACKING, false);
	}
}