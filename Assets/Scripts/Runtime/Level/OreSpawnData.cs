using UnityEngine;

namespace Taijj.HeartWarming
{
    public struct OreSpawnData
    {
        public Ore prefab;
		public Vector2 position;
		public float radius;
        public float scatterDuration;
        public bool ignoreDetection;

        public Vector2 force;
		public float torqueForce;
    }
}