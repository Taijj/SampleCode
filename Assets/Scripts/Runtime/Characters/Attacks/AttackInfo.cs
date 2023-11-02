
using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	[Flags]
	public enum AttackAttribute
	{
		Fire = 1,
		Ice = 2,
		Neutral = 4
	}

	/// <summary>
	/// Determines how the <see cref="Hero"/> flinches when hit by an attack.
	/// Default: Makes a small jump left or right.
	/// Volume: Is pushed outwards from the damaging collider's center.
	/// </summary>
	public enum FlinchKind
	{
		Default = 0,
		Volume
	}

	/// <summary>
	/// Data container, that is handed and filled through multiple layers of classes until
	/// it ends up in the hands of a <see cref="DamageReceiver"/>, which then uses the
	/// given information as necessary.
	/// </summary>
	public class AttackInfo
    {
		public void AddAttribute(AttackAttribute value) => Attributes |= value;
		public AttackAttribute Attributes { get; private set; }

		public FlinchKind Flinch { get; set; }
		public Contact Contact { get; set; }
		public Vector2 Direction { get; set; }
		public float Strength { get; set; }
		public int SourceLayer { get; set; }
	}
}