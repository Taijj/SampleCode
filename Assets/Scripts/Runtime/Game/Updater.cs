using System;
using System.Collections.Generic;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Centarlized manager for different update calls that wrap around Unity's own
	/// event functions.
	/// </summary>
	public class Updater : MonoBehaviour
	{
		#region LifeCycle
		[Space, Header("Updates")]
		[SerializeField] private int _updatesLength = 15;
		[SerializeField] private int _fixedUpdatesLength = 15;
		[SerializeField] private int _lateUpdatesLength = 15;
		[Space, Header("Tools")]
		[SerializeField] private int _frameObjectsLength = 5;
		[SerializeField] private int _delayedCallsLength = 5;
		[Tooltip("Allows for nicer movemend speed values in the inspector.")]
		[SerializeField] private float _fixedUpdateAmplifier = 100f;

		public void Wake()
		{
			Updates = new Action[_updatesLength];
			FixedUpdates = new Action[_fixedUpdatesLength];
			LateUpdates = new Action[_lateUpdatesLength];

			FrameObjects = new GameObject[_frameObjectsLength];
			DelayedCalls = new DelayedCall[_delayedCallsLength];

			AmplifiedFixedDeltaTime = Time.fixedDeltaTime * _fixedUpdateAmplifier;
		}
		#endregion



		#region De/Register Updates
		public void AddUpdate(Action update) => Updates.Add(update);
		public void AddFixed(Action update) => FixedUpdates.Add(update);
		public void AddLate(Action update) => LateUpdates.Add(update);

		public void RemoveUpdate(Action update) => Updates.Remove(update);
		public void RemoveFixed(Action update) => FixedUpdates.Remove(update);
		public void RemoveLate(Action update) => LateUpdates.Remove(update);

		private Action[] Updates { get; set; }
		private Action[] FixedUpdates { get; set; }
		private Action[] LateUpdates { get; set; }
		#endregion



		#region Unity
		public void Update()
		{
			UpdateDelayedCalls();
			for (int i = 0; i < Updates.Length; i++)
				Updates[i]?.Invoke();
		}

		public void FixedUpdate()
		{
			for (int i = 0; i < FixedUpdates.Length; i++)
				FixedUpdates[i]?.Invoke();
		}

		public void LateUpdate()
		{
			for (int i = 0; i < LateUpdates.Length; i++)
				LateUpdates[i]?.Invoke();

			ClearFrameObjects();

			#if UNITY_EDITOR
				UpdateEditorInfo();
			#endif
		}

		public float AmplifiedFixedDeltaTime { get; private set; }
		#endregion



		#region Frame Objects
		// Utility methods for recording an object for this frame.
		// The array will then be automatically cleared in LateUpdate()
		public void RecordForFrame(GameObject obj) => FrameObjects.Add(obj);

		public bool TryRecordForFrame(GameObject obj)
		{
			if (FrameObjects.Contains(obj))
				return false;

			FrameObjects.Add(obj);
			return true;
		}

		private void ClearFrameObjects()
		{
			for (int i = 0; i < FrameObjects.Length; i++)
				FrameObjects[i] = null;
		}

		private GameObject[] FrameObjects { get; set; }
		#endregion



		#region Delayed Calls
		public void Delay(DelayedCall call) => DelayedCalls.Add(call);
		public void Cancel(DelayedCall call) => DelayedCalls.Remove(call);

		private void UpdateDelayedCalls()
		{
			for (int i = 0; i < DelayedCalls.Length; i++)
			{
				DelayedCall call = DelayedCalls[i];
				if(call != null)
					call.OnUpdate(Time.deltaTime);
			}
		}

		private DelayedCall[] DelayedCalls { get; set; }
		#endregion



		#if UNITY_EDITOR
		const string DEBUG_FORMAT = "{0}.{1}";

		[Header("Debug")]
		[SerializeField, ReadOnly] private List<string> _updatesInfo;
		[SerializeField, ReadOnly] private List<string> _fixedUpdatesInfo;
		[SerializeField, ReadOnly] private List<string> _lateUpdatesInfo;
		[Space]
		[SerializeField, ReadOnly] private int _delayedCallsInfo;

		private void UpdateEditorInfo()
		{
			if (Updates == null)
				return;

			UpdateInfo(Updates, _updatesInfo);
			UpdateInfo(FixedUpdates, _fixedUpdatesInfo);
			UpdateInfo(LateUpdates, _lateUpdatesInfo);

			_delayedCallsInfo = 0;
			for(int i = 0; i < DelayedCalls.Length; i++)
			{
				if (DelayedCalls[i] != null)
					_delayedCallsInfo++;
			}
		}

		private void UpdateInfo(Action[] array,  List<string> info)
		{
			info.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
					continue;

				Action action = array[i];
				string output = string.Format(DEBUG_FORMAT,
					action.Target.GetType().Name,
					action.Method.Name);
				info.Add(output);
			}
		}
		#endif
	}
}