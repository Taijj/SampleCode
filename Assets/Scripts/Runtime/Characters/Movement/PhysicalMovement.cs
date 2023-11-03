using System.Collections.Generic;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Contains the logic needed for physical movement, i.e. movement that includes
	/// gravity and collisions with the solid World.
	/// </summary>
	public class PhysicalMovement : MonoBehaviour
	{
		#region LifeCycle
		public virtual void Wake(MoveData data)
		{
			Data = data;

			Grounded = new MoveGrounded(Data);
			Sloped = new MoveSloped(Data);
			Walled = new MoveWalled(Data);
			Roofed = new MoveRoofed(Data);
			Airborne = new MoveAirborne(Data);
			State = Airborne;

			WakeSticking();
		}

		protected MoveData Data { get; private set; }

		protected MoveGrounded Grounded { get; private set; }
		protected MoveSloped Sloped { get; private set; }
		protected MoveWalled Walled { get; private set; }
		protected MoveRoofed Roofed { get; private set; }
		protected MoveAirborne Airborne { get; private set; }

		public Vector2 CurrentVelocity => Data.Velocity;
		#endregion



		#region Main
		public void Move(int sign, float speed)
		{
			CalculateSpeed(speed, ref sign);
			float x = sign * Data.ModifiedSpeed * Game.Updater.AmplifiedFixedDeltaTime;
			Data.Velocity = Data.Velocity.WithX(x);
			Data.Rigidbody.velocity = Data.Velocity;
		}

		public virtual void Push(Vector2 direction, float force)
		{
			Data.RawSpeed = force;
			Data.Velocity = direction * Data.ModifiedSpeed * Game.Updater.AmplifiedFixedDeltaTime;
			Data.Rigidbody.velocity = Data.Velocity;

			SetState(Airborne);
			Data.Corpus.Contacts.Clear();
		}

		public void Stop()
		{
			Data.RawSpeed = 0f;
			Data.Velocity = Vector2.zero;
			Data.Rigidbody.velocity = Vector2.zero;
		}



		public virtual void OnFixedUpdate()
		{
			DetermineState();
			State.Execute();
			Data.Rigidbody.velocity = Data.Velocity;

			UpdateSlipDelta();
		}
		#endregion



		#region State
		protected void DetermineState()
		{
			List<Contact> contacts = Data.Corpus.Contacts;
			if (contacts.Count == 0)
				SetStateFromEmptyContacts();
			else
				SetStateFrom(contacts);
		}

		protected virtual void SetStateFromEmptyContacts()
		{
			if (false == TryStick())
				SetState(Airborne);
		}

		private void SetStateFrom(List<Contact> contacts)
		{
			Roofed.ClearContacts();
			Walled.ClearContacts();
			Sloped.ClearContacts();
			Grounded.ClearContacts();
			IsCollidingLeft = false;
			IsCollidingRight = false;



			float posX = Data.Rigidbody.position.x;
			for (int i = 0; i < contacts.Count; i++)
			{
				Contact con = contacts[i];

				if (con.angle > Roofed.ConfigAngle)
					Roofed.Add(con);
				else if (con.angle > Walled.ConfigAngle)
					Walled.Add(con);
				else if (con.angle > Sloped.ConfigAngle)
					Sloped.Add(con);
				else
					Grounded.Add(con);

				if(con.angle > Sloped.ConfigAngle)
				{
					float conX = con.point.x;
					IsCollidingLeft |= conX < posX;
					IsCollidingRight |= conX > posX;
				}
			}

			Data.IsCollidingLeft = IsCollidingLeft;
			Data.IsCollidingRight = IsCollidingRight;



			if (Grounded.HasContacts)
				SetState(Grounded);
			else if (Sloped.HasContacts)
				SetState(Sloped);
			else if (Walled.HasContacts)
				SetState(Walled);
			else if (Roofed.HasContacts)
				SetState(Roofed);
			else
				SetState(Airborne);
		}

		protected virtual void SetState(MoveState value)
		{
			if (ShouldStayAirborne(value))
				return;

			IsGrounded = value == Grounded;
			State = value;

			#if UNITY_EDITOR
				_currentStateName = State.GetType().Name;
			#endif
		}





		protected MoveState State { get; set; }

		[field: SerializeField, ReadOnly] public bool IsCollidingLeft { get; private set; }
		[field: SerializeField, ReadOnly] public bool IsCollidingRight { get; private set; }
		[field: SerializeField, ReadOnly] public bool IsGrounded { get; private set; }

		public bool IsStandingStill => IsGrounded && CurrentVelocity == Vector2.zero;
		#endregion



		#region State Keeping
		private void WakeSticking()
		{
			StickHits = new RaycastHit2D[MAX_STICK_HITS];
			ColliderRadius = Data.Collider.size.x/2f;
		}

		// Tries to "glue" the Rigidbody to ground that is close enough.
		// This is important for downwards slopes or small steps.
		private bool TryStick()
		{
			if (State != Grounded && State != Sloped)
				return false;

			int count = Physics2D.CircleCastNonAlloc(
				Data.Rigidbody.position + Vector2.up * ColliderRadius,
				ColliderRadius, Vector2.down, StickHits, Data.Config.groundCastDistance,
				Layers.ToMask(Layers.WORLD, Layers.SUPPORT));
			if (count == 0)
				return false;

			RaycastHit2D closest = new RaycastHit2D();
			closest.distance = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				RaycastHit2D hit = StickHits[i];
				if (hit.collider.isTrigger)
					continue;

				if(hit.collider.TryGet(out SnowBall ball))
				{
					if (false == ball.IsOnTop(hit.point))
						continue;
				}

				if (hit.distance < closest.distance)
					closest = hit;
			}

			if (closest.distance == float.MaxValue)
				return false;

			Data.Rigidbody.position += Vector2.down * closest.distance;
			State.Add(new Contact(closest));
			return true;
		}

		private bool ShouldStayAirborne(MoveState newState)
		{
			if (newState != Grounded)
				return false;

			return State == Airborne && Data.Velocity.y > 0;
		}

		private const int MAX_STICK_HITS = 10;
		private RaycastHit2D[] StickHits { get; set; }
		private float ColliderRadius { get; set; }
		#endregion



		#region Slipperyness
		private void CalculateSpeed(float speed, ref int sign)
		{
			float current = Data.RawSpeed * Data.Sign;
			float wanted = speed * sign;

			float result;
			if (wanted == 0)
			{
				float deceleration = SlipDelta;
				if (current > 0)
					result = Mathf.Max(current-deceleration, 0f);
				else
					result = Mathf.Min(current+deceleration, 0f);
			}
			else
			{
				float acceleration = SlipDelta*2f;
				if (wanted > 0)
					result = Mathf.Min(current+acceleration, speed);
				else
					result = Mathf.Max(current-acceleration, -speed);
			}

			sign = result.Sign(true);
			Data.RawSpeed = Mathf.Abs(result);
		}

		protected void UpdateSlipDelta()
		{
			if (Grounded.CurrentSlipperyness == 0f)
				SlipDelta = 1f;
			else
				SlipDelta = SLIP_RANGE.x + (SLIP_RANGE.y - SLIP_RANGE.x) * (1f - Grounded.CurrentSlipperyness);
		}

		// Arbitrary values that give the best feeling while playing.
		private static readonly Vector2 SLIP_RANGE = new Vector2(0.005f, 0.1f);
		[field: SerializeField, ReadOnly] private float SlipDelta { get; set; }
		#endregion



		#if UNITY_EDITOR
		[SerializeField, ReadOnly] private string _currentStateName;
		#endif
	}
}
