
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// <see cref="DamageCondition"/> for the specifics of a <see cref="Breath"/>.
	/// Prevents damaging of targets each frame.
	/// </summary>
	public class BreathCondition : DamageCondition
    {
		#region LifeCycle
		private class Target
		{
			public bool isActive;

			public int instanceID;
			public float nextDamageTime;
		}

		public BreathCondition(float interval)
		{
			Targets = new Target[SIMULTANEOUS_TARGETS];
			for (int i = 0; i < SIMULTANEOUS_TARGETS; i++)
				Targets[i] = new Target();

			Interval = interval;
		}

		public void OnUpdate()
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				Target target = Targets[i];
				if (target.isActive)
					target.isActive = Time.time < target.nextDamageTime;
			}
		}

		private const int SIMULTANEOUS_TARGETS = 5;
		private Target[] Targets { get; set; }
		#endregion



		#region Condition
		public override bool CanBeDamaged(DamageReceiver receiver)
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				Target target = Targets[i];
				bool wasDamagedBefore = target.instanceID == receiver.GetInstanceID();
				if (wasDamagedBefore && target.isActive)
					return false;
			}
			return true;
		}

		public override void OnDamaged(DamageReceiver receiver)
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				Target target = Targets[i];
				if (target.isActive)
					continue;

				target.instanceID = receiver.GetInstanceID();
				target.nextDamageTime = Time.time + Interval;
				target.isActive = true;
				return;
			}
		}

		private float Interval { get; set; }
		#endregion
	}
}