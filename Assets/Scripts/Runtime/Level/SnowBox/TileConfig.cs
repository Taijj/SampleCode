using FMODUnity;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Scriptable object containing configuration for SnowBox Tile
	/// behavior.
	/// </summary>
	[CreateAssetMenu]
	public class TileConfig : RuleTile<TileConfig.Neighbor>
	{
		#region Configuration
		[Space, Header("Configuration")]
		public SnowBox.State state;
		public FloatRange heatRange;
		public float defaultHeat;
		[Range(0f, 1f)] public float slipperyness;

		public EventReference sound;
		#endregion



		#region Unity Generated
		public class Neighbor : TilingRuleOutput.Neighbor
		{
			public const int NULL = 3;
			public const int NOT_NULL = 4;
		}

		public override bool RuleMatch(int neighbor, TileBase other)
		{
			if (other is RuleOverrideTile ot)
				other = ot.m_InstanceTile;

			switch (neighbor)
			{
				case TilingRuleOutput.Neighbor.This: return CheckForSlushOnThis(other);
				case TilingRuleOutput.Neighbor.NotThis: return CheckForSlushOnNotThis(other);
				case Neighbor.NULL: return other == null;
				case Neighbor.NOT_NULL: return other != null;
			}
			return true;
		}

		private bool CheckForSlushOnThis(TileBase other)
		{
			if(other is TileConfig)
			{
				TileConfig con = other as TileConfig;
				if (IsSlushAdjacentToSnow(con))
					return true;
			}

			return other == this;
		}

		private bool CheckForSlushOnNotThis(TileBase other)
		{
			if (other is TileConfig)
			{
				TileConfig con = other as TileConfig;
				if (IsSlushAdjacentToSnow(con))
					return false;
			}

			return other != this;
		}

		private bool IsSlushAdjacentToSnow(TileConfig other)
		{
			return state == SnowBox.State.Snow && other.state == SnowBox.State.Slush
				|| state == SnowBox.State.Slush && other.state == SnowBox.State.Snow;
		}
		#endregion
	}
}