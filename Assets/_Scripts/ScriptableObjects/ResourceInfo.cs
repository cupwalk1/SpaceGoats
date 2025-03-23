using UnityEngine;

[CreateAssetMenu(fileName = "ResourceInfo", menuName = "ScriptableObjects/ResourceInfo")]
public class ResourceInfo : ScriptableObject       
{      
   [Header("Plant Stats")] 
   public int fruitsPerPlant;
   public int plantRegenTime;
   public int maxFruitsInWarehouse;
   public int fruitEatingRate;
   
   //material stats
   [Header("Material Stats")]
   public int materialsPerOre;
   public int oreRegenTime;
   
   //energy stats
   [Header("Energy Stats")]
   public int energyAvailable;
   public int energyPerGenerator;
   public int averageGeneratorBreakingTime;
   public int semiRangeGeneratorBreakingTime;
   
   public void CopyFrom(ResourceInfo other)
   {
      fruitsPerPlant = other.fruitsPerPlant;
      plantRegenTime = other.plantRegenTime;
      maxFruitsInWarehouse = other.maxFruitsInWarehouse;
      fruitEatingRate = other.fruitEatingRate;
      
      materialsPerOre = other.materialsPerOre;
      oreRegenTime = other.oreRegenTime;
      
      energyAvailable = other.energyAvailable;
      energyPerGenerator = other.energyPerGenerator;
      averageGeneratorBreakingTime = other.averageGeneratorBreakingTime;
      semiRangeGeneratorBreakingTime = other.semiRangeGeneratorBreakingTime;
   }
}