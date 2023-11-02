using UnityEngine;
using Modifiers = Taijj.HeartWarming.MoveData.Modifiers;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Interface for entities, that react to <see cref="WaterZone"/>.
	/// </summary>
	public interface IWaterlogged
	{
		public void OnEnterWater(Modifiers modifiers);
		public void OnExitWater();
	}

	/// <summary>
	/// Represents a body of water other entities can react to.
	/// </summary>
	public class WaterZone : MonoBehaviour
	{
		#region Targets
		private class Entry
		{
			private Modifiers Modifiers { get; set; }
			public Entry(Modifiers modifiers) => Modifiers = modifiers;

			public void Enter(IWaterlogged target)
			{
				if(IsEmpty)
				{
					Target = target;
					Target.OnEnterWater(Modifiers);
				}
				Count++;
			}

			public void Exit()
			{
				Count--;
				if (Count <= 0)
				{
					Target.OnExitWater();
					Target = null;
				}
			}

			private int Count { get; set; }
			public IWaterlogged Target { get; private set; }

			public bool IsEmpty => Target == null;
		}

		public void Awake()
		{
			Entries = new Entry[MAX_ENTRIES];
			for (int i = 0; i < MAX_ENTRIES; i++)
				Entries[i] = new Entry(_movementModifiers);
		}

		private const int MAX_ENTRIES = 5;
		private Entry[] Entries { get; set; }
		#endregion



		#region Main
		[SerializeField] private Modifiers _movementModifiers;

		public void OnTriggerEnter2D(Collider2D collider)
		{
			if (false == Addons.TryGet(collider, out IWaterlogged target))
				return;

			for(int i = 0; i < Entries.Length; i++)
			{
				Entry en = Entries[i];
				if (en.Target == target)
				{
					en.Enter(target);
					return;
				}
			}

			for(int i = 0; i < Entries.Length; i++)
			{
				Entry en = Entries[i];
				if (en.IsEmpty)
				{
					en.Enter(target);
					return;
				}
			}
		}

		public void OnTriggerExit2D(Collider2D collision)
		{
			if (false == Addons.TryGet(collision, out IWaterlogged target))
				return;

			for (int i = 0; i < Entries.Length; i++)
			{
				Entry en = Entries[i];
				if (en.Target == target)
				{
					en.Exit();
					return;
				}
			}
		}
		#endregion
	}
}