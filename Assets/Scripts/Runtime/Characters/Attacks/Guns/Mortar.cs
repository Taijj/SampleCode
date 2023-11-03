using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Attack that throws a <see cref="Shot"/> in an arching motion.
	/// </summary>
	public class Mortar : Attack
	{
		#region Main
		public enum TargetKind
		{
			Fixed = 0,
			Hero
		}

		[Space, Header("Mortar")]
		[SerializeField] private Shot _prefab;
		[SerializeField] private float _refireDelay = 0.1f;
		[SerializeField] private float _offscreenTolerance;
		[Space]
		[SerializeField] private float _arcHeight = 5.0f;
		[SerializeField] private float _shotSpeedMultiplier = 1.0f;



		[SerializeField] private TargetKind _target;

		[DrawIf(nameof(_target), TargetKind.Fixed)]
		[SerializeField] private Vector2 _targetPosition;

		[DrawIf(nameof(_target), TargetKind.Hero)]
		[SerializeField] private FloatRange _range;

		public override void Wake()
		{
			base.Wake();

			ShotInfo = new ShotInfo();
			ShotInfo.prefab = _prefab;
		}

		protected override void Perform()
		{
			Vector2 target = _target == TargetKind.Fixed
				? (Vector2)Origin.position + _targetPosition.WithX(_targetPosition.x * Facing)
				: Level.Hero.Pawn.Center;

			ShotInfo.position = Origin.position;
			ShotInfo.velocity = GetLaunchVelocity(target);
			ShotInfo.offscreenTolerance = _offscreenTolerance;

			if (ShotInfo.prefab.TryGetComponent(out Rigidbody2D rigidbody))
				rigidbody.gravityScale = _shotSpeedMultiplier;

			Level.ShotFactory.Spawn(ShotInfo);
			NextShotTime = Time.time + _refireDelay;
		}

		private ShotInfo ShotInfo { get; set; }
		private float NextShotTime { get; set; }
		public override bool CanPerform => Time.time >= NextShotTime;
		#endregion



		#region Arc
		private Vector2 GetLaunchVelocity(Vector2 targetPosition)
		{
			Vector2 clamped = Clamp(targetPosition);
			return CalculateLaunchVelocity(clamped);
		}

		private Vector2 Clamp(Vector2 targetPosition)
		{
			if (_target == TargetKind.Fixed)
				return targetPosition;

			Vector2 pos = Origin.position;
			float minX = pos.x + _range.Min * Facing;
			float maxX = pos.x + _range.Max * Facing;
			float maxY = pos.y + _arcHeight;

			if (minX > maxX)
			{
				float swap = maxX;
				maxX = minX;
				minX = swap;
			}

			float x = Mathf.Clamp(targetPosition.x, minX, maxX);
			float y = Mathf.Min(maxY, targetPosition.y);
			return new Vector2(x, y);
		}

		private Vector2 CalculateLaunchVelocity(Vector2 targetPosition)
		{
			float gravity = Physics2D.gravity.y * _shotSpeedMultiplier;

			float a = Mathf.Sqrt(-2 * _arcHeight / gravity);

			float preB = 2 * (targetPosition.y - Origin.position.y - _arcHeight) / gravity;
			float b = Mathf.Sqrt(Mathf.Abs(preB));
			float divisor = a + b;

			float c = Mathf.Sqrt(-2 * gravity * _arcHeight);
			float d = -Mathf.Sign(gravity);
			Vector2 vector1 = Vector2.up * c * d;
			Vector2 vector2 = new Vector2(targetPosition.x - Origin.position.x, 0) / divisor;

			return vector1 + vector2;
		}
		#endregion



		#if UNITY_EDITOR
		private const float RANGE_GIZMO_HEIGHT = 10f;

		public void OnDrawGizmosSelected()
		{
			if (Origin.IsNull(true))
				return;

			if (_target == TargetKind.Fixed)
				DrawFixedGizmos();
			else
				DrawHeroGizmos();

		}

		private void DrawFixedGizmos()
		{
			Vector2 p1 = Origin.position;
			Vector2 p3 = (Vector2)Origin.position + _targetPosition * Facing;
			Vector2 p2 = p1 + (p3 - p1)/2f + Vector2.up * _arcHeight;

			Gizmos.color = ColorAddons.Red;
			Gizmos.DrawLine(p1, p2);
			Gizmos.DrawLine(p2, p3);
			Gizmos.DrawWireSphere(p3, 0.2f);
		}

		private void DrawHeroGizmos()
		{
			Vector2 pos = Origin.position;
			Vector2 p1 = pos + Vector2.right * _range.Min * Facing;
			Vector2 p2 = pos + Vector2.right * _range.Max * Facing;

			Gizmos.color = ColorAddons.Red;
			GizmoAddons.DrawLineVertical(p1, RANGE_GIZMO_HEIGHT);
			GizmoAddons.DrawLineVertical(p2, RANGE_GIZMO_HEIGHT);
		}
		#endif
	}
}
