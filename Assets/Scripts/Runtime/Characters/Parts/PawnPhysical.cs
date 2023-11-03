using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// <see cref="Pawn"/> with physics and physical movement.
	/// </summary>
	public class PawnPhysical : Pawn
	{
		#region LifeCycle
		[Space, Header("Physical")]
		[SerializeField] private Corpus _corpus;
		[SerializeField] private PhysicalMovement _movement;

		public override void Wake(PawnData data)
		{
			base.Wake(data);
			_corpus.Wake();
			_movement.Wake(new MoveData(Rigidbody, _corpus));

			if(_movement is HeroMovement)
			{
				HeroMovement = (HeroMovement)_movement;
				IsHero = true;
			}
		}
		#endregion



		#region Movement
		public override void Move(Vector2 normalizedDirection, float speed) => _movement.Move(normalizedDirection.x.Sign(true), speed);
		public override void Push(Vector2 normalizedDirection, float force) => _movement.Push(normalizedDirection, force);
		public override void Stop() => _movement.Stop();

		public void TryStartJump()
		{
			if (IsHero)
				HeroMovement.StartJump();
		}

		public void TryStopJump()
		{
			if (IsHero)
				HeroMovement.StopJump();
		}

		public override void OnFixedUpdate()
		{
			_movement.OnFixedUpdate();
			_corpus.OnFixedUpdate();
		}

		public override bool IsCollidingLeft => _movement.IsCollidingLeft;
		public override bool IsCollidingRight => _movement.IsCollidingRight;
		public override bool IsGrounded => _movement.IsGrounded;

		private HeroMovement HeroMovement { get; set; }
		private bool IsHero { get; set; }
		#endregion



		#if UNITY_EDITOR
		public override void OnValidate()
		{
			base.OnValidate();
			if (_corpus.IsNull(true)) _corpus = GetComponentInChildren<Corpus>();
			if (_movement.IsNull(true)) _movement = GetComponentInChildren<PhysicalMovement>();
		}
		#endif
	}
}