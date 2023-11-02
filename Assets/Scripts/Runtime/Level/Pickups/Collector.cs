using System.Collections.Generic;
using UnityEngine;

namespace Taijj.HeartWarming
{
    /// <summary>
    /// Controls the LifeCycle and collection of <see cref="Pickup"/>s. Also
	/// takes care of some special treatement for <see cref="Ore"/>s.
    /// </summary>
    public class Collector : MonoBehaviour
    {
		#region LifeCycle
		private const int SPAWNED_ORE_CAPACITY = 50;

		[SerializeField] private Pickup[] _staticPickups;
		[SerializeField] private Chest[] _chests;
		[Space]
        [SerializeField] private FloatRange _oreHorizontalForceRange;
        [SerializeField] private FloatRange _oreVerticalForceRange;
		[SerializeField] private float _oreTorqueForce;

		public void Wake()
		{
			for (int i = 0; i < _staticPickups.Length; i++)
				_staticPickups[i].Wake();

			for(int i = 0; i < _chests.Length; i++)
				_chests[i].Wake();

			SpawnedPickups = new List<Ore>(SPAWNED_ORE_CAPACITY);
		}

		public void SetUp()
		{
			for (int i = 0; i < _staticPickups.Length; i++)
				_staticPickups[i].OnCollected += Collect;

			Level.Route.RegisterRespawn(Respawn);
			Game.Updater.AddFixed(OnFixedUpdate);
			Game.Updater.AddUpdate(OnUpdate);
		}

		private void Respawn()
		{
			for (int i = 0; i < _staticPickups.Length; i++)
				_staticPickups[i].Respawn();

			while (SpawnedPickups.Count > 0)
				SpawnedPickups[0].Respawn();
		}

		public void CleanUp()
		{
			Level.Route.DeregisterRespawn(Respawn);
			Game.Updater.RemoveFixed(OnFixedUpdate);
			Game.Updater.RemoveUpdate(OnUpdate);

			for (int i = 0; i < _staticPickups.Length; i++)
			{
				_staticPickups[i].OnCollected -= Collect;
				_staticPickups[i].CleanUp();
			}
		}



		private void OnFixedUpdate()
		{
			for (int i = 0; i < _staticPickups.Length; i++)
				_staticPickups[i].OnFixedUpdate();

			for(int i = 0; i < SpawnedPickups.Count; i++)
				SpawnedPickups[i].OnFixedUpdate();
		}

		private void OnUpdate()
		{
			for (int i = 0; i < _staticPickups.Length; i++)
				_staticPickups[i].OnUpdate();

			for (int i = 0; i < SpawnedPickups.Count; i++)
				SpawnedPickups[i].OnUpdate();
		}
		#endregion



		#region Collecting
		public void Collect(Pickup pickup)
        {
			switch (pickup.Kind)
			{
				case PickupKind.OreBronze: Treasure.SetOreValue(pickup); break;
				case PickupKind.OreSilver: Treasure.SetOreValue(pickup); break;
				case PickupKind.OreGold: Treasure.SetOreValue(pickup); break;

				case PickupKind.Health: Level.Hero.Heal(pickup.Value); break;
				case PickupKind.Mana: Level.Hero.Recharge(pickup.Value); break;
				case PickupKind.Skill: UnlockSpecial(pickup.Value); break;
			}
		}

		private void UnlockSpecial(float pickupValue)
		{
			SpecialKind kind = (SpecialKind)pickupValue;			
			Game.Catalog.Special.Unlock(kind);
			Level.Hero.Arsenal.SetSelectedSpecial(kind);
			InputProvider.OnSelectSpecial?.Invoke(kind);
			Level.Hero.Mana.TopOff();
		}

		private Treasure Treasure => Game.Catalog.Treasure;
		#endregion



		#region Ore
        public void SpawnOre(OreSpawnData data)
        {
            Ore ore = Level.Pooler.Take(data.prefab);
			if (false == ore.IsReady)
				ore.Wake();
			ore.Transform.position = data.position + Random.insideUnitCircle * data.radius;
            ore.Transform.rotation = Quaternion.identity;
			ore.OnCollected += Collect;
            ore.OnDespawned += OnOreDespawned;

            data.force = new Vector2(_oreHorizontalForceRange.Random, _oreVerticalForceRange.Random);
			data.torqueForce = _oreTorqueForce;
            ore.Spawn(data);

			SpawnedPickups.Add(ore);
        }

        private void OnOreDespawned(Ore ore)
        {
			ore.OnCollected -= Collect;
			ore.OnDespawned -= OnOreDespawned;
			SpawnedPickups.Remove(ore);
			Level.Pooler.Return(ore);
        }

		private List<Ore> SpawnedPickups { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			this.TryAssign(ref _staticPickups);
			this.TryAssign(ref _chests);
		}
		#endif
	}
}