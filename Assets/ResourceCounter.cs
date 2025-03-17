using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResourceCounter : MonoBehaviour
{
   public TextMeshProUGUI materialText;
   public TextMeshProUGUI energyText;
   public TextMeshProUGUI plantText;
   public Slider energySlider;
   public Slider plantSlider;
   private ResourceManager _resourceManager;

   private void Start()
   {
      _resourceManager = ResourceManager.Instance;
      materialText.text = _resourceManager.TotalMaterials.ToString();
      energyText.text = _resourceManager.TotalEnergy.ToString();
   }

   void Update()
   {
      materialText.text = _resourceManager.TotalMaterials.ToString();

      var count = _resourceManager.Resources.Count(s => s.Type == ResourceData.ResourceType.Energy);
      energyText.text =
         $"{(_resourceManager.TotalEnergy * ResourceManager.Instance.EnergyProducedByEachGenerator).ToString()}/{count * ResourceManager.Instance.EnergyProducedByEachGenerator} kW";
      energySlider.value = (float)_resourceManager.TotalEnergy / count;

      plantText.text =
         $"{_resourceManager.TotalFood.ToString() }/{ResourceManager.Instance.PlantMaxCapacity}"; 
      plantSlider.value = (float)_resourceManager.TotalFood / ResourceManager.Instance.PlantMaxCapacity;
   }
}