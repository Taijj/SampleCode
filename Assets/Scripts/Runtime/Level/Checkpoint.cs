using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// A level checkpoint.
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
    public class Checkpoint : MonoBehaviour
    {
		#region LifeCycle
		private static readonly int REACH_HASH = Animator.StringToHash("Reach");

		[SerializeField, ReadOnly] private Vector2 _respawnPosition;
		[SerializeField] private Collider2D _collider;
		[SerializeField] private Animator _animator;
		[SerializeField] private EventReference _reachedSound;

		public Vector2 RespawnPosition => _respawnPosition;
		#endregion



		#region Triggering
		public void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.layer != Layers.HERO)
				return;

			if (false == collision.TryGet(out Corpus _))
				return;

			Game.Audio.Play(_reachedSound);
			Level.Route.OnReached(this);
		}

		public bool IsReached
		{
			set
			{
				_collider.enabled = !value;
				if (value)
					_animator.SetTrigger(REACH_HASH);
				else
					_animator.Reset();
			}
		}
		#endregion



		#if UNITY_EDITOR
		private const float MAX_CHANGE_DISTANCE = 0.001f;

		public void OnValidate()
		{
			this.TryAssign(ref _collider);
			this.TryAssign(ref _animator);
			SetRespawnPosition();
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(_respawnPosition, 0.25f);
		}

		public void SetRespawnPosition()
		{
			Vector2 castOrigin = (Vector2)_animator.transform.position + Vector2.down * 4f;

			_collider.enabled = false;
			RaycastHit2D hit = Physics2D.Raycast(castOrigin, Vector2.down, 20f, Layers.WORLD.ToMask());
			_collider.enabled = true;
			if (hit)
				_respawnPosition = Vector2.Distance(_respawnPosition, hit.point) > MAX_CHANGE_DISTANCE ? hit.point : _respawnPosition;
			else
				_respawnPosition = castOrigin;
		}
		#endif
	}
}