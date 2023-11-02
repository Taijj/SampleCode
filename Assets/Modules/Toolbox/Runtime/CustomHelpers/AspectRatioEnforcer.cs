#if false == (UNITY_SWITCH || UNITY_PS5 || UNITY_PS4 || UNITY_GAMECORE_XBOXSERIES)

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Makes sure the game only runs in the given aspect ratio, no matter the Screen,
/// by configuring the Cameras and Canvasses of the Scene.
///
/// All Canvases are assumed to be setup correctly in the editor. To quickly setup
/// a Canvas object, right click a Canvas and select "Ble/Configure Canvas for 4K".
/// </summary>
public class AspectRatioEnforcer : MonoBehaviour
{
    #region LifeCycle
    [SerializeField] private Vector2Int _aspectRatio;
    [SerializeField] private SortLayer _canvassesSortingLayer;
    [Space, Header("List Optimizations")]
    [SerializeField] private int _cameraCount;
    [SerializeField] private int _canvasCount;

    public void Wake() => InitializeAspectRatio();

    public void SetUp()
    {
        OnEnterScene();
        OnScreenSizeChanged(Screen.width, Screen.height);
        IsReady = true;
    }

    private bool IsReady { get; set; }
    #endregion



    #region Aspect Ratio
    private void InitializeAspectRatio()
    {
        LastScreenWidth = Screen.width;
        LastScreenHeight = Screen.height;
        TargetAspect = _aspectRatio.x / (float)_aspectRatio.y;
    }

    public void Update()
    {
        if (false == IsReady)
            return;

        // This ensures the black bars, not handled by any camera, are always black.
        GL.Clear(false, true, Color.black);

        int width = Screen.width;
        int height = Screen.height;
        if (width != LastScreenWidth || height != LastScreenHeight)
            OnScreenSizeChanged(width, height);

        LastScreenWidth = width;
        LastScreenHeight = height;
    }

    private void OnScreenSizeChanged(float screenWidht, float screenHeight)
    {
        float currentAspect = screenWidht / screenHeight;
        float heightFactor = currentAspect / TargetAspect;
        for(int i = 0; i < Cameras.Count; i++)
        {
            Camera cam = Cameras[i];
            if (heightFactor < 1f)
                SetLetterbox(cam, heightFactor);
            else
                SetPillarbox(cam, heightFactor);
        }
    }

    private void SetLetterbox(Camera cam, float heightFactor)
    {
        Rect rect = cam.rect;
        rect.width = 1f;
        rect.height = heightFactor;
        rect.x = 0;
        rect.y = (1f - heightFactor) / 2f;
        cam.rect = rect;
    }

    private void SetPillarbox(Camera cam, float heightFactor)
    {
        float widthFactor = 1.0f / heightFactor;

        Rect rect = cam.rect;
        rect.width = widthFactor;
        rect.height = 1f;
        rect.x = (1f - widthFactor) / 2f;
        rect.y = 0;
        cam.rect = rect;
    }

    private float TargetAspect { get; set; }
    private int LastScreenWidth { get; set; }
    private int LastScreenHeight { get; set; }
    #endregion



    #region Cameras & Canvasses
    private void OnEnterScene()
    {
        FetchComponents();
        AssignCameraToCanvases();
    }

    private void FetchComponents()
    {
        Scene sc = gameObject.scene;
        Cameras = sc.GetComponents<Camera>(estimatedCount: _cameraCount);
        Canvasses = sc.GetComponents<Canvas>(estimatedCount: _canvasCount);
        SetUiCamera();
    }

    private void SetUiCamera()
    {
        if (Cameras.Count == 0)
            throw new System.Exception("Cannot set UiCamera from empty List!");

        UiCamera = Cameras[0];
        for (int i = 0; i < Cameras.Count; i++)
        {
            Camera cam = Cameras[i];
            if (cam.gameObject.layer == LayerAddons.UI_LAYER)
            {
                UiCamera = cam;
                return;
            }
        }
    }

    private void AssignCameraToCanvases()
    {
        for(int i = 0; i < Canvasses.Count; i++)
        {
            Canvas can = Canvasses[i];
            if (false == can.isRootCanvas)
                continue;

            can.worldCamera = UiCamera;
            can.sortingLayerID = _canvassesSortingLayer.Id;
        }
    }

    /// <summary>
    /// Assign the current scene's UI Camera to the given Canvas. Use this
    /// to make sure the Canvas has the right size, if it was created after
    /// a Scene was entered.
    /// </summary>
    public void AssignCameraTo(Canvas canvas)
    {
        if (false == IsReady)
            throw new System.Exception("Enforcer is not ready, yet! Call Wake() and SetUp() first!");

        Canvasses.Add(canvas);
        canvas.worldCamera = UiCamera;
        canvas.sortingLayerID = _canvassesSortingLayer.Id;
    }


    private List<Camera> Cameras { get; set; }
    private List<Canvas> Canvasses { get; set; }
    private Camera UiCamera { get; set; }
    #endregion
}

#endif