using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Base manager of a Character's visuals and physics.
	/// Is the object, that actually moves through the scene.
	/// </summary>
    public abstract class Pawn : DamageReceiver
    {
		#region LifeCycle
		public class PawnData
		{
			public Func<bool> isInvulnerable;
			public Func<bool> isInvincible;
			public Func<bool> isDead;
			public Action<AttackInfo> onDamaged;
		}

		[Space, Header("Pawn")]
		[SerializeField] private Rigidbody2D _rigidbody;
		[SerializeField] private CharacterSkin _character;
		[SerializeField] private CharacterAnimator _animator;
		[SerializeField] private Vector2 _visualCenter;
		[Space]
		[SerializeField] private Cloth[] _cloths;
		[SerializeField] private GameObject[] _activeParts;
		[SerializeField] private Collider2D[] _colliders;

		public virtual void Wake(PawnData data)
		{
			Transform = transform;
			OriginalPosition = Transform.position;
			OriginalFacing = Transform.GetFacing();
			Facing = OriginalFacing;

			_animator.Wake();
			for (int i = 0; i < _cloths.Length; i++)
				_cloths[i].Wake();

			Data = data;
		}

		public virtual void SetUp()
		{
			for (int i = 0; i < _cloths.Length; i++)
				_cloths[i].SetUp();
		}

		public virtual void Respawn()
		{
			Transform.position = OriginalPosition;
			Facing = Transform.SetFacing(OriginalFacing);

			_animator.ResetSelf();
		}

		public virtual void CleanUp()
		{
			for (int i = 0; i < _cloths.Length; i++)
				_cloths[i].CleanUp();
		}

		private PawnData Data { get; set; }
		#endregion



		#region Enabling & Activation
		public void Enable()
		{
			for (int i = 0; i < _activeParts.Length; i++)
				_activeParts[i].Activate();

			for (int i = 0; i < _colliders.Length; i++)
				_colliders[i].enabled = true;
		}

		public void Disable()
		{
			for (int i = 0; i < _activeParts.Length; i++)
				_activeParts[i].Deactivate();

			for (int i = 0; i < _colliders.Length; i++)
				_colliders[i].enabled = false;
		}

		public void Activate()
		{
			_rigidbody.simulated = true;
			_character.Activate();
			for (int i = 0; i < _cloths.Length; i++)
				_cloths[i].Activate();
		}

		public void Deactivate()
		{
			_rigidbody.simulated = false;
			_character.Deactivate();
			for (int i = 0; i < _cloths.Length; i++)
				_cloths[i].Deactivate();
		}
		#endregion



		#region Movement
		public abstract void Move(Vector2 normalizedDirection, float speed);
		public abstract void Push(Vector2 normalizedDirection, float force);
		public abstract void Stop();

		public virtual void OnFixedUpdate() {}

		public virtual bool IsStandingStill => Rigidbody.velocity == Vector2.zero;
		public virtual bool IsCollidingLeft => false;
		public virtual bool IsCollidingRight => false;
		public virtual bool IsGrounded => false;
		#endregion



		#region Misc
		public override void TakeDamage(AttackInfo info) => Data.onDamaged(info);
		public override bool IsDead => Data.isDead();
		public override bool IsInvulnerable => Data.isInvulnerable();
		public override bool IsInvincible => Data.isInvincible();



		public Transform Transform { get; private set; }
		public Vector2 OriginalPosition { get; private set; }
		private int OriginalFacing { get; set; }

		public Rigidbody2D Rigidbody => _rigidbody;
		public CharacterAnimator Animator => _animator;
		public virtual Vector2 Center => (Vector2)Transform.position + _visualCenter;

		public void Face(Vector2 direction) => Facing = Transform.SetFacing(direction);
		public int Facing { get; private set; }
		#endregion



		#if UNITY_EDITOR
		public virtual void OnValidate()
		{
			this.TryAssign(ref _rigidbody);
			this.TryAssign(ref _character);
			this.TryAssign(ref _animator);
			this.TryAssign(ref _colliders);

			if (_rigidbody.HasReference(true))
			{
				_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
				_rigidbody.gravityScale = 0f;
			}
		}

		public void Flip()
		{
			UnityEditor.Undo.RecordObject(gameObject, "Flip");
			transform.SetFacing(-transform.GetFacing());
			Facing = transform.GetFacing();
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = ColorAddons.Pink;
			GizmoAddons.DrawX((Vector2)transform.position + _visualCenter);
		}
		#endif
	}
}