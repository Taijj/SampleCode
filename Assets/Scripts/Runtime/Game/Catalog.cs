using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// First draft of a simple manager for static or dynamic/persisted data.
	/// </summary>
	public class Catalog : MonoBehaviour
	{
		[SerializeField] private PhysicsConfig _physics;
		[SerializeField] private Special _special;
		[SerializeField] private Treasure _treasure;

		public PhysicsConfig Physics => _physics;
		public Special Special => _special;
		public Treasure Treasure => _treasure;

		#if UNITY_EDITOR
		private void OnValidate()
		{
			_special.DispatchChange();
		}
		#endif
	}

	[Serializable]
	public class PhysicsConfig
	{
		[Header("Grounded")]
		public float gravity;
		public float maxFallSpeed;
		public float groundCastDistance;

		[Header("Slopes & Walls")]
		public float slopeSpeed;
		[Range(0f, 0.2f)] public float slopeFriction;
		public float slopeAngle;
		public float wallAngle;

		//+1 Tolerance, so the Character won't get stuck on 90° Tile walls.
		public const float CEILING_ANGLE = 91f;
		// Allows for nicer numbers in the inspector.
		public const float AMPLIFIER = 100f;
	}

	[Serializable]
	public class Special
	{
		[Serializable]
		public class Skill
		{
			public SpecialKind specialKind;
			public bool isUnlocked;
			public float manaCost;
		}

		public event Action OnUnlockChanged;

		public Skill[] skills;

		public Skill GetSkill(SpecialKind specialKind)
		{
			for (int i = 0; i < skills.Length; i++)
			{
				if (skills[i].specialKind == specialKind)
					return skills[i];
			}
			throw new Exception("No skill of the given kind was found");

		}

		public void Unlock(SpecialKind kind)
		{
			GetSkill(kind).isUnlocked = true;
			DispatchChange();
		}

		public void DispatchChange() => OnUnlockChanged?.Invoke();

		public void ResetAllUnlocks()
		{
			for (int i = 0; i < skills.Length; i++)
			{
				skills[i].isUnlocked = false;
			}
		}
	}

	[Serializable]
	public class Treasure
	{
		public const int MAX_ORE_VALUE = 9999;

		public int oreBronze;
		public int oreSilver;
		public int oreGold;

		public void SetOreValue(Pickup pickup)
		{
			switch (pickup.Kind)
			{
				case PickupKind.OreBronze: if(oreBronze < MAX_ORE_VALUE) oreBronze += (int)pickup.Value; break;
				case PickupKind.OreSilver: if (oreSilver < MAX_ORE_VALUE) oreSilver += (int)pickup.Value; break;
				case PickupKind.OreGold: if (oreGold < MAX_ORE_VALUE) oreGold += (int)pickup.Value; break;
			}
			DispatchChange();
		}

		public void DispatchChange() => OnTreasureChanged?.Invoke();

		public event Action OnTreasureChanged;
	}
}