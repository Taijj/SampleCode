using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Taijj.SampleCode
{
	/// <summary>
	/// Sets up the parallax layers and updates them.
	/// </summary>
	/// 
#if UNITY_EDITOR
	[ExecuteInEditMode]
#endif
	public class ParallaxArea : MonoBehaviour
	{
		#region LifeCycle
		[SerializeField] private ParallaxLayer[] _layers;
		[SerializeField] private FloatRange _range;

		public void Wake()
		{
			for (int i = 0; i < _layers.Length; i++)
				_layers[i].Wake();
		}

		public void SetUp()
		{
			CamTransform = Level.Cameraman.Camera.transform;
			LastCamPosition = new Vector3(0f, 0f, CamTransform.position.z);
		}

		public void OnLateUpdate()
		{
			Vector2 delta = CamTransform.position - LastCamPosition;

			for (int i = 0; i < _layers.Length; i++)
				_layers[i].UpdateParallax(delta);

			LastCamPosition = CamTransform.position;
		}

		private Transform CamTransform { get; set; }
		private Vector3 LastCamPosition { get; set; }
		#endregion



		#if UNITY_EDITOR
		[Space, Header("Editor")]
		[SerializeField, ReadOnly] private bool _parallaxInEditor;

		[DrawIf(nameof(_parallaxInEditor), false)]
		[Button(nameof(EnableEditorParallax))]
		public bool enableButton;
		public void EnableEditorParallax()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			Validate();

			Camera levelCam = Camera.main;
			
			_parallaxInEditor = true;
			CamTransform = SceneView.lastActiveSceneView.camera.transform;
			LastCamPosition = Vector2.zero;

			float height = levelCam.orthographicSize * 2f;
			MainCamSize = new Vector2(height * levelCam.aspect, height);

			EditorApplication.update += ParallaxInEditor;
			EditorApplication.playModeStateChanged += OnPlayModeChange;
		}

		[DrawIf(nameof(_parallaxInEditor), true)]
		[Button(nameof(DisableEditorParallax))]
		public bool disableButton;
		public void DisableEditorParallax()
		{
			Validate();

			_parallaxInEditor = false;
			EditorApplication.update -= ParallaxInEditor;
			EditorApplication.playModeStateChanged -= OnPlayModeChange;
		}

		public void OnDestroy() => DisableEditorParallax();



		public void OnValidate()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				DisableEditorParallax();
				return;
			}

			if (false == _parallaxInEditor)
				Validate();
		}

		private void Validate()
		{			
			transform.position = new Vector3(0f, 0f, transform.position.z);

			this.TryAssign(ref _layers);
			for (int i = 0; i < _layers.Length; i++)
				_layers[i].Validate(_range);
		}
		
		private void ParallaxInEditor()
		{
			if (_parallaxInEditor)
				OnLateUpdate();
		}

		private void OnPlayModeChange(PlayModeStateChange change)
		{
			if (false == _parallaxInEditor)
				return;
						
			if (change.IsEither(PlayModeStateChange.ExitingEditMode, PlayModeStateChange.EnteredEditMode))
			{
				DisableEditorParallax();
				EditorUtility.SetDirty(gameObject);
			}			
		}

		public void OnDrawGizmos()
		{
			if (false == _parallaxInEditor || CamTransform.IsNull(true))
				return;

			Vector2 center = Vector2.one * CamTransform.position;

			Gizmos.color = ColorAddons.Lime.With(0.5f);			
			Gizmos.DrawCube(center, new Vector2(MainCamSize.x, 0.01f));
			Gizmos.DrawCube(center, new Vector2(0.01f, MainCamSize.y));
			Gizmos.DrawWireCube(center, MainCamSize);
		}

		private Vector2 MainCamSize { get; set; }
		#endif
	}
}
