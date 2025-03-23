using TMPro;
using UnityEngine;

namespace _Scripts.Panels
{
   public class GameOverPanel : UIPanel
   {
      [SerializeField] private TextMeshProUGUI energyText;
      [SerializeField] private TextMeshProUGUI plantText;
      
      public override void Show()
      {
         base.Show();
         if(ResourceManager.Instance.TotalEnergy == 0)
         {
            energyText.enabled = true;
            plantText.enabled = false;
         }
         else
         {
            energyText.enabled = false;
            plantText.enabled = true; 
         }
      }
   }
}