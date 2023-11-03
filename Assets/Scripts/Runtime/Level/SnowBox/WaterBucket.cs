
using System;
using System.Collections.Generic;
using UnityEngine;
using State = Taijj.SampleCode.SnowBox.State;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Helper for filling a <see cref="SnowBox"/> with water. Has a certain amount
	/// of "fill" an will empty that fill algorithmicly into a defined area of <see cref="State.Air"/>
	/// tiles.
	/// </summary>
	[System.Serializable]
    public class WaterBucket
    {
		#region LifeCycle;
		[SerializeField] private float _maxFill;
		[SerializeField] private int _maxFillPositions;

		public struct Data
		{
			public Dictionary<Vector3Int, SnowBoxTile> tilesByPosition;
			public Action<Vector3Int, State> changeStateAt;
		}

    	public void Wake(Data data)
    	{
			TilesByPosition = data.tilesByPosition;
			ChangeStateAt = data.changeStateAt;

			FillPositions = new List<Vector3Int>(_maxFillPositions);
    	}

		private Dictionary<Vector3Int, SnowBoxTile> TilesByPosition { get; set; }
		private Action<Vector3Int, State> ChangeStateAt { get; set; }
		#endregion



		#region Fill & Tip Over
		public void Fill(SnowBoxTile source, float amount)
		{
			CurrentFill += amount;
			if (CurrentFill >= _maxFill)
			{
				CurrentFill = 0;
				TipOver(source.GridPosition);
			}
		}

		private void TipOver(Vector3Int sourcePosition)
		{
			FillPositions.Clear();
			RaiseWaterRecursive(sourcePosition);

			for (int i = 0; i < FillPositions.Count; i++)
				ChangeStateAt(FillPositions[i], State.Water);
		}

		public void Respawn() => CurrentFill = 0;

		private float CurrentFill { get; set; }
		private List<Vector3Int> FillPositions { get; set; }
		#endregion



		#region Water Raising
		private void RaiseWaterRecursive(Vector3Int from)
		{
			Vector3Int middle = FindFurthest(from, Vector3Int.down);
			Vector3Int left = FindRunningDown(middle, -1);
			Vector3Int right = FindRunningDown(middle, 1);

			bool hasLeft = left.y < middle.y;
			bool hasRight = right.y < middle.y;

			if (hasLeft)
				RaiseWaterRecursive(left);
			if (hasRight)
				RaiseWaterRecursive(right);

			if (false == hasLeft && false == hasRight)
				RecordRow(middle);
		}

		private void RecordRow(Vector3Int start)
		{
			Vector3Int next = FindFurthest(start, Vector3Int.left);
			while (true)
			{
				if (false == FillPositions.Contains(next))
					FillPositions.Add(next);

				next += Vector3Int.right;
				if (IsNotAir(next))
					return;
			}
		}

		/// <summary>
		/// Finds the lowest <see cref="State.Air"/> tile in the given x direction.
		/// It's used to identify "basins" that should fill with water first.
		/// </summary>
		private Vector3Int FindRunningDown(Vector3Int start, int direction)
		{
			Vector3Int result = start;
			while (true)
			{
				Vector3Int next = result + Vector3Int.right * direction;
				if (IsNotAir(next))
					return result;

				result = FindFurthest(next, Vector3Int.down);
			}
		}

		/// <summary>
		/// Looks for the last <see cref="State.Air"/> tile in the given direction.
		/// </summary>
		private Vector3Int FindFurthest(Vector3Int start, Vector3Int direction)
		{
			Vector3Int current = start;
			while (true)
			{
				Vector3Int next = current + direction;
				if (IsNotAir(next))
					return current;

				current = next;
			}
		}



		private bool IsNotAir(Vector3Int pos)
		{
			if (false == TilesByPosition.ContainsKey(pos))
				return true;

			return TilesByPosition[pos].State != State.Air;
		}
		#endregion
	}
}