using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Base for objects that can receive damage from a
	/// <see cref="DamageSender"/>.
	/// </summary>
	public abstract class DamageReceiver : MonoBehaviour
	{
		[Space, Header("Damage Receiver")]
		[SerializeField] private AttackAttribute _attribute;

		public virtual bool ReactsTo(AttackAttribute attribute) => _attribute.IsAny(attribute);
		public abstract void TakeDamage(AttackInfo info);
		public virtual void Kill(AttackInfo info) { }

		public virtual bool IsInvulnerable => false; // Attacks are ignored completely
		public virtual bool IsInvincible => false; // Attacks hit, but don't harm
		public abstract bool IsDead { get; }
	}
}