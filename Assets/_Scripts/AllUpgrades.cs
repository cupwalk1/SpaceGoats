using System.Linq;
using _Scripts;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AllUpgrades", menuName = "ScriptableObjects/AllUpgrades")]
public class AllUpgrades : ScriptableObject
{
   public GoatStats goatStats;
   public ResourceInfo resourceInfo;

   public void IncreasePipeDuration(int i)
   {
      resourceInfo.averageGeneratorBreakingTime += i;
   }

   public void MaxOutPipeDuration()
   {
      resourceInfo.averageGeneratorBreakingTime = -1;
      resourceInfo.semiRangeGeneratorBreakingTime = 0;
      ResourceManager.Instance.Resources.FindAll(r => r.Type == ResourceData.ResourceType.Energy && !r.IsRipe)
         .ForEach(r => r.TimeToRipe = -1);
   }

   public void IncreaseEnergyCapacity(int i)
   {
      resourceInfo.energyAvailable += i;
   }

   public void UnlockSerra()
   {
      FindFirstObjectByType<UIManager>().OnSerraClick();
   }

   public void UnlockSecondaCaverna()
   {
      FindFirstObjectByType<UIManager>().OnSecCavClick();
   }

   public void IncreaseGoatSpeed()
   {
      var um = FindFirstObjectByType<UpgradeManager>();
      um.SpeedSlider.EnablePanel();
      um.SpeedSlider.Slider.maxValue ++;
      if (um.SpeedSlider.Slider.maxValue == 2) um.SpeedSlider.Slider.minValue = 1;
   }
   
   public void IncreaseGoatJump()
   {
      var um = FindFirstObjectByType<UpgradeManager>();
      um.JumpSlider.EnablePanel();
      um.JumpSlider.Slider.maxValue ++;
      if (um.JumpSlider.Slider.maxValue == 2) um.JumpSlider.Slider.minValue = 1;
   }

   public void IncreaseGoatHealth(int i)
   {
      goatStats.maxGoatHealth += i;
   }

   public void IncreaseGoatOxygen(int i)
   {
      goatStats.maxTimeOxygen += i;
   }

   public void ReducePlantRegenTimePercent(int i)
   {
      resourceInfo.plantRegenTime = (int)(resourceInfo.plantRegenTime * (1 - i / 100f));
   }

   public void IncreaseFruitsPerPlant(int i)
   {
      resourceInfo.fruitsPerPlant += i;
   }

   public void IncreaseMaterialsPerOre(int i)
   {
      resourceInfo.materialsPerOre += i;
   }

   public void ContructTrivella()
   {
      ResourceManager.Instance.StartMiner();
   }
}