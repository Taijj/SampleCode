#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Editor only helper for the <see cref="SnowBox"/> and a TilePalette including
	/// <see cref="SnowBoxTile"/>s.
	/// </summary>
    public class SnowBoxTileMap : MonoBehaviour
    {
		[SerializeField] private Tilemap _map;
		private const float RADIUS = 0.25f;

		public void OnDrawGizmos() => DrawGizmos(_map);
		public void OnValidate() => UpdateWaterTransparency(_map);



		public static void DrawGizmos(Tilemap map)
		{
			if (map.IsNull(true))
				return;

			foreach (Vector3Int pos in map.cellBounds.allPositionsWithin)
			{
				if (!map.TryGetTile(pos, out TileConfig config))
					continue;

				bool cannotTurnToWater = config.state != SnowBox.State.Slush && config.state != SnowBox.State.Air;
				if (cannotTurnToWater)
					continue;

				Gizmos.color = ColorAddons.Aqua;
				Vector2 world = map.CellToWorld(pos) + map.tileAnchor;
				Gizmos.DrawSphere(world, RADIUS);
			}
		}

		public static void UpdateWaterTransparency(Tilemap map)
		{
			if (map.IsNull(true))
				return;

			foreach (Vector3Int pos in map.cellBounds.allPositionsWithin)
			{
				if (!map.TryGetTile(pos, out TileConfig config))
					continue;

				if(config.state == SnowBox.State.Water)
					map.SetColor(pos, Color.white.With(SnowBox.WATER_ALPHA));
			}
		}
	}
}
#endif