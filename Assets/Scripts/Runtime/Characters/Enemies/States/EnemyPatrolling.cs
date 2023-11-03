
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// In this state, the <see cref="Enemy"/> will move back and forth
	/// between the set area. It also handles turning around, if any intraversable
	/// surface is detected.
	/// </summary>
	public class EnemyPatrolling : EnemyState
	{
		#region LifeCycle
		[SerializeField] private FloatRange _area;
		[SerializeField] private float _acceleration = 1;
		[SerializeField] private float _maxSpeed = 5;
		[Space]
		[SerializeField] private EnemyState _attackState;
		[SerializeField] private EnemyState _actState;

		public override void Wake(StateData data)
		{
			base.Wake(data);

			float x = Pawn.Transform.position.x;
			MinX = x + _area.Min;
			MaxX = x + _area.Max;
		}

		public override void Enter()
		{
			Pawn.Animator.ModifySpeed(1f);
			Speed = 0f;
		}

		public override void OnUpdate()
		{
			if (TryTransitDueToAggro(_actState))
				return;

			Detector.OnUpdate();

			if (TryTransitDueToDetection(Detector.CanAttack, _attackState))
				return;
			if(TryTransitDueToDetection(Detector.CanAct, _actState))
				return;

			Move();
		}
		#endregion



		#region Movement
		private void Move()
		{
			if (Pawn.Facing == -1)
				Speed = Mathf.Max(Speed - _acceleration, -_maxSpeed);
			else
				Speed = Mathf.Min(Speed + _acceleration, _maxSpeed);

			Pawn.Move(Vector2.right * Speed.Sign(), Mathf.Abs(Speed));
			TryFlipByLimits();
			TryFlipByCollissions();
		}

		private void TryFlipByLimits()
		{
			float x = Pawn.Rigidbody.position.x;
			if (x >= MaxX)
				Pawn.Face(Vector2.left);
			if (x <= MinX)
				Pawn.Face(Vector2.right);
		}

		private void TryFlipByCollissions()
		{
			if (Pawn.IsCollidingRight)
			{
				Pawn.Face(Vector2.left);
				Speed = -Mathf.Abs(Speed);
			}
			if (Pawn.IsCollidingLeft)
			{
				Pawn.Face(Vector2.right);
				Speed = Mathf.Abs(Speed);
			}
		}

		public void ForceFlip()
		{
			Pawn.Face(Vector2.left * Pawn.Facing);
			Speed *= -1f;
		}

		public override void OnFixedUpdate() => Pawn.OnFixedUpdate();

		private float MinX { get; set; }
		private float MaxX { get; set; }
		private float Speed { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Vector2 pos = UnityEditor.EditorApplication.isPlaying ? Pawn.OriginalPosition : transform.position;
			GizmoAddons.DrawLineVertical(pos + Vector2.right*_area.Min);
			GizmoAddons.DrawLineVertical(pos + Vector2.right*_area.Max);
		}
		#endif
	}
}