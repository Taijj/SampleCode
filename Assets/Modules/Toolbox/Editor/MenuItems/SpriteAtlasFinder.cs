using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// Logs the names of all SpriteAtlasses that contain the selected Sprites.
/// Use this to resolve the problem of sprites being part of multiple atlasses.
/// </summary>
public class SpriteAtlasFinder : Editor
{
    #region Main
    private const string MENU_PATH = "Assets/Ble/Find Sprite Atlas %#d"; // Ctrl+Shift+d

    [MenuItem(MENU_PATH, isValidateFunction:true)]
    public static bool IsValid()
    {
        Object[] sel = Selection.objects;
        if (sel.Length == 0)
            return false;

        IEnumerable<Sprite> selectedSprites = sel.OfType<Sprite>();
        IEnumerable<Sprite> textureSprites = sel.OfType<Texture2D>()
            .Select(t => AssetDatabase.GetAssetPath(t))
            .SelectMany(p => AssetDatabase.LoadAllAssetsAtPath(p))
            .OfType<Sprite>();

        Sprites = selectedSprites
            .Concat(textureSprites)
            .ToArray();

        return Sprites.Length > 0;
    }

    [MenuItem(MENU_PATH)]
    public static void FindAtlasses()
    {
        if (false == IsValid())
            return;

        SearchAtlasses();
    }
    private static Sprite[] Sprites { get; set; }
    #endregion



    #region Search
    private static readonly string SPRITE_ATLAS_SEARCH = $"t:{typeof(SpriteAtlas).Name}";
    private static readonly string SPRITE_SEARCH = $"t:{typeof(Sprite).Name}";

    private static void SearchAtlasses()
    {
        IEnumerable<SpriteAtlas> atlasses = AssetDatabase.FindAssets(SPRITE_ATLAS_SEARCH)
           .Select(g => AssetDatabase.GUIDToAssetPath(g))
           .Select(p => AssetDatabase.LoadAssetAtPath<SpriteAtlas>(p));

        foreach (Sprite sprite in Sprites)
        {
            List<SpriteAtlas> containingAtlasses = new List<SpriteAtlas>();
            foreach (SpriteAtlas atlas in atlasses)
            {
                List<Sprite> packedSprites = GetPackedFrom(atlas);
                if (packedSprites.Contains(sprite))
                    containingAtlasses.Add(atlas);
            }

            string log = string.Empty;
            Color color;
            if (containingAtlasses.Count != 0)
            {
                log = $"Atlasses containing '{sprite.name}':\n";
                foreach (SpriteAtlas containingAtlas in containingAtlasses)
                    log += containingAtlas.name + ", ";
                color = ColorAddons.Lime;
            }
            else
            {
                log = $"'{sprite.name}' is not part of any atlas.";
                color = ColorAddons.Orange;
            }
            Note.Log(log, color);
        }
    }

    private static List<Sprite> GetPackedFrom(SpriteAtlas atlas)
    {
        List<Sprite> result = new List<Sprite>();
        Object[] packed = SpriteAtlasExtensions.GetPackables(atlas);
        foreach (Object ob in packed)
        {
            if (ob is Sprite)
            {
                result.Add(ob as Sprite);
                continue;
            }

            if (ob is Texture2D)
            {
                string path = AssetDatabase.GetAssetPath(ob);
                result.AddRange(AssetDatabase.LoadAllAssetsAtPath(path)
                    .OfType<Sprite>());
                continue;
            }

            if (ob is DefaultAsset)
            {
                string path = AssetDatabase.GetAssetPath(ob);
                result.AddRange(
                    AssetDatabase.FindAssets(SPRITE_SEARCH, new string[] { path })
                    .Select(g => AssetDatabase.GUIDToAssetPath(g))
                    .Select(p => AssetDatabase.LoadAssetAtPath<Sprite>(p))
                    );
            }
        }

        return result;
    }
    #endregion
}