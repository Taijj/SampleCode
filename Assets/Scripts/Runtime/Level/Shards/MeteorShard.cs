using FMODUnity;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Interactive support element that explodes when shot with any fire type attack.
	/// </summary>
	public class MeteorShard : DamageReceiver, IHeatable
	{
		#region LifeCycle
		[Header("Visuals")]
		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private Sprite[] _sprites;
		[SerializeField] private CircleCollider2D _collider;
		[SerializeField] private CircleCollider2D _explosionCollider;
		[SerializeField] private Animator _animator;
		[SerializeField] private GameObject _idleEffects;

		[Space, Header("Explosion")]
		[SerializeField] private DamageSender _damageSender;
		[SerializeField] private Heater _heater;
		[SerializeField] private EventReference _explodeSound;

		[Space]
		[SerializeField] private Resource _health;

		public override bool IsDead => _health.IsEmpty;
		private Transform Transform { get; set; }



		public void Wake()
		{
			Transform = transform;

			AttackInfo info = new AttackInfo();
			info.Flinch = FlinchKind.Volume;
			_damageSender.Wake(info);			
			_heater.Wake();

			AssignRandomSprite();

			_explosionCollider.enabled = false;
		}

		private void AssignRandomSprite()
		{
			int index = Random.Range(0, _sprites.Length);
			_spriteRenderer.sprite = _sprites[index];
		}

		public void CleanUp()
		{
			_isUpdating = false;
		}

		public void Respawn()
		{
			this.Activate();

			_health.TopOff();
			_animator.Reset();

			_idleEffects.Activate();

			_spriteRenderer.Activate();

			_collider.enabled = true;
			_explosionCollider.enabled = false;
			_isUpdating = false;
		}

		public override void TakeDamage(AttackInfo info)
		{
			_health.Consume(info.Strength);
			if (IsDead)
				Explode();
		}

		public void Heat(float amount)
		{
			if (amount > 0)
				Explode();
		}
		#endregion



		#region Explosion
		private static readonly int EXPLODE_HASH = Animator.StringToHash("Explode");

		private float _currentTime;
		private bool _isUpdating;

		private void Explode()
		{
			_collider.enabled = false;
			_explosionCollider.enabled = true;

			_heater.Heat();
			_damageSender.OnUpdate();

			_spriteRenderer.Deactivate();

			_isUpdating = true;
						
			_idleEffects.Deactivate();

			Level.Cameraman.Shake.Do(CameraShake.Normal);
			Game.Audio.Play(_explodeSound, Transform);
		}

		public void OnUpdate()
		{
			if (false == _isUpdating)
				return;
		}
		#endregion



		#if UNITY_EDITOR
		[Space, Button(nameof(RandomizeSprite))]
		public bool randomizeSprite;

		public void RandomizeSprite()
		{
			UnityEditor.Undo.RecordObject(this, name);
			AssignRandomSprite();
		}

		private void OnValidate()
		{
			_collider = GetComponent<CircleCollider2D>();
		}
		#endif
	}
}
