
using UnityEngine;

public static class LayerAddons
{
    #region Default Layers
    public static readonly int DEFAULT_LAYER = LayerMask.NameToLayer("Default");
    public static readonly int TRANSPARENT_FX_LAYER = LayerMask.NameToLayer("TransparentFX");
    public static readonly int IGNORE_RAYCAST_LAYER = LayerMask.NameToLayer("Ignore Raycast");
    public static readonly int WATER_LAYER = LayerMask.NameToLayer("Water");
    public static readonly int UI_LAYER = LayerMask.NameToLayer("UI");
    #endregion



    #region Layer Checks
    private static bool IsIn(this GameObject @this, int layer) => @this.layer == layer;
    private static bool IsIn(this Component @this, int layer) => IsIn(@this.gameObject, layer);

    private static bool IsIn(this GameObject @this, string layerName)
    {
        return @this.layer == LayerMask.NameToLayer(layerName);
    }

    private static bool IsIn(this Component @this, string layerName)
    {
        return IsIn(@this.gameObject, layerName);
    }

    public static bool IsIn(this GameObject @this, LayerMask mask)
    {
        return (1 << @this.layer | mask.value) == mask.value;
    }

    public static bool IsIn(this Component @this, LayerMask mask)
    {
        return IsIn(@this.gameObject, mask);
    }
    #endregion



    #region Layer Assignments
    public static void SetTo(GameObject target, int layer) => target.layer = layer;
    public static void SetTo(this Component @this, int layer) => SetTo(@this.gameObject, layer);



    public static void SetToRecursive(this GameObject @this, int layer)
    {
        @this.layer = layer;
        foreach (Transform child in @this.transform)
            child.gameObject.SetToRecursive(layer);
    }

    public static void SetToRecursive(this GameObject @this, string layerName)
    {
        SetToRecursive(@this, LayerMask.NameToLayer(layerName));
    }

    public static void SetToRecursive(this Component @this, int layer)
    {
        SetToRecursive(@this.gameObject, layer);
    }

    public static void SetToRecursive(this Component @this, string layerName)
    {
        SetToRecursive(@this, LayerMask.NameToLayer(layerName));
    }
    #endregion
}