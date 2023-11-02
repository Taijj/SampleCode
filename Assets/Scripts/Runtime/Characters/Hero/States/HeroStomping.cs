using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Handles the hero's Ice Stomp Special.
	/// </summary>
	public class HeroStomping : HeroState
	{
		#region LifeCycle
		[SerializeField] private Rigidbody2D _rigidbody;
		[SerializeField] private Corpus _corpus;
		[SerializeField] private HeroMovement _movement;
		[SerializeField] private IceStomp _stomp;
		[Space]
		[SerializeField] private float _maxSpeed;
		[SerializeField] private float _acceleration;
		[SerializeField] private float _recoilForce;
		[SerializeField] private float _invulDuration;
		[Space]
		[SerializeField] private EventReference _performSound;
		[SerializeField] private EventReference _bounceSound;
		[SerializeField] private EventReference _impactSound;

		public override void Wake(StateData data)
		{
			base.Wake(data);
			_stomp.Wake(Recoil);
		}

		public override void CleanUp() => _stomp.CleanUp();
		#endregion



		#region Flow
		public override void Enter()
		{
			Pawn.Stop();
			Pawn.Animator.Set(IS_STOMPING_HASH, true);
			Speed = _maxSpeed;
						
			_stomp.Begin();
			Game.Audio.Play(_performSound, Pawn.Transform);
		}

		public override void OnUpdate() => _stomp.UpdateDamager();

		public override void OnFixedUpdate()
		{
			if (TryComplete())
				return;

			Speed = Mathf.Min(Speed + _acceleration, _maxSpeed);
			float speed = Speed * Game.Updater.AmplifiedFixedDeltaTime;
			_rigidbody.velocity = Vector2.down * speed;

			_corpus.Contacts.Clear();			
		}

		private void Recoil()
		{
			Complete();
			_movement.OverrideJumpCount(1);
			Pawn.Push(Vector2.up, _recoilForce);
		}

		private static readonly int IS_STOMPING_HASH = Animator.StringToHash("IsStomping");
		private float Speed { get; set; }
		#endregion



		#region Impact & Completion
		private bool TryComplete()
		{
			if (WillHitWater())
			{
				Complete();
				return true;
			}

			for (int i = 0; i < _corpus.Contacts.Count; i++)
			{
				float angle = _corpus.Contacts[i].angle;
				if (angle < Game.Catalog.Physics.slopeAngle)
				{
					Complete();
					return true;
				}
			}
			return false;
		}

		private bool WillHitWater()
		{			
			if (_rigidbody.velocity.y > 0)
				return false;
						
			Vector2 origin = _rigidbody.position;
			float distance = Mathf.Abs(_rigidbody.velocity.y * Time.fixedDeltaTime);

			Note.DrawRay(origin, Vector2.down * distance, Color.magenta);

			LayerMask mask = Layers.SNOWBOX.ToMask() | Layers.WORLD.ToMask();
			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, distance, mask);
			if (false == hit)
				return false;
						
			if (false == hit.collider.TryGet(out WaterZone _))
				return false;

			_rigidbody.position = hit.point;
			return true;
		}

		private void Complete(MoveData.Modifiers _ = null)
		{
			Game.Audio.Play(_impactSound, Pawn.Transform);

			_rigidbody.velocity = Vector2.zero;
			_stomp.Impact();
			Pawn.Animator.Set(IS_STOMPING_HASH, false);

			Invul.StartHiddenInvul(_invulDuration);
			Transit(typeof(HeroMoving));
		}
		#endregion
	}
}