using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Encapsulates and manages user input to be relayed and preprocessed for
	/// the <see cref="Hero"/>.
	/// </summary>
	public class UserInputProvider : InputProvider
	{
		#region Main
		[SerializeField] private HeroMap _heroMap;
		[SerializeField] private float _moveDeadzone;
		[SerializeField] private float _interactDeadzone;
		[SerializeField] private int _interactFrames;

		public override void Enable()
		{
			_heroMap.OnJumpPress += OnJumpPressInput;
			_heroMap.OnJumpRelease += OnJumpReleaseInput;
			_heroMap.OnShootFire += OnShootFireInput;
			_heroMap.OnShootIce += OnShootIceInput;
			_heroMap.OnSpecial += OnSpecialInput;
			_heroMap.OnSelectSpecial += OnSelectSpecialInput;

			_heroMap.OnCheat += OnCheat;

			Game.Input.PushMap(_heroMap);

			_interactFrames = Mathf.Max(1, _interactFrames);
		}

		public override void Disable()
		{
			_heroMap.OnJumpPress -= OnJumpPressInput;
			_heroMap.OnJumpRelease -= OnJumpReleaseInput;
			_heroMap.OnShootFire -= OnShootFireInput;
			_heroMap.OnShootIce -= OnShootIceInput;
			_heroMap.OnSpecial -= OnSpecialInput;
			_heroMap.OnSelectSpecial -= OnSelectSpecialInput;

			_heroMap.OnCheat -= OnCheat;

			Game.Input.PopMap();
		}

		private bool TryPerform(Interactor interactor)
		{
			bool canInteract = Level.Hero.Pawn.IsGrounded
				&& false == IsBreathingFire
				&& false == IsBreathingIce;

			InteractCounter = canInteract ? InteractCounter+1 : 0;
			if (InteractCounter > _interactFrames)
			{
				interactor.Perform();
				InteractCounter = 0;
				return true;
			}
			return false;
		}

		private float InteractCounter { get; set; }
		#endregion



		#region Directions
		public override void OnUpdate()
		{
			DetermineMove(out Vector2 rawInput);
			DetermineVerticalInputs();

			BreathDirection = rawInput;
			IsBreathingFire = _heroMap.IsBreathingFire;
			IsBreathingIce = _heroMap.IsBreathingIce;

			Level.Cameraman.LookInput = _heroMap.LookValue;
		}

		private void DetermineMove(out Vector2 raw)
		{
			raw = _heroMap.MoveValue;

			if (_heroMap.IsStandingBreath)
			{
				Move = Vector2.zero;
				return;
			}

			float x = Mathf.Abs(raw.x) > _moveDeadzone ? raw.x : 0f;
			float y = Mathf.Abs(raw.y) > _moveDeadzone ? raw.y : 0f;
			Move = new Vector2(x, y);
		}

		private void DetermineVerticalInputs()
		{
			IsUpHeld = Move.y > 0f;
			IsDownHeld = Move.y < 0f;
		}
		#endregion



		#region Wrapping
		// These are necessary to make it possible to set the base class'
		// methods to null in order to "unassign" them. Otherwise setting
		// them to null won't have any effect.
		private void OnJumpPressInput() => OnJumpPress?.Invoke();
		private void OnJumpReleaseInput() => OnJumpRelease?.Invoke();
		private void OnShootFireInput() => OnShootFire?.Invoke();
		private void OnShootIceInput() => OnShootIce?.Invoke();

		private void OnSpecialInput()
		{
			if(TryGetInteractor(out Interactor interactor))
			{
				InteractCounter = _interactFrames;
				if(TryPerform(interactor))
					return;
			}
			OnSpecial?.Invoke();
		}

		private void OnSelectSpecialInput(SpecialKind skillKind)
		{
			if (Game.Catalog.Special.GetSkill(skillKind).isUnlocked)
				OnSelectSpecial?.Invoke(skillKind);
		}
		#endregion



		#region Cheats
		private void OnCheat(DevCheat cheat)
		{
			Hero hero = Level.Hero;
			switch (cheat)
			{
				case DevCheat.HealHero: hero.Heal(2f); break;
				case DevCheat.KillHero: hero.Kill(); break;

				case DevCheat.DamageHero:

					AttackInfo info = new AttackInfo();
					info.Strength = 1f;
					info.Direction = Vector2.left * hero.Pawn.Facing;
					hero.Pawn.TakeDamage(info);
					break;

				case DevCheat.GodMode:
					hero.godMode = !hero.godMode;
					break;

				case DevCheat.InfiniteMana:
					hero.Mana.isInfinite = !hero.Mana.isInfinite;
					hero.Mana.TopOff();
					break;

				case DevCheat.UnlockAll:
					Special.Skill[] skills = Game.Catalog.Special.skills;
					bool areUnlocked = !skills[0].isUnlocked;
					for (int i = 0; i < skills.Length; i++)
						skills[i].isUnlocked = areUnlocked;
					Game.Catalog.Special.DispatchChange();
				break;

				case DevCheat.TeleportHero:
					Level.Route.TeleportHero();
                    break;

				case DevCheat.ToggleMusic:
					Game.Audio.ToggleMusic();
					break;
			}
		}
		#endregion
	}
}