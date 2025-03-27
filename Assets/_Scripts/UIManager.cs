using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public enum PanelType
{
   Settings,
   Credits,
   Potenziamenti,
   GameOver,
   Potenziamenti2,
   Victory
}

public abstract class UIPanel : MonoBehaviour
{
   [SerializeField] protected Transform menu;
   [SerializeField] protected Transform menuSpawn;

   public virtual void Show()
   {
      SoundManager.Instance.PlaySFX(SoundManager.Instance.click);
      gameObject.SetActive(true); 
      if (menu) menu.LeanMove(Vector3.zero, 1f).setEaseOutElastic().period = 1.2f;
      Debug.Log($"Showing panel {this.name}");
   }

   public virtual void Hide(bool ommitNoise = false)
   {
      if (menu)
      {
         menu.LeanMove(menuSpawn.position, 1f).setEaseInQuad().setOnComplete(() => gameObject.SetActive(false));
         if (!ommitNoise) SoundManager.Instance.PlaySFX(SoundManager.Instance.backBtn);
      }
      else
      {
         gameObject.SetActive(false);
      }
   }
}

public class UIManager : MonoBehaviour
{
   public GameObject serra;
   public GameObject secCav;
   
   public ResourceInfo ResourceInfo;
   public GoatStats GoatStats;
   
   public ResourceInfo DefaultResourceInfo;
   public GoatStats DefaultGoatStats;
   
   private Dictionary<PanelType, UIPanel> panels = new Dictionary<PanelType, UIPanel>();
   [SerializeField] private ResourceCounter rc;
   [SerializeField] private ResourceManager _rm;
   [SerializeField] private Slider volumeSlider;
   
   [SerializeField] private UpgradeSlider VelocityPanel;
   [SerializeField] private UpgradeSlider JumpPanel; 
   
   private void Awake()
   {
      foreach (var panel in FindObjectsOfType<UIPanel>())
      {
         if (Enum.TryParse(panel.name, out PanelType panelType))
         {
            panels[panelType] = panel;
            panel.Hide(true);
         }
      }
   }

   private void Start()
   {
      GameManager.Instance.ResetGame.AddListener(RestartGame);
      
      serra.SetActive(false);
      secCav.SetActive(false);
      _rm = ResourceManager.Instance;
      volumeSlider.onValueChanged.AddListener((float value) => PlayerPrefs.SetFloat("volume", value));
      volumeSlider.value = PlayerPrefs.GetFloat("volume");
      _rm.OnResourcesChanged.AddListener(CheckGameOver);
        
      GameManager.Instance.OnGameWin.AddListener(ShowVictory);
      CheckGameOver();
      GameManager.Instance.OnGameOver.AddListener(ShowGameOver);
      PlayerPrefs.SetFloat("volume", PlayerPrefs.GetFloat("volume", 1f));
      PlayerPrefs.SetInt("vibration", PlayerPrefs.GetInt("vibration", 1));
      PlayerPrefs.Save();
   }


   private void CheckGameOver()
   {
      if ((_rm.TotalFood == 0 || _rm.TotalEnergy == 0) && !GameManager.Instance.IsGameOver)GameManager.Instance.OnGameOver.Invoke();
   }

   public void TogglePanel(PanelType panelType)
   {
      if (panels.TryGetValue(panelType, out UIPanel panel))
      {
         if (panel.gameObject.activeSelf) panel.Hide();
         else panel.Show();
      }
   }
    
   public void HidePanel(PanelType panelType, bool ommitNoise = false)
   {
      if (panels.TryGetValue(panelType, out UIPanel panel))
      {
         panel.Hide(ommitNoise);
      }
   }

   public void LoadGame(string level) => SceneManager.LoadScene(level);

   public void CallReset()
   {
      GameManager.Instance.ResetGame.Invoke();
   }

   public void RestartGame()
   {
      foreach (var resource in _rm.Resources)
      {
         resource.TimeToRipe = (resource.Type == ResourceData.ResourceType.Energy) ? _rm.RandomMaxEnergyTime : 0;
      }
      serra.SetActive(false);
      secCav.SetActive(false);
      HidePanel(PanelType.Victory, true);
      HideGameOver(true);
      _rm.SaveResources();
      
      var volume = PlayerPrefs.GetFloat("volume", 1f);
      PlayerPrefs.DeleteAll();
      PlayerPrefs.SetFloat("volume", volume);
      PlayerPrefs.Save();
      
      ResourceInfo.CopyFrom(DefaultResourceInfo);
      GoatStats.CopyFrom(DefaultGoatStats);
      _rm.TotalMaterials = 0;
      _rm.TotalFood = Mathf.RoundToInt(_rm.ResourceInfo.maxFruitsInWarehouse / 1.5f);
      GameManager.Instance.IsGameOver = false;
   }

   private void HideGameOver(bool omit = false)
   {
      HidePanel(PanelType.GameOver, omit);
      if (GameManager.Instance.IsGameOver) GameManager.Instance.IsGameOver = false;
   }

   void ShowPanel(PanelType panelType)
   {if(panels.TryGetValue(panelType, out UIPanel panel)) panel.Show();
      else Debug.LogError($"Panel {panelType} not found.");
   }

   public void ToggleSettings() => TogglePanel(PanelType.Settings);
   public void ToggleCredits() => TogglePanel(PanelType.Credits);
   public void TogglePotenziamenti() => TogglePanel(PanelType.Potenziamenti);
   public void TogglePotenziamenti2() => TogglePanel(PanelType.Potenziamenti2);
   public void ShowGameOver()
   {
      HidePotenziamenti(true);
      HidePotenziamenti2(true);
      HideSettings(true);
      HidePanel(PanelType.Credits, true);
      ShowPanel(PanelType.GameOver);
      SoundManager.Instance.PlayGameOver();
   }

   public void ShowVictory()
   {
      HidePotenziamenti(true);
      HidePotenziamenti2(true);
      HideSettings(true);
      HidePanel(PanelType.Credits, true);
      ShowPanel(PanelType.Victory);
      SoundManager.Instance.PlayWin();
   }

   public void HidePotenziamenti(bool ommitNoise = false) => HidePanel(PanelType.Potenziamenti, ommitNoise);
   public void HidePotenziamenti2(bool ommitNoise = false) => HidePanel(PanelType.Potenziamenti2, ommitNoise);
   public void HideSettings(bool ommitNoise = false) => HidePanel(PanelType.Settings, ommitNoise);

   public void HideVictory(bool ommit = false)
   {
      HidePanel(PanelType.Victory, ommit);
      GameManager.Instance.IsFreePlay = true;
   }
    
   public void OnSerraClick() => serra.SetActive(true);
   public void OnExitClick() => Application.Quit();
   public void OnDonateClick() => Application.OpenURL("https://buymeacoffee.com/cupwalk1");
   public void OnCoseUnaCittaSostenibileClick() => Application.OpenURL("https://unric.org/it/obiettivo-11-rendere-le-citta-e-gli-insediamenti-umani-inclusivi-sicuri-duraturi-e-sostenibili/");

   public void OnSecCavClick()
   {
      secCav.SetActive(true);
   }
}