using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class PlantTileAsset
{
   [MenuItem("Assets/Create/Tile/ResourceTile")]
   public static void CreateResourceTile()
   {
      string path = EditorUtility.SaveFilePanelInProject("Save Resource Tile", "New Resource Tile", "asset", "Save Resource Tile", "Assets");
      if (string.IsNullOrEmpty(path))
         return;

      ResourceTile tile = ScriptableObject.CreateInstance<ResourceTile>();
      AssetDatabase.CreateAsset(tile, path);
      AssetDatabase.SaveAssets();
      EditorUtility.FocusProjectWindow();
      Selection.activeObject = tile;
   }
}