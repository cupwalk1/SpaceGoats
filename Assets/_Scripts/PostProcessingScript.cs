using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingScript : MonoBehaviour
{
   [SerializeField] private Volume postProcessVolume;
   [SerializeField] private float defaultVignette = 0.2f;
   [SerializeField] private float deathVignette = 0.4f;

   private Vignette vignette;

   void Start()
   {
      if (postProcessVolume == null)
      {
         Debug.LogError("PostProcessVolume is not assigned.");
         return;
      }

      if (postProcessVolume.profile == null)
      {
         Debug.LogError("PostProcessVolume profile is not assigned.");
         return;
      }

      if (postProcessVolume.profile.TryGet(out vignette))
      {
         vignette.intensity.value = defaultVignette;
         Debug.Log("Vignette effect found and set to default intensity.");
      }
      else
      {
         Debug.LogError("Vignette effect is not found in the PostProcessVolume profile.");
      }
   }

   public void ApplyVignetteOnDamaged()
   {
      if (vignette != null)
      {
         vignette.intensity.value = deathVignette;
         Debug.Log("Vignette intensity set to deathVignette.");
      }
      else
      {
         Debug.LogWarning("Vignette not found in Post Processing Volume");
      }
   }
}