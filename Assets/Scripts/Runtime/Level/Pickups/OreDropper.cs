using FMODUnity;
using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Helper for dropping <see cref="Ore"/> with configurable settings,
	/// e.g. when an <see cref="Enemy"/> dies or for objects like chests.
	/// </summary>
	public class OreDropper : MonoBehaviour
    {
		#region LifeCycle
		[Tooltip("The radius the ore will spawn in, around this dropper's position.")]
		[SerializeField] private float _radius;
		[Tooltip("How long after spawning is the ore prevented from being collected.")]
		[SerializeField] private float _scatterDuration;
        [Tooltip("Should the ore stay active even when offscreen?")]
        [SerializeField] private bool _ignoreCameraDetection;
		[Space]
		[SerializeField] private Entry[] _entries;

		public void Wake() => Transform = transform;
		private Transform Transform { get; set; }
		#endregion



		#region Dropping
		[Serializable]
		public struct Entry
		{
			public Pickup prefab;
			public int amount;
		}

		public virtual void Drop()
        {
			for(int i = 0; i < _entries.Length; i++)
			{
				Entry entry = _entries[i];
				for (int j = 0; j < entry.amount; j++)
					Drop(entry.prefab);
			}
        }

        protected void Drop(Pickup prefab)
        {
			Level.Collector.SpawnOre(new OreSpawnData
			{
				prefab = (Ore)prefab,
				position = Transform.position,
				radius = _radius,
				scatterDuration = _scatterDuration,
				ignoreDetection = _ignoreCameraDetection
			});
        }
		#endregion



		#if UNITY_EDITOR
		public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere((Vector2)transform.position, _radius);
        }
		#endif
    }
}