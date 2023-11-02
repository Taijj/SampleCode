namespace Taijj.HeartWarming
{
	/// <summary>
	/// Condtion for Special damage. Necessary to ensure, that a dash
	/// or a stomp won't hit multiple times a frame.
	/// </summary>
	public class SpecialCondition : DamageCondition
	{
		private const int MAX_RECEIVERS = 10;
		public SpecialCondition() => Receivers = new DamageReceiver[MAX_RECEIVERS];

		public override bool CanBeDamaged(DamageReceiver receiver) => false == Receivers.Contains(receiver);
		public override void OnDamaged(DamageReceiver receiver)
		{
			Count++;
			Receivers.Add(receiver);
		}

		public void Clear()
		{
			if (Count == 0)
				return;
			Count = 0;

			for (int i = 0; i < Receivers.Length; i++)
				Receivers[i] = null;
		}

		private DamageReceiver[] Receivers { get; set; }
		private int Count { get; set; }
	}
}
