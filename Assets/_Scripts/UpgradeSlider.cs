using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UpgradeSlider
{
   public GameObject Panel;
   public Slider Slider;
   public TextMeshProUGUI Text;
   
   public void UpdateSlider()
   {
      Text.text = Slider.value.ToString();
   }

   public void EnablePanel() => Panel.SetActive(true);
   public void DisablePanel() => Panel.SetActive(false);
}