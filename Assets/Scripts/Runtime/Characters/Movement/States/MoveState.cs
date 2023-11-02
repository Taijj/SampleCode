
using System.Collections.Generic;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Abstract base for a "Movement State". Contains common logic and shortcuts to read/write to
	/// <see cref="MoveData"/>.
	/// </summary>
	public abstract class MoveState
    {
		#region LifeCycle
		private const int MAX_CONTACTS = 3;

		public MoveState(MoveData data)
    	{
			Data = data;
			Contacts = new List<Contact>(MAX_CONTACTS);
    	}

		public virtual void Handle(Jump jump) {}
		public abstract void Execute();
		public abstract float ConfigAngle { get; }
		#endregion



		#region Data Shortcuts
		private MoveData Data { get; set; }

		protected Rigidbody2D Rigidbody => Data.Rigidbody;
		protected PhysicsConfig Config => Data.Config;
		protected float GravityCapped => Mathf.Max(Data.Velocity.y - GravityRaw, -Data.Config.maxFallSpeed);
		protected float GravityRaw => Data.ModifiedGravity * Game.Updater.AmplifiedFixedDeltaTime;
		protected float MoveSpeed => Data.ModifiedSpeed * Game.Updater.AmplifiedFixedDeltaTime;
		protected float SlopeSpeed => Config.slopeSpeed * Game.Updater.AmplifiedFixedDeltaTime;
		protected int MoveSign => Data.Sign;

		protected Vector2 Velocity { get => Data.Velocity; set => Data.Velocity = value; }
		#endregion



		#region Contacts
		public void ClearContacts() => Contacts.Clear();
		public void Add(Contact contact) => Contacts.Add(contact);
		public bool HasContacts => Contacts.Count > 0;
		protected List<Contact> Contacts { get; private set; }

		protected bool IsCollidingLeft => Data.IsCollidingLeft;
		protected bool IsCollidingRight => Data.IsCollidingRight;
		#endregion
	}
}