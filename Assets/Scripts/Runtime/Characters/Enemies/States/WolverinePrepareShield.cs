using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Initialization State, that handles the Wolverine <see cref="Enemy"/>'s
	/// <see cref="Shield"/>'s lifecycle, before transitting to the actual
	/// default state.
	/// </summary>
	public class WolverinePrepareShield : EnemyState
	{
		#region LifeCycle
		[SerializeField] private Shield _shield;
		[SerializeField] private EnemyState _nextState;
		[Space]
		[SerializeField] private ShieldCollider _collider;
		[SerializeField] private EnemyPatrolling _patrollingState;

		public override void SetUp()
		{
			base.SetUp();
			_collider.OnHitWall = OnHitWallWithShield;
			_shield.Wake(OnShieldDied);
			Level.Route.RegisterRespawn(_shield.Respawn);
		}

		public override void CleanUp()
		{
			base.CleanUp();
			_shield.CleanUp();
			Level.Route.DeregisterRespawn(_shield.Respawn);
		}

		public override void OnUpdate() => Transit(_nextState.GetType());
		#endregion



		#region Shield
		private static readonly int HAS_SHIELD_HASH = Animator.StringToHash("HasShield");
		public override void Enter()
		{
			Pawn.Animator.Set(HAS_SHIELD_HASH, true);
			_collider.Activate();
		}

		private void OnShieldDied()
		{
			Pawn.Animator.Set(HAS_SHIELD_HASH, false);
			_collider.Deactivate();
		}

		private void OnHitWallWithShield() => _patrollingState.ForceFlip();
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			if(_nextState.IsNull(true)) _nextState = GetComponents<EnemyState>()[1];
		}
		#endif
	}
}
