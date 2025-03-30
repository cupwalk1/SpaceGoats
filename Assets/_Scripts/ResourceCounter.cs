using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ResourceCounter : MonoBehaviour
{
   public TextMeshProUGUI materialText;
   public TextMeshProUGUI energyText;
   public TextMeshProUGUI plantText;
   public Slider energySlider;
   private ResourceManager _resourceManager;

   private void Start()
   {
      _resourceManager = ResourceManager.Instance;
      _resourceManager.OnResourcesChanged.AddListener(UpdateUI);
      materialText.text = _resourceManager.TotalMaterials.ToString();
      InvokeRepeating("UpdateResources", 0, 0.25f);
   }

   void UpdateResources() => _resourceManager.OnResourcesChanged.Invoke();

   private void UpdateUI()
   {
      materialText.text = _resourceManager.TotalMaterials.ToString();
      energyText.text = _resourceManager.TotalEnergy.ToString() + " / " +
                        _resourceManager.ResourceInfo.energyAvailable.ToString() + " kW";
      energySlider.onValueChanged.RemoveAllListeners();
      energySlider.value = (float)_resourceManager.TotalEnergy / _resourceManager.ResourceInfo.energyAvailable;

      plantText.text =
         $"{_resourceManager.TotalFood.ToString()}";

   }
}