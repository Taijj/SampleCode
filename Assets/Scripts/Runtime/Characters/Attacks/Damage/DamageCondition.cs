namespace Taijj.HeartWarming
{
	/// <summary>
	/// Base to help defining entity specific conditions for damage
	/// sending/retreaval. See <see cref="DamageSender"/>.
	/// </summary>
    public class DamageCondition
    {
		public static DamageCondition Empty => new DamageCondition();

		public virtual bool CanBeDamaged(DamageReceiver receiver) => true;
		public virtual void OnDamaged(DamageReceiver receiver) {}
    }
}