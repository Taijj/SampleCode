#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Generates a custom RuleTile from a 2D Texture Spritesheet given a RuleTile-Template.
	/// </summary>
	public class RuleTileGenerator
	{
		#region Common
		private const string MENU_ITEM_PATH = "Assets/Ble/Generate Rule Tile";

		private const string TEMPLATE_RULE_TILE = "Assets/Ble/Scripts/Editor/RuleTileGenerator/TemplateSnowBoxCustomRuleTile.asset";
		private const string CREATED_RULE_TILE_PATH = "Assets/Ble/Art/Tiles/";
		private const string CREATED_SPRITE_ATLAS_PATH = "Assets/Ble/Art/Tiles/";



		[MenuItem(MENU_ITEM_PATH, isValidateFunction: true)]
		private static bool IsValid()
		{
			Object sel = Selection.activeObject;
			if (sel.IsNull())
				return false;

			if (sel.GetType() != typeof(Texture2D))
				return false;

			Texture = sel as Texture2D;
			string path = AssetDatabase.GetAssetPath(Texture);
			Importer = (TextureImporter)TextureImporter.GetAtPath(path);
			if (Importer.IsNull())
				return false;

			Template = AssetDatabase.LoadAssetAtPath<TileConfig>(TEMPLATE_RULE_TILE);
			if (Template.IsNull())
				return false;

			return true;
		}

		private static Texture2D Texture { get; set; }
		private static TextureImporter Importer { get; set; }
		private static TileConfig Template { get; set; }
		#endregion



		#region Flow
		[MenuItem(MENU_ITEM_PATH)]
		private static void GenerateRuleTile()
		{
			if (false == IsValid())
				return;

			Importer.spriteImportMode = SpriteImportMode.Multiple;
			Importer.maxTextureSize = NextPowerOfTwo();
			SaveAndReimportTexture();

			SliceSpriteSheet();

			CreateSpriteAtlasAsset($"{CREATED_SPRITE_ATLAS_PATH}{Texture.name}SpriteAtlas.asset");

			TileConfig createRuleTile =
				CreateRuleTileAsset($"{CREATED_RULE_TILE_PATH}{Texture.name}RuleTile.asset");
			FillWithRules(createRuleTile);

			SaveAsset(createRuleTile);
		}

		private static int NextPowerOfTwo()
		{
			Importer.GetSourceTextureWidthAndHeight(out int sourceWidth, out int sourceHeight);
			int maxSize = Mathf.Max(sourceWidth, sourceHeight);

			int logBaseTwo = (int)(Mathf.Log(maxSize) / Mathf.Log(2));

			if (Mathf.Pow(2, logBaseTwo) == maxSize)
				return maxSize;

			return (int)Mathf.Pow(2, logBaseTwo + 1);
		}
		#endregion



		#region Sprite Slicing
		private static void SliceSpriteSheet()
		{
			int individualSpriteSize = 100;
			int columnCount = Texture.width / individualSpriteSize;
			int rowCount = Texture.height / individualSpriteSize;

			List<SpriteMetaData> individualSpritesMetaDatas = new List<SpriteMetaData>();

			for (int row = rowCount; row > 0; row--)
			{
				for (int column = 0; column < columnCount; column++)
				{
					SpriteMetaData spriteMetaData = new SpriteMetaData();
					spriteMetaData.pivot = new Vector2(.5f, .5f);
					spriteMetaData.alignment = (int)SpriteAlignment.Center;
					spriteMetaData.name = $"{Texture.name}_{row - 1}_{column}";
					spriteMetaData.rect = new Rect(column * individualSpriteSize,
											(rowCount - row) * individualSpriteSize,
											individualSpriteSize,
											individualSpriteSize);

					individualSpritesMetaDatas.Add(spriteMetaData);
				}
			}
			Importer.spritesheet = individualSpritesMetaDatas.ToArray();
			SaveAndReimportTexture();
		}

		private static void SaveAndReimportTexture()
		{
			EditorUtility.SetDirty(Importer);
			Importer.SaveAndReimport();
		}
		#endregion



		#region SpriteAtlas Creation
		private static SpriteAtlas CreateSpriteAtlasAsset(string path)
		{
			SpriteAtlas spriteAtlas = new SpriteAtlas();
			AssetDatabase.CreateAsset(spriteAtlas, path);

			SpriteAtlasExtensions.Add(spriteAtlas, GetSpritesFromTexture(Texture));

			SaveAsset(spriteAtlas);

			return spriteAtlas;
		}

		private static void SaveAsset(Object objectToSave)
		{
			EditorUtility.SetDirty(objectToSave);
			AssetDatabase.SaveAssets();
		}
		#endregion



		#region RuleTile Creation
		private static TileConfig CreateRuleTileAsset(string path)
		{
			TileConfig createdSnowBoxCustomRuleTile = ScriptableObject.CreateInstance<TileConfig>();

			AssetDatabase.CreateAsset(createdSnowBoxCustomRuleTile, path);

			return createdSnowBoxCustomRuleTile;
		}

		private static void FillWithRules(TileConfig tile)
		{
			Sprite[] sprites = GetSpritesFromTexture(Texture);

			for (int i = 0; i < sprites.Length; i++)
			{
				RuleTile.TilingRule tilingRule = new RuleTile.TilingRule();
				tilingRule = Template.m_TilingRules[i];
				tilingRule.m_Sprites[0] = sprites[i];
				tile.m_TilingRules.Add(tilingRule);
			}
		}

		private static Sprite[] GetSpritesFromTexture(Texture2D activeTexture)
		{
			string path = AssetDatabase.GetAssetPath(activeTexture);
			return AssetDatabase.LoadAllAssetRepresentationsAtPath(path).OfType<Sprite>().ToArray();
		}
		#endregion
	}
}
#endif