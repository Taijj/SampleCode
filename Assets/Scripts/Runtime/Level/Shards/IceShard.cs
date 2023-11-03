using FMODUnity;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Interactive support element that transforms into a platform when hit with any fire type attack
	/// </summary>
	public class IceShard : DamageReceiver
	{
		#region LifeCycle
		[Space, Header("Shard")]
		[SerializeField] private GameObject _platform;
		[SerializeField] private GameObject _shard;
		[SerializeField] private SpriteRenderer _shardRenderer;
		[SerializeField] private Sprite[] _shardSprites;		
		[Space]
		[SerializeField] private EventReference _toShardSound;
		[SerializeField] private EventReference _toPlatformSound;

		private bool _isPlatform;
		public override bool IsDead => false;

		public void Wake()
		{
			Transform = transform;
			AssignRandomSprite();
		}

		private void AssignRandomSprite()
		{
			int index = Random.Range(0, _shardSprites.Length);
			_shardRenderer.sprite = _shardSprites[index];
		}

		public void SetUp() => MorphToShard(true);

		public void Respawn() => MorphToShard(true);
				
		public override void TakeDamage(AttackInfo info)
		{
			bool canBecomeShard = _isPlatform && info.Attributes.IsAny(AttackAttribute.Fire);
			bool canBecomePlatform = !_isPlatform && info.Attributes.IsAny(AttackAttribute.Ice);

			if (canBecomeShard)
				MorphToShard();
			else if (canBecomePlatform)
				MorphToPlatform();
		}

		private Transform Transform { get; set; }
		#endregion



		#region Transformation
		private void MorphToPlatform()
		{
			_shard.Deactivate();
			_platform.Activate();
			_isPlatform = true;
						
			Game.Audio.Play(_toPlatformSound, Transform);
		}

		private void MorphToShard(bool isSilent = false)
		{
			_shard.Activate();
			_platform.Deactivate();
			_isPlatform = false;

			if(false == isSilent)
				Game.Audio.Play(_toShardSound, Transform);
		}
		#endregion



		#if UNITY_EDITOR
		[Space, Button(nameof(RandomizeShardSprite))]
		public bool randomizeShardSprite;

		[Button(nameof(ChangeState))]
		public bool changeState;

		public void RandomizeShardSprite()
		{
			UnityEditor.Undo.RecordObject(this, name);
			AssignRandomSprite();
		}

		public void ChangeState()
		{
			UnityEditor.Undo.RecordObject(this, name);
			if (_isPlatform)
				MorphToShard();
			else
				MorphToPlatform();
		}
		#endif
	}
}
