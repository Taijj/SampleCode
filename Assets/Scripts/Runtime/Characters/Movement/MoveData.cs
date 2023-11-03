
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Data helper and communication object between <see cref="HeroMovement"/> and its <see cref="MoveState"/>s.
	/// </summary>
	public class MoveData
	{
		#region Components
		public MoveData(Rigidbody2D rigidbody, Corpus corpus)
		{
			Rigidbody = rigidbody;
			Corpus = corpus;
			Corpus.OnEnteredWater = SetModifiers;
			Corpus.OnExitedWater = ResetModifiers;

			CurrentModifiers = new Modifiers();
			ResetModifiers();
		}

		public Corpus Corpus { get; private set; }
		public Rigidbody2D Rigidbody { get; private set; }
		public CapsuleCollider2D Collider => Corpus.Collider;
		public PhysicsConfig Config => Game.Catalog.Physics;
		#endregion



		#region Data
		public float RawSpeed { set; get; }
		public Vector2 Velocity { set; get; }
		public int Sign => Velocity.x.Sign(true);

		public bool IsCollidingLeft { get; set; }
		public bool IsCollidingRight { get; set; }
		#endregion



		#region Modifiers
		[System.Serializable]
		public class Modifiers
		{
			public float moveSpeed;
			public float gravity;
		}

		public void SetModifiers(Modifiers modifiers)
		{
			CurrentModifiers.moveSpeed = modifiers.moveSpeed;
			CurrentModifiers.gravity = modifiers.gravity;
		}

		public void ResetModifiers()
		{
			CurrentModifiers.moveSpeed = 1f;
			CurrentModifiers.gravity = 1f;
		}

		public float ModifiedSpeed => RawSpeed * CurrentModifiers.moveSpeed;
		public float ModifiedGravity => Config.gravity * CurrentModifiers.gravity;

		private Modifiers CurrentModifiers { get; set; }
		#endregion
	}
}