using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Taijj.SampleCode
{
	/// <summary>
	/// Contains an unordered list of methods and utilities that are often unsed and might
	/// be transferred into the Ble Toolbox at some point.
	/// </summary>
    public static class Addons
    {
		public static bool TryGetTile(this Tilemap @this, Vector3Int gridPos, out TileConfig snowBoxTile)
		{
			snowBoxTile = @this.GetTile<TileConfig>(gridPos);
			return !snowBoxTile.IsNull();
		}

		/// <summary>
		/// Checks if this collider's GameObject has a component of the given
		/// Type attached. If none is found the Collider's attachedRigidbody
		/// is checked, if one exists.
		/// </summary>
		/// <typeparam name="T">Type of the component to look for.</typeparam>
		/// <param name="component">The component, if one is found, null otherwise.</param>
		/// <returns>True, if a component of the given Type could be found on the collider or it's rigid body, false otherwise.</returns>
		public static bool TryGet<T>(this Collider2D @this, out T component)
		{
			if (@this.TryGetComponent(out component))
				return true;

			Rigidbody2D rigid = @this.attachedRigidbody;
			if (rigid.IsNull())
				return false;
			return rigid.TryGetComponent(out component);
		}

		public static Vector2 GetDirectionFrom(List<Contact> contacts)
		{
			if (contacts.Count == 1)
				return GetDirectionFromFirst(contacts);

			Vector2 leftMost = contacts[0].point;
			Vector2 rightMost = contacts[contacts.Count-1].point;
			for (int i = 0; i < contacts.Count; i++)
			{
				Contact con = contacts[i];
				if (con.point.x < leftMost.x)
					leftMost = con.point;
				else if (con.point.x > rightMost.x)
					rightMost = con.point;
			}

			Vector2 leftToRight = rightMost - leftMost;
			if (leftToRight.sqrMagnitude > MIN_CONTACT_POSITION_DELTA)
				return leftToRight.normalized;
			else
				return GetDirectionFromFirst(contacts);
		}

		public static Vector2 GetDirectionFromFirst(List<Contact> contacts)
		{
			return Vector2.Perpendicular(contacts[0].normal) * -1f;
		}

		private const float MIN_CONTACT_POSITION_DELTA = 0.001f;


		/// <summary>
		/// Check to see if a flags enumeration has a specific flag set.
		/// Based on: https://stackoverflow.com/questions/4108828/generic-extension-method-to-see-if-an-enum-contains-a-flag
		/// </summary>
		public static bool IsAny(this Enum @this, Enum value)
		{
			// Not as good as the .NET 4 version of this function, but should be good enough
			if (@this.GetType() != value.GetType())
			{
				throw new ArgumentException(
					$"Enumeration type mismatch. The flag is of type '{value.GetType()}', was expecting '{@this.GetType()}'.");
			}

			int val = Convert.ToInt32(value);
			int thi = Convert.ToInt32(@this);
			return (thi & val) != 0;
		}




		public static void TryAssign<T>(this MonoBehaviour @this, ref T injection) where T : UnityEngine.Component
		{
			if (injection.IsNull(true)) injection = @this.GetComponentInChildren<T>();
		}

		public static void TryAssign<T>(this MonoBehaviour @this, ref T[] injection) where T : UnityEngine.Component
		{
			T[] children = @this.GetComponentsInChildren<T>();
			if (injection.IsFaultyFixed() || children.Length > injection.Length) injection = children;
		}

		public static void TryFind<T>(this MonoBehaviour @this, ref T injection) where T: UnityEngine.Component
		{
			if (injection.IsNull(true)) injection = UnityEngine.Object.FindObjectOfType<T>(true);
		}

		public static bool IsFaultyFixed<T>(this T[] @this) where T : UnityEngine.Object
		{
			if (@this == null || @this.Length == 0)
				return true;

			foreach (T element in @this)
			{
				if (element.IsNull(true))
					return true;
			}
			return false;
		}



		public static void Randomize(this Animator @this)
		{
			@this.SetFloat(AnimatorHashes.CYCLE_OFFSET, UnityEngine.Random.value);
		}



		#if UNITY_EDITOR
		/// <summary>
		/// Unused so far, but important later for OnValidate
		/// </summary>
		private static bool IsRemovedFromPrefab(Component component)
		{
			var original = PrefabUtility.GetCorrespondingObjectFromOriginalSource(component);
			if (original.IsNull(true))
				return false;
			return original != PrefabUtility.GetCorrespondingObjectFromSource(component);
		}

		public static bool IsNotInCurrentHierarchy(this MonoBehaviour @this)
		{
			PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
			Scene currentScene;
			if (stage != null && stage.stageHandle.IsValid())
				currentScene = stage.scene;
			else
				currentScene = @this.gameObject.scene;

			return @this.gameObject.scene != currentScene;
		}
		#endif
	}
}
