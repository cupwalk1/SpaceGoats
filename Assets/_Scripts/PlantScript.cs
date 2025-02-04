using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantScript : MonoBehaviour
{
   public TileData tileData;
   public Sprite ripeSprite;
   public Sprite unripeSprite;
   public Vector3Int position;
   public bool IsRipe = true;
   public Tilemap tilemap;

   private void Harvest()
   {
      if (!IsRipe) return;
      IsRipe = false;
      tilemap.RefreshTile(position);
   }
   
   public void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.gameObject.CompareTag("Player")) return;
      Harvest();
   }
}