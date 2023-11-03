using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Component for Testing Ore dropping.
	/// </summary>
    public class DummyOreDropper : DamageReceiver
    {
		[Space, Header("Dropping")]
		[SerializeField] private OreDropper _dropper;
		public void Awake() => _dropper.Wake();
		public override void TakeDamage(AttackInfo info) => _dropper.Drop();
		public override bool IsDead => false;
	}
}
