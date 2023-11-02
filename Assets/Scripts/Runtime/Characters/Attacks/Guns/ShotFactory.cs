
using System.Linq;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Helper for life cycle management and configuration
	/// of <see cref="Shot"/>s.
	/// </summary>
	public class ShotFactory : MonoBehaviour
	{
		#region Own LifeCycle
		[SerializeField] private int _maxShotCount = 100;

		public void Wake() => Shots = new Shot[_maxShotCount];

		public void SetUp()
		{
			Level.Route.RegisterRespawn(DestroyAll);
			Game.Updater.AddUpdate(UpdateShots);
		}

		public void CleanUp()
		{
			Level.Route.DeregisterRespawn(DestroyAll);
			Game.Updater.RemoveUpdate(UpdateShots);
			DestroyAll();
		}

		private Shot[] Shots { get; set; }
		#endregion



		#region Shots LifeCycle
		public void Spawn(ShotInfo info)
		{
			info.onDestroyed = OnShotDestroyed;

			Shot shot = Level.Pooler.Take(info.prefab);
			if(false == shot.IsReady)
				shot.Wake();
			shot.Shoot(info);

			Shots.Add(shot);
		}

		private void UpdateShots()
		{
			for(int i = 0; i < Shots.Length; i++)
			{
				if (Shots[i].HasReference())
					Shots[i].OnUpdate();
			}
		}

		private void OnShotDestroyed(Shot shot)
		{
			Shots.Remove(shot);
			Level.Pooler.Return(shot);
		}



		public void DisintegrateAll()
		{
			for (int i = 0; i < Shots.Length; i++)
			{
				Shot s = Shots[i];
				if (s.HasReference())
					s.Disintegrate();
			}
		}

		public void DestroyAll()
		{
			for (int i = 0; i < Shots.Length; i++)
			{
				Shot s = Shots[i];
				if (s.HasReference())
					s.Destroy();
			}
		}
		#endregion
	}
}