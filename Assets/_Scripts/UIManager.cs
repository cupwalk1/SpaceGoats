using System;
using System.Collections.Generic;
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
        gameObject.SetActive(true);
        if (menu) menu.LeanMove(Vector3.zero, 1f).setEaseOutElastic().period = 1.2f;
    }

    public virtual void Hide()
    {
        if (menu)
        {
            menu.LeanMove(menuSpawn.position, 1f).setEaseInQuad().setOnComplete(() => gameObject.SetActive(false));
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
    private Dictionary<PanelType, UIPanel> panels = new Dictionary<PanelType, UIPanel>();
    [SerializeField] private ResourceCounter rc;
    [SerializeField] private ResourceManager _rm;
    [SerializeField] private Slider volumeSlider;
    
    private void Awake()
    {
        foreach (var panel in FindObjectsOfType<UIPanel>())
        {
            if (Enum.TryParse(panel.name, out PanelType panelType))
            {
                panels[panelType] = panel;
                panel.Hide();
            }
        }
    }

    private void Start()
    {
        serra.SetActive(false);
        _rm = ResourceManager.Instance;
        volumeSlider.onValueChanged.AddListener((float value) => PlayerPrefs.SetFloat("volume", value));
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        _rm.OnResourcesChanged.AddListener(CheckGameOver);
        if (_rm.TotalFood == 0 || _rm.TotalEnergy == 0) GameManager.Instance.OnGameOver.Invoke();
        GameManager.Instance.OnGameOver.AddListener(ShowGameOver);
        PlayerPrefs.SetFloat("volume", PlayerPrefs.GetFloat("volume", 1f));
        PlayerPrefs.SetInt("vibration", PlayerPrefs.GetInt("vibration", 1));
        PlayerPrefs.Save();
    }

    private void CheckGameOver()
    {
        if (_rm.TotalFood == 0 || _rm.TotalEnergy == 0) GameManager.Instance.OnGameOver.Invoke();
    }

    public void TogglePanel(PanelType panelType)
    {
        if (panels.TryGetValue(panelType, out UIPanel panel))
        {
            if (panel.gameObject.activeSelf) panel.Hide();
            else panel.Show();
        }
    }

    public void LoadGame(string level) => SceneManager.LoadScene(level);

    public void OnRetryClick()
    {
        foreach (var resource in _rm.Resources)
        {
            resource.TimeToRipe = (resource.Type == ResourceData.ResourceType.Energy) ? _rm.RandomMaxEnergyTime : 0;
        }
        _rm.SaveResources();
        PlayerPrefs.DeleteAll();
        _rm.TotalFood = Mathf.RoundToInt(_rm.ResourceInfo.maxFruitsInWarehouse / 1.5f);
        _rm.TotalMaterials = 0;
        UpgradeManager.Instance.UpdateStacks();
        TogglePanel(PanelType.GameOver);
    }
    void ShowPanel(PanelType panelType)
    {if(panels.TryGetValue(panelType, out UIPanel panel)) panel.Show();
    else Debug.LogError($"Panel {panelType} not found.");
    }

    public void ToggleSettings() => TogglePanel(PanelType.Settings);
    public void ToggleCredits() => TogglePanel(PanelType.Credits);
    public void TogglePotenziamenti() => TogglePanel(PanelType.Potenziamenti);
    public void TogglePotenziamenti2() => TogglePanel(PanelType.Potenziamenti2);
    public void ToggleGameOver() => TogglePanel(PanelType.GameOver);
    public void ShowGameOver() => ShowPanel(PanelType.GameOver);
    public void ShowVictory() => ShowPanel(PanelType.Victory);
    public void ToggleVictory() => TogglePanel(PanelType.Victory);
    
    public void OnSerraClick() => serra.SetActive(true);
    public void OnExitClick() => Application.Quit();
    public void OnDonateClick() => Application.OpenURL("https://buymeacoffee.com/cupwalk1");
}
