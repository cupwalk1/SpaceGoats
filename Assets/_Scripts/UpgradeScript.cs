using System;
using System.Linq;
using _Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScript : MonoBehaviour
{
   public TMP_Text Name;
   public TMP_Text Description;
   public GameObject EnergyCost;
   public GameObject FoodCost;
   public GameObject MaterialCost;
   public Button BuyButton;
   public UpgradeData UpgradeData;
   public UpgradeStack UpgradeStack;
   
   private ResourceManager _rm;
   
   private void Start()
   {
      //update UI later with OnResourcesChanged
      
      _rm = ResourceManager.Instance;
      _rm.OnResourcesChanged.AddListener(RefreshButton);
      Name.text = UpgradeData.Name;
      Description.text = UpgradeData.Description;
      BuyButton.onClick.AddListener(BuyUpgrade);
      
      if (UpgradeData.Cost.fruits > 0)
      {
         FoodCost.SetActive(true);
         FoodCost.GetComponentInChildren<TMP_Text>().text = UpgradeData.Cost.fruits.ToString();
      }
      if (UpgradeData.Cost.materials > 0)
      {
         MaterialCost.SetActive(true);
         MaterialCost.GetComponentInChildren<TMP_Text>().text = UpgradeData.Cost.materials.ToString();
      }
      if (UpgradeData.Cost.power > 0)
      {
         EnergyCost.SetActive(true);
         EnergyCost.GetComponentInChildren<TMP_Text>().text = UpgradeData.Cost.power.ToString() + " kW";
      }
      
   }
   

   
   void BuyUpgrade()
   {
      Debug.Log("Buying upgrade");
      if (_rm.HasEnoughResources(UpgradeData.Cost))
      {
         _rm.DeductResources(UpgradeData.Cost);
         UpgradeData.OnUpgrade.Invoke();
         UpgradeStack.UpgradeLevel++;
         
         bool victory = true;
         UpgradeManager.Instance.UpgradeStacks.Where(s => s.IsRequiredForVictory).ToList().ForEach(s => { if (s.UpgradeLevel < s.Upgrades.Length) victory = false; });
         if (victory && !GameManager.Instance.IsFreePlay) GameManager.Instance.Victory();
         
         SoundManager.Instance.PlaySFX(SoundManager.Instance.upgrade);
      }
   }

   private void RefreshButton()
   {
      if (_rm.HasEnoughResources(UpgradeData.Cost))
      {
         BuyButton.interactable = true;
      }
      else
      {
         BuyButton.interactable = false;
      }
   }
}