
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// State an <see cref="Enemy"/> is in while doing nothing.
	/// </summary>
	public class EnemyIdle : EnemyState
	{
		[SerializeField] private EnemyState _actState;
		[SerializeField] private EnemyState _attackState;

		public override void Enter()
		{
			Pawn.Stop();
			Pawn.Animator.Trigger(AnimatorHashes.IDLE);
		}

		public override void OnUpdate()
		{
			if (TryTransitDueToAggro(_actState))
				return;

			Detector.OnUpdate();

			if (TryTransitDueToDetection(Detector.CanAttack, _attackState))
				return;
			TryTransitDueToDetection(Detector.CanAct, _actState);
		}

		public override void OnFixedUpdate() => Pawn.OnFixedUpdate();
	}
}