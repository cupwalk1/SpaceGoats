using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class PlantTileAsset
{
   [MenuItem("Assets/Create/Tile/PlantTile")]
   public static void CreatePlantTile()
   {
      string path = EditorUtility.SaveFilePanelInProject("Save Plant Tile", "New Plant Tile", "asset", "Save Plant Tile", "Assets");
      if (path == "")
         return;

      PlantTile tile = ScriptableObject.CreateInstance<PlantTile>();
      AssetDatabase.CreateAsset(tile, path);
      AssetDatabase.SaveAssets();
      EditorUtility.FocusProjectWindow();
      Selection.activeObject = tile;
   }
}