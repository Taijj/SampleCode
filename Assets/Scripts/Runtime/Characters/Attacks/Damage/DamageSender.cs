
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Kind = Taijj.SampleCode.DamageOutcome.Kind;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Handles the transfer of damage and other information to a <see cref="DamageReceiver"/> and its owner.
	/// </summary>
	public class DamageSender : MonoBehaviour
    {
		#region LifeCycle
		private const int MAX_OVERLAPS = 50;
		private const string HELP = "DamageCollider is used to look for objects that can be damaged, e.g. enemies. " +
			"It is usually larger and respects the configured LayerMask.\n" +
			"WorldCollider is OPTIONAL and used to look for objects on the 'World' Layer, which will then be intepreted" +
			"as solid collisions. It is usually smaller.";

		[SerializeField] private LayerMask _layerMask;
		[SerializeField] private AttackAttribute _attribute;
		[SerializeField] private float _strength = 1;
		[Space]
		[Help(HELP, HelpAttribute.HelpKind.Info)]
		public bool helpTarget;
		[Space]
		[SerializeField] private Collider2D[] _damageColliders;
		[SerializeField] private Collider2D _worldCollider;

		public void Wake(AttackInfo info, DamageCondition condition = null)
		{
			Transform = transform;

			Info = info;
			info.AddAttribute(_attribute);
			info.Strength = _strength;
			info.SourceLayer = gameObject.layer;

			Condition = condition == null ? DamageCondition.Empty : condition;
			HitColliders = new Collider2D[MAX_OVERLAPS];
			ValidOverlaps = new Collider2D[MAX_OVERLAPS];
			HasWorldCollider = _worldCollider.HasReference(true);
		}

		private Transform Transform { get; set; }
		#endregion



		#region Overlap Detection
		public void OnUpdate()
		{
			HandleDamage(out bool isHit);
			if (isHit)
				return;
			HandleWorldCollision();
		}

		public void HandleDamage(out bool isHit)
		{
			GetDamageOverlaps();
			isHit = OverlapCount != 0;
			if (false == isHit)
				return;

			bool hasDamagedSomething = false;
			for (int i = 0; i < OverlapCount; i++)
				hasDamagedSomething |= TryDamage(ValidOverlaps[i]);

			if (hasDamagedSomething)
				return;

			Collider2D hit = ValidOverlaps[0];
			
			// Workaround so Fishbones won't hit ore pieces.
			if (hit.TryGet(out Pickup _))
				return;

			if (hit.IsIn(_layerMask))
				SignalOutcome(hit.isTrigger ? Kind.Ignored : Kind.HitSolid);
		}

		private void GetDamageOverlaps()
		{
			ContactFilter2D filter = new ContactFilter2D
			{
				useLayerMask = true,
				layerMask = _layerMask,
				useTriggers = true
			};

			OverlapCount = 0;
			for(int i = 0; i < _damageColliders.Length; i++)
			{
				int count = _damageColliders[i].OverlapCollider(filter, HitColliders);
				if (count == 0)
					continue;

				for(int j = 0; j < count; j++)
				{
					ValidOverlaps[OverlapCount] = HitColliders[j];
					OverlapCount++;

					if (OverlapCount >= MAX_OVERLAPS)
						break;
				}

				if (OverlapCount >= MAX_OVERLAPS)
					break;
			}
		}

		public void HandleWorldCollision()
		{
			if (false == HasWorldCollider)
				return;

			ContactFilter2D filter = new ContactFilter2D
			{
				useLayerMask = true,
				layerMask = Layers.WORLD.ToMask()
			};
			int count = _worldCollider.OverlapCollider(filter, HitColliders);
			if (count > 0)
				SignalOutcome(Kind.HitSolid);
		}



		private int OverlapCount { get; set; }
		private Collider2D[] HitColliders { get; set; }
		private Collider2D[] ValidOverlaps { get; set; }

		private bool HasWorldCollider { get; set; }
		#endregion



		#region Receiver Processing
		public bool TryDamage(Collider2D other)
		{
			if (false == Addons.TryGet(other, out DamageReceiver receiver))
				return false;

			if (false == Game.Updater.TryRecordForFrame(receiver.gameObject))
				return false;

			Receiver = receiver;
			SetInfoContact(other);

			if (false == Receiver.ReactsTo(Info.Attributes))
				return false;

			TryDamageReceiver();
			return true;
		}

		private void TryDamageReceiver()
		{
			if (Receiver.IsInvulnerable)
			{
				SignalOutcome(Kind.Ignored);
				return;
			}

			if(Receiver.IsInvincible)
			{
				SignalOutcome(Kind.HitSolid);
				return;
			}

			if (false == Condition.CanBeDamaged(Receiver))
			{
				SignalOutcome(Kind.HitSolid);
				return;
			}

			Receiver.TakeDamage(Info);
			if (Receiver.IsDead)
			{
				Receiver.Kill(Info);
				SignalOutcome(Kind.Killed);
			}
			else
			{
				Condition.OnDamaged(Receiver);
				SignalOutcome(Kind.Damaged);
			}
		}

		private AttackInfo Info { get; set; }
		private DamageCondition Condition { get; set; }
		private DamageReceiver Receiver { get; set; }
		#endregion



		#region Helpers
		private void SetInfoContact(Collider2D overlap)
		{
			Vector2 point = overlap.ClosestPoint(Transform.position);
			Vector2 normal = ((Vector2)Transform.position - point).normalized;
			Contact con = new Contact
			{
				point = point,
				normal = normal,
				collider = overlap
			};

			Info.Contact = con;
		}

		private void SignalOutcome(Kind kind)
		{
			DamageOutcome outcome = new DamageOutcome
			{
				kind = kind,
				receiver = Receiver,
				info = Info
			};

			OnDamageSent?.Invoke(outcome);
		}

		public Action<DamageOutcome> OnDamageSent;
		#endregion



		#if UNITY_EDITOR
		[Space, Header("Editor")]
		[SerializeField] private GameObject _collidersRetrieval;

		[Help("Drag any object in the field above, to retrieve all Triggers from it that should be used as '_damagesColliders'")]
		public bool help;

		public void OnValidate()
		{
			if(_collidersRetrieval.HasReference(true))
			{
				Collider2D[] all = _collidersRetrieval.GetComponentsInChildren<Collider2D>();
				_damageColliders = all.Where(c => c.isTrigger).ToArray();
				_collidersRetrieval = null;
			}
		}
		#endif
	}
}