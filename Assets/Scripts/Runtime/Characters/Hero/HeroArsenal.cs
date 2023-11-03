using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Controller for all the <see cref="Hero"/>'s attacks.
	/// </summary>
    public class HeroArsenal : MonoBehaviour
    {
		#region LifeCycle
		public class ArsenalData
		{
			public Resource mana;
			public Action<Type> transitToState;
			public Action<Mortar> throwBomb;
		}

		[SerializeField] private Gun _gunFire;
		[SerializeField] private Gun _gunIce;
		[SerializeField] private Breath _breathFire;
		[SerializeField] private Breath _breathIce;
		[Space]
		[SerializeField] private Mortar _mortarFire;
		[SerializeField] private Mortar _mortarIce;
		[SerializeField] private float _mortarsCoolDownDelay;

		public void Wake(ArsenalData data)
		{
			Data = data;
			InputProvider.OnSelectSpecial += SetSelectedSpecial;

			_gunFire.Wake();
			_gunIce.Wake();
			_breathFire.Wake();
			_breathIce.Wake();
			_mortarFire.Wake();
			_mortarIce.Wake();

			MortarCoolOff = new DelayedCall(OnMortarsCooledOff, _mortarsCoolDownDelay);
			CanFireMortar = true;
		}

		public void CleanUp()
		{
			InputProvider.OnSelectSpecial -= SetSelectedSpecial;

			_gunFire.CleanUp();
			_gunIce.CleanUp();
			_breathFire.CleanUp();
			_breathIce.CleanUp();
			_mortarFire.CleanUp();
			_mortarIce.CleanUp();
		}

		private ArsenalData Data { get; set; }
		#endregion



		#region Guns
		public bool TryShootFire()
		{
			if (IsBreathing)
				return false;

			_gunFire.TryPerform();
			_gunFire.Cease();
			return true;
		}

		public bool TryShootIce()
		{
			if (IsBreathing)
				return false;

			_gunIce.TryPerform();
			_gunIce.Cease();
			return true;
		}
		#endregion



		#region Breaths
		private bool _isBreathingFire;
		public bool IsBreathingFire
		{
			set
			{
				if (_isBreathingIce)
					return;

				_isBreathingFire = value;
				if (_isBreathingFire)
					_breathFire.TryPerform();
				else
					_breathFire.Cease();
			}
		}

		private bool _isBreathingIce;
		public bool IsBreathingIce
		{
			set
			{
				if (_isBreathingFire)
					return;

				_isBreathingIce = value;
				if (_isBreathingIce)
					_breathIce.TryPerform();
				else
					_breathIce.Cease();
			}
		}

		public Vector2 BreathRotationInput
		{
			set
			{
				_breathFire.RotationInput = value;
				_breathIce.RotationInput = value;
			}
		}

		public bool IsBreathing => _isBreathingIce || _isBreathingFire;
		#endregion



		#region Specials
		public void SetSelectedSpecial(SpecialKind skillKind)
		{
			CanFireMortar = true;
			MortarCoolOff.Stop();

			SelectedSpecial = skillKind;
		}

		public void PerformSpecial()
		{
			if (IsBreathing || SelectedSpecial == SpecialKind.None)
				return;

			Special.Skill skill = Game.Catalog.Special.GetSkill(SelectedSpecial);
			float manaCost = skill.manaCost;
			if (manaCost > Data.mana.Normalized)
				return;

			bool isManaConsumed = false;
			switch (SelectedSpecial)
			{
				case SpecialKind.IceBomb: TryFireMortar(_mortarIce, out isManaConsumed); break;
				case SpecialKind.FireBomb: TryFireMortar(_mortarFire, out isManaConsumed); break;

				case SpecialKind.FireDash: TryDash(out isManaConsumed); break;
				case SpecialKind.IceStomp: TryStomp(out isManaConsumed); break;
			}

			if(isManaConsumed)
				Data.mana.Consume(manaCost);
		}

		private void TryFireMortar(Mortar mortar, out bool isExecuted)
		{
			isExecuted = CanFireMortar;
			if (isExecuted)
			{
				CanFireMortar = false;
				MortarCoolOff.Restart();
				Data.throwBomb(mortar);
			}
		}

		private void OnMortarsCooledOff() => CanFireMortar = true;

		private void TryStomp(out bool isExecuted)
		{
			isExecuted = false;
			if (Level.Hero.Pawn.IsGrounded)
				return;

			Data.transitToState(typeof(HeroStomping));
			isExecuted = true;
		}

		private void TryDash(out bool isExecuted)
		{
			isExecuted = true;
			Data.transitToState(typeof(HeroDashing));
		}

		private SpecialKind SelectedSpecial { get; set; }
		private DelayedCall MortarCoolOff { get; set; }
		private bool CanFireMortar { get; set; }
		#endregion
	}
}