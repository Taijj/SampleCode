using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Selects 2 Sprites, that are sorted in the same layer with the same order,
/// from all children of the currently selected GameObjects in the Hierarchy.
///
/// Use this to identify and resolve potential Sprite sorting issues.
/// </summary>
///
public class SameSortingSpriteSelector
{
    private const string MENU_PATH = "GameObject/Ble/Select Sprites with equal Sorting %#e"; // Ctrl+Shift+e

    [MenuItem(MENU_PATH, isValidateFunction: true)]
    private static bool GetSelectedSpriteRenderers()
    {
        Renderers = Selection.objects
            .OfType<GameObject>()
            .SelectMany(o => o.GetComponentsInChildren<SpriteRenderer>())
            .ToArray();
        return Renderers.Length > 0;
    }

    [MenuItem(MENU_PATH)]
    public static void SelectSpritesWithEqualSorting()
    {
        if (false == GetSelectedSpriteRenderers())
            return;

        for (int i = 0; i < Renderers.Length; i++)
        {
            SpriteRenderer first = Renderers[i];
            for (int j = 0; j < Renderers.Length; j++)
            {
                if (i == j)
                    continue;

                SpriteRenderer second = Renderers[j];
                if (IsSortingEqual(first, second))
                    break;
            }
        }
    }

    private static bool IsSortingEqual(SpriteRenderer first, SpriteRenderer second)
    {
        bool isSameSorting = first.sortingLayerID == second.sortingLayerID
                    && first.sortingOrder == second.sortingOrder;

        if (false == isSameSorting)
            return false;

        Bounds bounds1 = first.bounds;
        Bounds bounds2 = second.bounds;

        bounds1.Expand(-0.1f);
        bounds2.Expand(-0.1f);
        if (bounds1.Intersects(bounds2))
        {
            Object[] newSelection = new Object[2];
            newSelection[0] = first.gameObject;
            newSelection[1] = second.gameObject;
            Selection.objects = newSelection;
            return true;
        }
        return false;
    }

    private static SpriteRenderer[] Renderers { get; set; }
}