using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Bundled result of a potentially damaging collision.
	/// Created by <see cref="DamageSender"/>
	/// </summary>
    public struct DamageOutcome
    {
		public enum Kind
		{
			Ignored = 0,

			HitSolid,
			Damaged,
			Killed
		}

		public Kind kind;
		public DamageReceiver receiver;
		public AttackInfo info;
	}
}