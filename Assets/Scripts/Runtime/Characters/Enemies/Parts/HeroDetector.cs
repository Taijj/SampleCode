
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Performs different distance and visibility checks towards the <see cref="Hero"/>,
	/// to determine, if they are visible or in range.
	/// </summary>
	public class HeroDetector : MonoBehaviour
	{
		#region LifeCycle
		[SerializeField] private float _actDistance;
		[SerializeField] private float _attackDistance;
		[Space]
		[SerializeField] private Transform _eyes;

		public void Wake(Pawn enemyPawn) => EnemyPawn = enemyPawn;

		public void SetUp(Pawn heroPawn)
		{
			HeroPawn = heroPawn;
			Transform = transform;

			SquareAct = _actDistance * _actDistance;
			SquareAttack = _attackDistance * _attackDistance;

			WakeSpotting();
		}

		private Pawn EnemyPawn { get; set; }
		private Pawn HeroPawn { get; set; }
		public Transform Transform { get; private set; }



		[field: SerializeField, ReadOnly] public bool IsFacingHero { get; private set; }
		public bool CanAct => IsInActRange && IsInView;
		public bool CanAttack => IsInAttackRange && IsInView;
		#endregion



		#region Main
		public void OnUpdate()
		{
			ToHero = HeroCenter - (Vector2)Transform.position;
			IsFacingHero = ToHero.x.Sign() == EnemyPawn.Facing;
			IsInAttackRange = ToHero.sqrMagnitude <= SquareAttack;

			if (false == IsInAttackRange)
				IsInActRange = ToHero.sqrMagnitude <= SquareAct;
			else
				IsInActRange = true;

			TrySpotHero();
		}

		[field: SerializeField, ReadOnly] private bool IsInActRange { get; set; }
		[field: SerializeField, ReadOnly] private bool IsInAttackRange { get; set; }

		public Vector2 ToHero { get; private set; }
		public Vector2 HeroCenter => HeroPawn.Center;

		private float SquareAct { get; set; }
		private float SquareAttack { get; set; }
		#endregion



		#region Spotting
		private void WakeSpotting()
		{
			HasEyes = _eyes.HasReference(true);
			if(HasEyes)
			{
				IsInView = false;
				ViewHits = new RaycastHit2D[1];
			}
			else
			{
				IsInView = true;
			}
		}

		private void TrySpotHero()
		{
			bool cannotSpot = !(IsInActRange && HasEyes);
			if (cannotSpot)
			{
				IsInView = true;
				return;
			}

			ContactFilter2D filter = new ContactFilter2D
			{
				useTriggers = false,
				useLayerMask = true,
				layerMask = Layers.ToMask(Layers.WORLD, Layers.HERO)
			};

			Vector2 pos = _eyes.position;
			Vector2 dir = HeroPawn.Center - pos;

			int count = Physics2D.Raycast(pos, dir, filter, ViewHits);
			if (count > 0)
				IsInView = ViewHits[0].collider.gameObject.layer == Layers.HERO;
			else
				IsInView = false;
		}

		[field: SerializeField, ReadOnly] private bool IsInView { get; set; }
		private bool HasEyes { get; set; }
		private RaycastHit2D[] ViewHits { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			if(_attackDistance > _actDistance)
				_actDistance = _attackDistance;
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = ColorAddons.Orange;
			Gizmos.DrawWireSphere(transform.position, _actDistance);

			Gizmos.color = ColorAddons.Red;
			Gizmos.DrawWireSphere(transform.position, _attackDistance);
		}
		#endif
	}
}