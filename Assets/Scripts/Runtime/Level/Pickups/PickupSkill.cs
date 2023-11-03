using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// A <see cref="Pickup"/> that unlocks the given Special and
	/// does not respawn.
	/// </summary>
    public class PickupSkill : Pickup
    {
		#region Main
		[Space, Header("Skill")]
		[SerializeField] private SpecialKind _special = SpecialKind.None;
		public override void Respawn() => Deactivate();
		#endregion



		#if UNITY_EDITOR
		public override void OnValidate()
		{
			base.OnValidate();
			KindFromEditor = PickupKind.Skill;
			ValueFromEditor = (int)_special;
		}
		#endif
	}
}