using FMODUnity;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// A rolling ball of snow, that can be frozen and melted.
	/// Grows while rolling and will cause defined water basins
	/// to fill with water, when melted.
	/// </summary>
	public class SnowBall : MonoBehaviour, IHeatable
	{
		#region LifeCycle
		[System.Serializable]
		public class BallSprite
		{
			public SpriteRenderer renderer;
			public Sprite snowSprite;
			public Sprite iceSprite;

			public float scale;
		}

		[SerializeField] private Rigidbody2D _rigidbody;
		[SerializeField] private BallSprite[] _sprites;
		[SerializeField] private CircleCollider2D _worldCollider;
		[SerializeField] private Collider2D _snowBoxCollider;
		[SerializeField] private CameraDetector _detector;
		[Space]
		[SerializeField, Range(0f, 1f)] private float _growthRate;
		[SerializeField] private FloatRange _scaleRange;
		[SerializeField] private float _topForce;
		[SerializeField] private float _groundCheckDistance;
		[Space]
		[SerializeField] private float _defaultHeat;
		[SerializeField] private float _shrinkByHeat;
		[SerializeField] private float _waterByMelt;
		[Space]
		[SerializeField] private EventReference _rollLoop;
		[SerializeField] private EventReference _colliderSound;
		[SerializeField] private EventReference _freezeSound;
		[SerializeField] private EventReference _meltSound;
		[Space]
		[SerializeField] private float _collideSoundMinSpeed;

		private const float GROWTH_DAMP = 10f;



		public void Wake()
		{
			Transform = transform;
			DefaultPosition = Transform.position;
			DefaultMass = _rigidbody.mass;

			AudioLoop = new AudioLoop(_rollLoop, Transform);
			_detector.Wake(Activate, Deactivate);
			
			ResetSelf();
		}

		public void Respawn()
		{
			Transform.position = DefaultPosition;
			Transform.rotation = Quaternion.identity;
			_rigidbody.velocity = Vector2.zero;
			_rigidbody.angularVelocity = 0f;

			ResetSelf();
		}

		private void ResetSelf()
		{
			Scale = _scaleRange.Min;
			CurrentHeat = _defaultHeat;
			Transform.localScale = Vector2.one * Scale;

			LastDelta = Vector2.zero;
			LastPosition = Transform.position;
			AudioLoop.Stop();
			IsFrozen = false;

			UpdateGrounded();
			UpdateSprites();
			UpdateMass();
		}

		public void CleanUp() => AudioLoop.Destroy();

		private Transform Transform { get; set; }
		private Vector2 DefaultPosition { get; set; }
		private AudioLoop AudioLoop { get; set; }
		#endregion



		#region Activate/Deactivate
		private void Activate()
		{
			gameObject.Activate();
			_rigidbody.velocity = VelocityBeforeDeactivate;
			_rigidbody.angularVelocity = AngularVelocityBeforeDeactivate;			
		}

		private void Deactivate()
		{
			VelocityBeforeDeactivate = _rigidbody.velocity;
			AngularVelocityBeforeDeactivate = _rigidbody.angularVelocity;
			gameObject.Deactivate();
		}		

		private Vector2 VelocityBeforeDeactivate { get; set; }
		private float AngularVelocityBeforeDeactivate { get; set; }
		#endregion



		#region Main Loop
		public void OnFixedUpdate()
		{
			_detector.OnUpdate();
			if (false == gameObject.activeSelf)
				return;

			UpdateGrounded();
			TryGrow();
			UpdateMass();
			UpdateSprites();
			UpdateExtends();
			UpdateAudio();

			NormalizedScale = (Scale - _scaleRange.Min)/_scaleRange.Delta;
			Transform.localScale = Vector2.one * Scale;
			WasGrounded = IsGrounded;
		}

		private void TryGrow()
		{
			bool willGrow = _rigidbody.velocity != Vector2.zero
				&& IsGrounded
				&& false == IsFrozen;
			if (false == willGrow)
				return;

			float delta = _rigidbody.velocity.magnitude;
			float growth = delta/GROWTH_DAMP * _growthRate * Time.deltaTime;
			Scale = Mathf.Clamp(Scale+growth, _scaleRange.Min, _scaleRange.Max);
		}

		private void UpdateSprites()
		{
			for (int i = 0; i < _sprites.Length; i++)
			{
				BallSprite sp = _sprites[i];

				float nextScale = i == _sprites.Length-1 ? float.MaxValue : _sprites[i+1].scale;
				bool isActive = Scale >= sp.scale && Scale < nextScale;
				if (isActive)
				{
					sp.renderer.Activate();
					sp.renderer.sprite = IsFrozen ? sp.iceSprite : sp.snowSprite;
				}
				else
				{
					sp.renderer.Deactivate();
				}
			}
		}

		private void UpdateMass()
		{
			float iceFactor = IsFrozen ? 2f : 1f;
			_rigidbody.mass = DefaultMass * Scale * iceFactor;
		}			

		private void UpdateGrounded()
		{
			float radius = _worldCollider.radius * transform.localScale.x;
			RaycastHit2D hit = Physics2D.CircleCast(_rigidbody.position, radius,
				Vector2.down, _groundCheckDistance, Layers.WORLD.ToMask());			
			IsGrounded = hit;
		}

		private float Scale { get; set; }
		private float NormalizedScale { get; set; }
		private float DefaultMass { get; set; }
				
		[field: Space, SerializeField] private bool IsGrounded { get; set; }
		private bool WasGrounded { get; set; }
		#endregion



		#region Audio
		private void UpdateAudio()
		{	
			UpdateAudioLoop();
			TryPlayCollideSound();
		}

		private void UpdateAudioLoop()
		{
			if(false == gameObject.activeSelf)
			{
				AudioLoop.Stop();
				return;
			}

			bool becameAirborne = WasGrounded && false == IsGrounded;
			if (becameAirborne)
			{				
				AudioLoop.Stop();
				return;
			}

			float speed = _rigidbody.velocity.magnitude;
			AudioLoop.Play();
			AudioLoop.OnUpdate(SPEED_PARAMETER, speed/LOOP_MAX_SPEED);
			AudioLoop.OnUpdate(SIZE_PARAMETER, NormalizedScale);
		}

		private void TryPlayCollideSound()
		{
			void PlaySound()
			{				
				Game.Audio.Play(_colliderSound, Transform);
				Game.Audio.SetParameterOf(_colliderSound, SIZE_PARAMETER, NormalizedScale);
			}

			void UpdateLasts(Vector2 delta, float speed)
			{
				LastPosition = Transform.position;
				LastDelta = delta;
				LastSpeed = speed;
			}

			Vector2 delta = LastPosition - (Vector2)Transform.position;
			float speed = delta.magnitude;
			if (speed < _collideSoundMinSpeed)
			{
				float speedDelta = Mathf.Abs(LastSpeed - speed);
				if (speedDelta > _collideSoundMinSpeed)
					PlaySound();
				UpdateLasts(delta, speed);
				return;
			}

			bool becameGrounded = false == WasGrounded && IsGrounded;
			if (becameGrounded)
			{
				PlaySound();
				UpdateLasts(delta, speed);
				return;
			}			

			UpdateLasts(delta, speed);
		}		

		public const string SPEED_PARAMETER = "Speed";
		public const string SIZE_PARAMETER = "Size";			
		
		private const float COLLIDE_SOUND_ANGLE = 40;
		private const float LOOP_MAX_SPEED = 14f;	
		
		private Vector2 LastPosition { get; set; }
		private Vector2 LastDelta { get; set; }
		private float LastSpeed { get; set; }
		#endregion



		#region On Top Check
		private void UpdateExtends()
		{
			Vector2 pos = Transform.position;
			LeftX = pos.x - Scale/2f;
			RightX = pos.x + Scale/2f;
		}

		public bool IsOnTop(Vector2 position)
		{
			if (position.y < Transform.position.y)
				return false;
			return position.x.IsBetween(LeftX, RightX);
		}

		public void AddTopForce(Vector2 point)
		{
			if (false == IsOnTop(point))
				return;

			if (point.x.IsAround(Transform.position.x, 0.5f))
				return;

			_rigidbody.AddForceAtPosition(Vector2.down * _topForce, point, ForceMode2D.Impulse);
		}

		private float LeftX { get; set; }
		private float RightX { get; set; }
		#endregion
		


		#region Heat
		public void Heat(float amount)
		{
			CurrentHeat = Mathf.Clamp(CurrentHeat + amount, -_defaultHeat, _defaultHeat);

			if(IsFrozen && CurrentHeat > 0)
			{
				IsFrozen = false;
				Game.Audio.Play(_meltSound, Transform);
			}
			else if(false == IsFrozen && CurrentHeat <= 0)
			{
				IsFrozen = true;
				Game.Audio.Play(_freezeSound, Transform);
			}

			TryMelt(amount);
			UpdateSprites();
			UpdateMass();
		}

		private void TryMelt(float heatedAmount)
		{
			bool canMelt = heatedAmount > 0
				&& false == IsFrozen
				&& Scale > _scaleRange.Min;
			if(false == canMelt)
				return;

			float reduce = _shrinkByHeat/GROWTH_DAMP * heatedAmount * Time.deltaTime;
			float scale = Scale - reduce;
			Scale = Mathf.Max(scale, _scaleRange.Min);						

			TryAddWater();
		}

		private void TryAddWater()
		{			
			Vector2 origin = (Vector2)Transform.position + Vector2.up*WATER_CHECK_DISTANCE;

			_snowBoxCollider.enabled = false;
			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, WATER_CHECK_DISTANCE*2f, Layers.SNOWBOX.ToMask());
			_snowBoxCollider.enabled = true;

			if (false == hit)
				return;

			if (hit.collider.TryGet(out SnowBoxTile tile))
				tile.AddWater(_waterByMelt);
		}

		private const float WATER_CHECK_DISTANCE = 10f;
		[field: SerializeField, ReadOnly] private float CurrentHeat { get; set; }
		private bool IsFrozen { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			this.TryAssign(ref _rigidbody);
			this.TryAssign(ref _detector);
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			float halfScale = transform.localScale.x/2f;
			GizmoAddons.DrawLineVertical(transform.position + Vector3.left*halfScale);
			GizmoAddons.DrawLineVertical(transform.position + Vector3.right*halfScale);

			Gizmos.color = Color.yellow;
			float radius = _worldCollider.radius * transform.localScale.x;
			Gizmos.DrawWireSphere(_rigidbody.position, radius);
			Gizmos.DrawWireSphere(_rigidbody.position + Vector2.down * _groundCheckDistance, radius);
		}



		[Button(nameof(TestAddWater))]
		public bool addWaterTarget;
		public void TestAddWater() => TryAddWater();
		#endif
	}
}