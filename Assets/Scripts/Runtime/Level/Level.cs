using UnityEngine;
using UnityEngine.U2D;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// SceneRoot and Facade for a playable level Scene.
	/// </summary>
	public class Level : SceneRoot
	{
		#region LifeCycle
		[Space, Header("Characters")]
		[SerializeField] private Hero _hero;
		[SerializeField] private Horde _horde;
		[SerializeField] private World _world;
		[Space, Header("Camera")]
		[SerializeField] private Cameraman _cameraman;		
		[Space, Header("Misc")]
		[SerializeField] private ShotFactory _shotFactory;
		[SerializeField] private Route _route;
		[SerializeField] private Pooler _pooler;
		[SerializeField] private Collector _collector;
		[SerializeField] private SceneAudio _audio;
		[SerializeField] private SpriteShapeController[] _shapes;
		[Space, Header("Ui")]
		[SerializeField] private UiLevel _uiLevel;
		[SerializeField] private UiHud _uiHud;

		public override void Wake()
		{
			Instance = this;

			_audio.Wake();
			_hero.Wake();
			_horde.Wake();
			_world.Wake();

			_cameraman.Wake();

			_shotFactory.Wake();
			_route.Wake();
			_pooler.Wake();
			_collector.Wake();

			_uiLevel.Wake();
			_uiHud.Wake();

			for (int i = 0; i < _shapes.Length; i++)
			{
				_shapes[i].autoUpdateCollider = false;
				_shapes[i].BakeCollider();
			}
		}

		public override void SetUp()
		{
			_hero.SetUp();
			_horde.SetUp();
			_world.SetUp();

			_shotFactory.SetUp();
			_route.SetUp();
			_collector.SetUp();
		}

		public override void CleanUp()
		{
			_hero.CleanUp();
			_horde.CleanUp();
			_world.CleanUp();

			_cameraman.CleanUp();

			_shotFactory.CleanUp();
			_collector.CleanUp();
			_route.CleanUp();

			_uiLevel.CleanUp();
			_uiHud.CleanUp();
			_audio.CleanUp();

			Game.Catalog.Special.ResetAllUnlocks();

			Instance = null;
		}
		#endregion



		#region Statics
		private static Level Instance { get; set; }

		public static Hero Hero => Instance._hero;
		public static Cameraman Cameraman => Instance._cameraman;
		public static ShotFactory ShotFactory => Instance._shotFactory;
		public static Route Route => Instance._route;
		public static Pooler Pooler => Instance._pooler;
		public static Collector Collector => Instance._collector;
		public static Horde Horde => Instance._horde;
		public static SceneAudio Audio => Instance._audio;

		public static UiLevel UiLevel => Instance._uiLevel;
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			this.TryFind(ref _audio);
			this.TryFind(ref _hero);
			this.TryFind(ref _horde);
			this.TryFind(ref _world);
						
			this.TryFind(ref _cameraman);
			this.TryFind(ref _route);
			this.TryFind(ref _collector);

			this.TryFind(ref _uiLevel);
			this.TryFind(ref _uiHud);

			this.TryAssign(ref _shotFactory);
			this.TryAssign(ref _pooler);


			if (string.IsNullOrEmpty(gameObject.scene.path))
				return;

			SpriteShapeController[] allShapes = FindObjectsOfType<SpriteShapeController>();
			if (_shapes.IsFaultyFixed())
			{
				_shapes = allShapes;
				return;
			}

			if (allShapes.Length != _shapes.Length)
				_shapes = allShapes;
		}
		#endif
	}
}