using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Central manager for SnowBox features. Bundles together a <see cref="Tilemap"/>
	/// and respective colliders and triggers. Manages Tile life cycle and state.
	/// </summary>
	public class SnowBox : MonoBehaviour
	{
		#region LifeCycle
		[Header("Parts")]
		[SerializeField] private SnowBoxTile _prefab;
		[SerializeField] private Tilemap _map;
		[SerializeField] private Transform _tilesContainer;
		[SerializeField] private WaterBucket _bucket;
		[SerializeField] private SpriteRenderer _waterWorkaround;
		[Space, Header("Configs")]
		[SerializeField] private TileConfig _noneConfig;
		[SerializeField] private TileConfig _airConfig;
		[SerializeField] private TileConfig _slushConfig;
		[SerializeField] private TileConfig _waterConfig;
		[SerializeField] private TileConfig _snowConfig;
		[SerializeField] private TileConfig _iceConfig;

		public void Wake()
		{
			_bucket.Wake(new WaterBucket.Data
			{
				tilesByPosition = _tilesByGridPosition,
				changeStateAt = ChangeTileAt
			});
									
			foreach (Vector3Int gridPos in _map.cellBounds.allPositionsWithin)
			{
				if (!_map.TryGetTile(gridPos, out TileConfig config))
					continue;

				Vector3 worldPos = _map.CellToWorld(gridPos) + _map.tileAnchor;
				SnowBoxTile tile = Instantiate(_prefab, worldPos, Quaternion.identity, _tilesContainer);
				tile.name = $"Original-{config.state})";
				tile.Wake(new SnowBoxTile.Data
				{
					worldPosition = worldPos,
					gridPosition = gridPos,
					originalConfig = config,

					box = this
				});

				if (config.state == State.Water)
					_map.SetTile(gridPos, _noneConfig);

				_tilesByGridPosition.Add(gridPos, tile);
			}			

			ChangeDatas = new TileChangeData[_tilesByGridPosition.Count];
			ClearChanges();

			UpdateWaterWorkaround();			
		}

		public void Respawn()
		{
			_bucket.Respawn();

			ClearChanges();
			foreach (SnowBoxTile tile in _tilesByGridPosition.Values)
				SetState(tile, tile.OriginalConfig.state);
		}

		public WaterBucket Bucket => _bucket;
		#endregion



		#region Tiles
		public enum State
		{
			None = 0,

			Air,
			Water,
			Slush,
			Snow,
			Ice
		}

		public const float WATER_ALPHA = 0.6f;
		private static readonly Vector3Int NONE_POSITION = new Vector3Int(3000, 3000, 0);



		public void ChangeTileAt(Vector3Int gridPosition, State state)
		{
			SnowBoxTile tile = _tilesByGridPosition[gridPosition];
			if (tile.IsOccupied)
				return;

			SetState(tile, state);
			tile.PlayStateSound();
		}

		private void SetState(SnowBoxTile tile, State state)
		{
			TileConfig config = _noneConfig;
			switch (state)
			{
				case State.Air: config = _airConfig; break;
				case State.Slush: config = _slushConfig; break;
				case State.Snow: config = _snowConfig; break;
				case State.Ice: config = _iceConfig; break;
				case State.Water: config = _waterConfig; break;
			}

			tile.Set(config);
			WorkaroundIsDirty = true;
			ChangeMapTile(tile, state != State.Water ? config : _noneConfig);
		}

		private void ChangeMapTile(SnowBoxTile tile, TileConfig config)
		{
			Color color = config.state == State.Water ? Color.white.With(WATER_ALPHA) : Color.white;
			for (int i = 0; i < ChangeDatas.Length; i++)
			{
				TileChangeData data = ChangeDatas[i];
				if (data.position != tile.GridPosition)
					continue;

				data.color = color;
				data.tile = config;
				ChangeDatas[i] = data;
				return;
			}
			ChangeDatas[ChangeCount] = new TileChangeData(tile.GridPosition, config, color, Matrix4x4.identity);
			ChangeCount++;
		}



		public void ApplyChanges()
		{
			if (WorkaroundIsDirty)
				UpdateWaterWorkaround();

			if (ChangeCount == 0)
				return;

			for (int i = ChangeCount; i < ChangeDatas.Length; i++)
				ChangeDatas[i] = GetNoneData();
			_map.SetTiles(ChangeDatas, true);

			foreach (SnowBoxTile tile in _tilesByGridPosition.Values)
				tile.OnLateUpate();
		}

		private void ClearChanges()
		{
			ChangeCount = 0;
			for (int i = 0; i < ChangeDatas.Length; i++)
				ChangeDatas[i] = GetNoneData();
		}

		private TileChangeData GetNoneData() => new TileChangeData(NONE_POSITION, null, Color.white, Matrix4x4.identity);



		private Dictionary<Vector3Int, SnowBoxTile> _tilesByGridPosition = new Dictionary<Vector3Int, SnowBoxTile>();
		private TileChangeData[] ChangeDatas { get; set; }
		private int ChangeCount { get; set; }
		#endregion



		//This section implements a rather quick workaround to replace WaterTiles with the WaterWorkaround 9-sliced tileable Sprite.
		//This prevents weird holes to appear in the TileMap
		private void UpdateWaterWorkaround()
		{
			bool hasWater = DoWaterWorkaround(out Bounds bounds);
			_waterWorkaround.gameObject.SetActive(hasWater);
			if (false == hasWater)
				return;

			_waterWorkaround.transform.position = bounds.center;
			_waterWorkaround.size = bounds.size;
			WorkaroundIsDirty = false;
		}

		private bool DoWaterWorkaround(out Bounds bounds)
		{
			bool hasWater = false;
			bounds = new Bounds();

			Vector2 topLeft = new Vector2(float.MaxValue, float.MinValue);
			Vector2 botRight = new Vector2(float.MinValue, float.MaxValue);
			foreach (SnowBoxTile tile in _tilesByGridPosition.Values)
			{
				if (tile.State != State.Water)
					continue;

				Vector2 pos = tile.WorldPosition;
				if (pos.x < topLeft.x) topLeft.x = pos.x;
				if (pos.x > botRight.x) botRight.x = pos.x;
				if (pos.y > topLeft.y) topLeft.y = pos.y;
				if (pos.y < botRight.y) botRight.y = pos.y;
				
				hasWater = true;
			}

			if (false == hasWater)
				return false;

			topLeft += new Vector2(-1, 1)/2f;
			botRight += new Vector2(1, -1)/2f;

			Vector2 center = topLeft + (botRight-topLeft)/2f;
			float widht = botRight.x - topLeft.x;
			float height = topLeft.y - botRight.y;
			Vector2 size = new Vector2(widht, height);
			
			bounds.center = center;
			bounds.size = size;
			return true;			
		}
		
		private bool WorkaroundIsDirty { get; set; }
		


		#if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			if (UnityEditor.EditorApplication.isPlaying)
				return;

			SnowBoxTileMap.DrawGizmos(_map);
		}

		public void OnValidate()
		{
			if (_map.IsNull()) _map = GetComponentInChildren<Tilemap>();
			SnowBoxTileMap.UpdateWaterTransparency(_map);
		}
		#endif
	}
}
