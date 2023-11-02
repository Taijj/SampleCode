
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor.Presets;
using System.Linq;
using Object = UnityEngine.Object;

/// <summary>
/// Configures a Canvas object to work properly with 4K, and removes unnecessary components.
///
/// Note: Uses the pixels per unit value of the first TextureImporter Preset that can be found
/// in the project. 100, if none are found.
/// </summary>
public static class CanvasConfigurator
{
    #region Main
    private const string MENU_PATH = "GameObject/Ble/Configure Canvas for 4K";

    [MenuItem(MENU_PATH, isValidateFunction: true)]
    public static bool TryGetCanvasses()
    {
        Canvasses = Selection.objects
            .OfType<GameObject>()
            .Select(o => o.GetComponent<Canvas>())
            .Where(c => c.HasReference(true))
            .ToArray();
        return Canvasses.Length != 0;
    }

    [MenuItem(MENU_PATH)]
    public static void ConfigureCanvasses()
    {
        if (false == TryGetCanvasses())
            return;

        DeterminePixelsPerUnit();
        foreach (Canvas canvas in Canvasses)
            Configure(canvas);
    }



    private static void Configure(Canvas canvas)
    {
        TryConfigureCamera(canvas);
        TryConfigureScaler(canvas);

        if (canvas.TryGetComponent(out GraphicRaycaster raycaster))
            Object.DestroyImmediate(raycaster);

        canvas.SetToRecursive(LayerAddons.UI_LAYER);
        EditorUtility.SetDirty(canvas.gameObject);
    }

    private static void TryConfigureCamera(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.planeDistance = 1;

        Camera[] cameras = Object.FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.gameObject.layer == LayerAddons.UI_LAYER)
            {
                canvas.worldCamera = cam;
                break;
            }
        }

        if (canvas.worldCamera.IsNull() && cameras.Length > 0)
            canvas.worldCamera = cameras[0];
    }

    private static void TryConfigureScaler(Canvas canvas)
    {
        if (canvas.TryGetComponent(out CanvasScaler scaler))
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referencePixelsPerUnit = CanvasPixelsPerUnit;
            scaler.referenceResolution = new Vector2(3840, 2160);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }
    }

    private static Canvas[] Canvasses { get; set; }
    #endregion



    #region Pixels Per Unit
    private static void DeterminePixelsPerUnit()
    {
        Preset preset = AssetDatabase.FindAssets(PRESET_SEARCH)
               .Select(g => AssetDatabase.GUIDToAssetPath(g))
               .Select(p => AssetDatabase.LoadAssetAtPath<Preset>(p))
               .Where(p => p.GetPresetType().GetManagedTypeName() == typeof(TextureImporter).FullName)
               .FirstOrDefault();

        if (preset.IsNull())
        {
            SetDefaultPixelsPerUnit();
            return;
        }

        PropertyModification mod = preset.PropertyModifications
            .FirstOrDefault(m => m.propertyPath == PPU_PROPERTY_PATH);

        if (mod.propertyPath != PPU_PROPERTY_PATH)
        {
            SetDefaultPixelsPerUnit();
            return;
        }

        CanvasPixelsPerUnit = float.Parse(mod.value);
    }

    private static void SetDefaultPixelsPerUnit()
    {
        EditorUtility.DisplayDialog("Preset Missing",
            "PixelsPerUnit cannot be set, because a TextureImporter Preset cannot be found! Default value is used.",
            "Ok");
        CanvasPixelsPerUnit = DEFAULT_PPU;
    }



    private const int DEFAULT_PPU = 100;

    private const string PPU_PROPERTY_PATH = "m_SpritePixelsToUnits";
    private static readonly string PRESET_SEARCH = $"t:{typeof(Preset).Name}";

    private static float CanvasPixelsPerUnit { get; set; }
    #endregion
}