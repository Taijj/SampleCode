using UnityEngine;
using State = Taijj.HeartWarming.SnowBox.State;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Holds the aggregate state of itself and manages transitions to other states,
	/// i.e. reacts to Heat changes.
	/// </summary>
	public class SnowBoxTile : MonoBehaviour, IHeatable
	{
		#region LifeCycle
		public struct Data
		{
			public Vector2 worldPosition;
			public Vector3Int gridPosition;
			public TileConfig originalConfig;

			public SnowBox box;
		}

		[SerializeField] private Collider2D _defaultTrigger;
		[SerializeField] private Collider2D _waterTrigger;
		[Space]
		[SerializeField, ReadOnly] private Vector2 _worldPosition;
		[SerializeField, ReadOnly] private Vector3Int _gridPosition;
		[Space]
		[SerializeField, ReadOnly] private float _heat;
		[SerializeField, ReadOnly] private TileConfig _config;

		public void Wake(Data data)
		{
			_worldPosition = data.worldPosition;
			_gridPosition = data.gridPosition;
			Transform = transform;
			Box = data.box;

			OriginalConfig = data.originalConfig;
			CanBeWater = OriginalConfig.state.IsEither(State.Air, State.Water, State.Slush);
			Set(OriginalConfig);
		}

		public TileConfig OriginalConfig { get; private set; }
		public Vector3Int GridPosition => _gridPosition;
		public Vector2 WorldPosition => _worldPosition;
		public SnowBox Box { get; private set; }
		public Transform Transform { get; set; }
		#endregion



		#region Misc		
		public void PlayStateSound() => Game.Audio.Play(_config.sound, Transform);
		public void AddWater(float amount) => Box.Bucket.Fill(this, amount);
		public float Slipperyness => _config.slipperyness;

		public void OnLateUpate()
		{
			if (false == _waterTrigger.enabled)
				return;

			LayerMask mask = Layers.HERO.ToMask() | Layers.SUPPORT.ToMask() | Layers.ENEMY.ToMask();
			ContactFilter2D filter = new ContactFilter2D
			{
				useTriggers = false,
				useLayerMask = true,
				layerMask = mask
			};

			IsOccupied = _waterTrigger.OverlapCollider(filter, _overlaps) > 0;
		}

		private Collider2D[] _overlaps = new Collider2D[5];
		public bool IsOccupied { get; private set; }
		#endregion



		#region Config (State)
		public void Set(TileConfig config)
		{
			_config = config;
			IsOccupied = false;

			if(State != State.None)
			{
				_heat = DefaultHeat;
				_defaultTrigger.enabled = true;
				_waterTrigger.enabled = State == State.Water;
			}
			else
			{
				_heat = -1;
				_defaultTrigger.enabled = false;
				_waterTrigger.enabled = false;
			}
		}

		public void Heat(float amount)
		{
			_heat = HeatRange.Clamp(_heat + amount);
			if (_heat == HeatRange.Max)
				Melt();
			else if (_heat == HeatRange.Min)
				Freeze();
		}

		private void Melt()
		{
			switch (State)
			{
				case State.Slush: ChangeState(State.Water); break;
				case State.Snow: ChangeState(CanBeWater ? State.Water : State.None); break;
				case State.Ice: ChangeState(CanBeWater ? State.Water : State.Snow); break;
			}
		}

		private void Freeze()
		{
			if (State.IsEither(State.Water, State.Snow, State.Slush))
				ChangeState(State.Ice);
		}

		public void ChangeState(State state) => Box.ChangeTileAt(_gridPosition, state);



		private FloatRange HeatRange => _config.heatRange;
		private float DefaultHeat => _config.defaultHeat;
		public State State => _config.state;
		public bool CanBeWater { get; private set; }
		#endregion



		#if UNITY_EDITOR
		[Button(nameof(CoolDownTest))]
		public bool coolDownTest;

		[Button(nameof(HeatUpTest))]
		public bool heatUpTest;

		public void CoolDownTest()
		{
			if (Sanitize())
				Heat(-25);
		}

		public void HeatUpTest()
		{
			if (Sanitize())
				Heat(25);
		}

		private bool Sanitize()
		{
			if (false == UnityEditor.EditorApplication.isPlaying)
			{
				Note.Log("This only works in PlayMode!", ColorAddons.Orange);
				return false;
			}

			if (gameObject.scene.buildIndex == -1)
			{
				Note.Log("It seems you have the prefab selected! Select an actual instance in the scene instead!", ColorAddons.Orange);
				return false;
			}

			if(_config.IsNull())
			{
				Note.Log("Cannot manipulate None Tile! Config is null!", ColorAddons.Orange);
				return false;
			}

			return true;
		}



		public void OnDrawGizmos()
		{
			if (IsOccupied)
				Gizmos.DrawSphere(transform.position, 0.25f);
		}
		#endif
	}
}
