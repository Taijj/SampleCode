
using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Centralized and static Helper for dealing with Unity layers.
	/// </summary>
    public static class Layers
    {
		public static readonly int WORLD = LayerMask.NameToLayer("World");
		public static readonly int HERO = LayerMask.NameToLayer("Hero");
		public static readonly int ENEMY = LayerMask.NameToLayer("Enemy");
		public static readonly int SNOWBOX = LayerMask.NameToLayer("SnowBox");
		public static readonly int SUPPORT = LayerMask.NameToLayer("Support");
		public static readonly int PICKUP = LayerMask.NameToLayer("Pickup");

		public static int ToMask(this int @this)
		{
			if (@this < 0)
				throw new Exception("Only positive numbers can be converted to a Mask value");

			return 1 << @this;
		}

		public static int ToMask(params int[] layers)
		{
			int result = layers[0].ToMask();
			for(int i = 0; i < layers.Length; i++)
				result |= layers[i].ToMask();
			return result;
		}
	}
}