using FMODUnity;
using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
    /// <summary>
    /// <see cref="Pickup"/> with special features:
    ///  - influenced by gravity
    ///  - has solid physics collision
    ///  - despawns after some time
    ///  - magnet
    /// </summary>
    public class Ore : Pickup
    {
        #region Initialization
        [Space]
        [Header("Ore Settings")]
        [SerializeField] [ReadOnly] private float _scatterDuration;
        [SerializeField] private float _aliveDuration = 1.0f;
        [SerializeField] private float _fadingDuration = 1.0f;
        [Space]
		[SerializeField] private Rigidbody2D _rigidbody;
		[SerializeField] private Collider2D _collider;
		[SerializeField] private Magnet _magnet;
		[Space]
		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private Material _blinkerMaterial;
		[SerializeField] private Material _pickupShineMaterial;

		[SerializeField] private Blinker _blinker;
		[SerializeField] private Blink _fadeBlink;
		[Space]
		[SerializeField] private EventReference _bounceSound;

        public override void Wake()
        {            
			_blinker.Wake();
            _magnet.Wake(_rigidbody);

			_spriteRenderer.sharedMaterial = _pickupShineMaterial;

			base.Wake();
			IsReady = true;
        }
				
		public override void CleanUp() => _blinker.Stop();
				
		public bool IsReady { get; set; }
        #endregion



        #region Unity Messages
        public override void OnFixedUpdate()
        {
			base.OnFixedUpdate();

			if (false == ContentEnabled)
				return;

			if (IsCollected || false == IsCollectible)
				return;

			_collider.enabled = true;
			Hero hero = Level.Hero;
			if (hero.GetIsDead())
				return;

			Vector2 toHero = hero.Pawn.Center - (Vector2)Transform.position;
			float squareDist = toHero.sqrMagnitude;
			if(squareDist < COLLECT_DISTANCE)
			{
				Collect();
				return;
			}
			else if(squareDist < _magnet.SquareRange)
			{
				_magnet.Attract(toHero);
				_collider.enabled = false;
			}
        }

        public override void OnUpdate()
        {
			base.OnUpdate();
            if (CurrentWaitTime == NONE_TIME)
                return;

            if (Time.time < CurrentWaitTime)
                return;

            switch(CurrentState)
            {
                case State.Spawned: TrySetAlive();  break;
                case State.Alive: TrySetFading(); break;
                case State.Fading: Despawn(); break;
            }
        }

        public void OnCollisionEnter2D(Collision2D _)
        {
            // To prevent ore from bouncing into oblivion, if spawned from very high up.
            if (_rigidbody.velocity.y > MAX_BOUNCE_VELOCITY)
				_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, MAX_BOUNCE_VELOCITY);

            if (_rigidbody.velocity.y > BOUNCE_SOUND_TRESHOLD)
                PlaySound(_bounceSound);
        }

		private const float COLLECT_DISTANCE = 0.1f;
        private const float BOUNCE_SOUND_TRESHOLD = 0.2f;
        private const float MAX_BOUNCE_VELOCITY = 10f;
        #endregion



        #region State
        private enum State
        {
            None, // Disabled
            Spawned, // Enabled, not collectible
            Alive, // Enabled, collectible
            Fading // About to disable, collectible
        }

        private void SetNone()
        {
			IsWorkedAround = false;
			CurrentState = State.None;
            CurrentWaitTime = NONE_TIME;
			_blinker.Stop();
        }

        private void TrySetSpawned()
        {
            if (CurrentState == State.Spawned)
                return;

			IsWorkedAround = true;
			CurrentState = State.Spawned;
            CurrentWaitTime = Time.time + _scatterDuration;

			_spriteRenderer.sharedMaterial = _pickupShineMaterial;
			_blinker.Stop();
        }

        private void TrySetAlive()
        {
            if (CurrentState == State.Alive)
                return;

			IsWorkedAround = false;
			CurrentState = State.Alive;
            CurrentWaitTime = Time.time + _aliveDuration;
			_blinker.Stop();
        }

        private void TrySetFading()
        {
            if (CurrentState == State.Fading)
                return;

			IsWorkedAround = false;
            CurrentState = State.Fading;
            CurrentWaitTime = Time.time + _fadingDuration;

			_spriteRenderer.sharedMaterial = _blinkerMaterial;
			_blinker.Do(_fadeBlink);
        }

		/// <summary>
		/// This is a workaround so ores that are still scattering will collide with the solid world
		/// but not with the hero.
		/// </summary>
		private bool IsWorkedAround
		{
			set
			{
				_collider.gameObject.layer = value ? Layers.HERO : Layers.PICKUP;
				_rigidbody.gameObject.layer = value ? Layers.HERO : Layers.PICKUP;
			}
		}

        private const float NONE_TIME = -1f;
        private State CurrentState { get; set; }
        private float CurrentWaitTime { get; set; }

        private bool IsCollectible => CurrentState == State.Alive || CurrentState == State.Fading;
        #endregion



        #region Spawning
        protected override void Collect()
        {
            if(IsCollectible)
			{
                base.Collect();
				Despawn();
			}
        }

        public override void Respawn()
        {
            // The Ore lifecycle is controlled externally. So it is
            // excluded from general respawning and is despawned instead.
            Despawn();
        }

        public void Spawn(OreSpawnData data)
        {
            base.Respawn();

            _scatterDuration = data.scatterDuration;
            TrySetSpawned();

            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.AddForce(data.force, ForceMode2D.Impulse);
			_rigidbody.AddTorque(UnityEngine.Random.Range(-data.torqueForce, data.torqueForce));

            VelocityBeforeDisable = _rigidbody.velocity;
            IsDetectorIgnored = data.ignoreDetection;
        }

        private void Despawn()
        {			
			ContentEnabled = false;
            SetNone();
            OnDespawned?.Invoke(this);
        }

		protected override void Activate()
		{
			ContentEnabled = true;
			if (IsDetectorIgnored)
				return;

			_rigidbody.bodyType = RigidbodyType2D.Dynamic;
			_magnet.Respawn();
			_rigidbody.velocity = VelocityBeforeDisable;
		}

		protected override void Deactivate()
		{
			if(IsDetectorIgnored)
			{
				ContentEnabled = true;
				return;
			}

			VelocityBeforeDisable = _rigidbody.velocity;
			_rigidbody.bodyType = RigidbodyType2D.Static;
			ContentEnabled = false;
		}

		public event Action<Ore> OnDespawned;
		private Vector2 VelocityBeforeDisable { get; set; }
        private bool IsDetectorIgnored { get; set; }
        #endregion



		#if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {
			_magnet.DrawGizmo(transform.position);
        }

		public override void OnValidate()
		{
			base.OnValidate();
			this.TryAssign(ref _rigidbody);
			this.TryAssign(ref _collider);
			this.TryAssign(ref _blinker);
			this.TryAssign(ref _spriteRenderer);
		}
		#endif
	}
}