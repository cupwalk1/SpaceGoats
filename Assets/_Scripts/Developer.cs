using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Tilemaps;

public class Developer
{
   // Add a Developer menu at the top of the Unity editor
   [MenuItem("Developer/Respawn Player")]
   private static void Respawn()
   {
      GameObject player = GameObject.Find("Player");
      player.transform.position = new Vector3(-8, -2.7f);
      player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
      player.GetComponent<PlayerHealth>().ResetHealth();
      player.GetComponent<PlayerManager>().ShouldMoveCamera = true;
      player.GetComponent<PlayerManager>().EnableMoveJump();
      player.GetComponent<Collider2D>().enabled = true;
   }
   
   [MenuItem("Developer/Check PostProcessing")]
   private static void CheckPostProcessing()
   {
      // Ensure the PostProcessLayer is added to the camera
      var camera = Camera.main;
      var postProcessLayer = camera.GetComponent<PostProcessLayer>();
      if (postProcessLayer == null)
      {
         postProcessLayer = camera.gameObject.AddComponent<PostProcessLayer>();
      }
      postProcessLayer.volumeLayer = LayerMask.GetMask("PostProcessing");

// Ensure the PostProcessVolume is configured correctly
      var postProcessVolume = GameObject.FindFirstObjectByType<PostProcessVolume>();
      if (postProcessVolume != null)
      {
         postProcessVolume.isGlobal = true;
         // Ensure the profile is assigned and contains the Vignette effect
         if (postProcessVolume.profile == null)
         {
            Debug.LogError("PostProcessProfile is not assigned to the PostProcessVolume.");
         }
         else
         {
            Vignette vignette;
            if (!postProcessVolume.profile.TryGetSettings(out vignette))
            {
               Debug.LogError("Vignette effect is not added to the PostProcessProfile.");
            }
         }
      }
      else
      {
         Debug.LogError("PostProcessVolume is not found in the scene.");
      }
   }
   
   [MenuItem("Developer/Refresh Tilemaps")]
   private static void RefreshTileMaps()
   {
      Tilemap tilemap= GameObject.Find("Plants").GetComponent<Tilemap>();
      tilemap.RefreshAllTiles();
   }
   //erase plants from plants.json
   [MenuItem("Developer/Erase Plants")]
   private static void ErasePlants()
   {
      PlantManager.Plants.Clear();
      PlantManager.SavePlants();
   }
   
}