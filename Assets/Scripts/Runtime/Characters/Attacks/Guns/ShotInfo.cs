
using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Configuration helper to be able to configure the behavior of <see cref="Shot"/>s
	/// before they are spawned. Usually used by <see cref="Attack"/>s.
	/// </summary>
    public class ShotInfo
    {
		public Shot prefab;

		public Vector2 position;
		public Vector2 velocity;
		public float offscreenTolerance;

		public Action<Shot> onDestroyed;
	}
}