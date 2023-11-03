using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// State for <see cref="Enemy"/>s while they chase after the <see cref="Hero"/>.
	/// </summary>
	public class EnemyChasing : EnemyState
	{
		#region LifeCycle
		[SerializeField] private EnemyState _attackState;
		[SerializeField] private float _acceleration = 1f;
		[SerializeField] private float _maxSpeed = 5f;
		[SerializeField] private float _tolerance = 1f;
		[SerializeField] private float _animationSpeedModifier;

		public override void Enter()
		{
			Pawn.Animator.ModifySpeed(_animationSpeedModifier);
			Speed = 0f;
		}
		#endregion



		#region Updates
		public override void OnUpdate()
		{
			Detector.OnUpdate();
			if (TryTransitDueToDetection(Detector.CanAttack, _attackState))
				return;

			Move();
		}

		private void Move()
		{
			if (Mathf.Abs(Detector.ToHero.x) < _tolerance)
				return;

			if (Pawn.Facing == -1)
				Speed = Mathf.Max(Speed - _acceleration, -_maxSpeed);
			else
				Speed = Mathf.Min(Speed + _acceleration, _maxSpeed);

			Pawn.Move(Vector2.right * Speed.Sign(), Mathf.Abs(Speed));
			Pawn.Face(Detector.ToHero);
		}

		public override void OnFixedUpdate() => Pawn.OnFixedUpdate();

		private float Speed { get; set; }
		#endregion
	}
}