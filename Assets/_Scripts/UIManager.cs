using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
   // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 _initialPositionSettings;
    private Vector3 _initialPositionCredits;
   [SerializeField] private GameObject SettingsGear;
   [SerializeField] private GameObject SettingsPanel;
   [SerializeField] private GameObject SettingsMenu;
   [SerializeField] private GameObject CreditsPanel;
   [SerializeField] private GameObject CreditsMenu;
   [SerializeField] private Slider volumeSlider;
   public void LoadGame(string level)
   {
      SceneManager.LoadScene(level);
   }

  
    // public void SetLocale(string locale)
    // {
    //     LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(locale);
    //     PlayerPrefs.SetString("locale", locale);
    //     PlayerPrefs.Save();
    // }
    
    public void Start()
    {
        volumeSlider.onValueChanged.AddListener((float value) => PlayerPrefs.SetFloat("volume", value));
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        // SetLocale(PlayerPrefs.GetString("locale"));
        // if (PlayerPrefs.GetString("locale") == null)
        // {
        //     SetLocale("en");
        // }
        if (PlayerPrefs.GetFloat("volume") == null)
        {
            PlayerPrefs.SetFloat("volume", 1f);
        }
        if (PlayerPrefs.GetInt("vibration") == null)
        {
            PlayerPrefs.SetInt("vibration", 1);
        }
        PlayerPrefs.Save();
        SettingsPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        _initialPositionSettings = SettingsMenu.transform.position;
        _initialPositionCredits = CreditsMenu.transform.position;
    }
    
    // public void OnTutorialClick()
    // {
    //     SoundManager.Instance.PlaySFX(SoundManager.Instance.click);
    //     GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
    //     gameController.StartGame(Game.GameType.Tutorial);
    //      
    // }

    public void OnSettingsOpen()
    {
        SettingsPanel.SetActive(true);
        SettingsMenu.transform.LeanMove(Vector3.zero, 1f).setEaseOutElastic().period = 1.2f;
    }
    
    public void OnSettingsClose()
    {
        PlayerPrefs.Save();
        SettingsMenu.transform.LeanMove(_initialPositionSettings, 1f).setEaseInQuad().setOnComplete(() => SettingsPanel.SetActive(false));
    }
    
    public void OnCreditsOpen()
    {
        OnSettingsClose();
        CreditsPanel.SetActive(true);
        CreditsMenu.transform.LeanMove(Vector3.zero, 1f).setEaseOutElastic().period = 1.2f;
    }
    
    public void OnCreditsClose()
    {
        OnSettingsOpen();
        PlayerPrefs.Save();
        CreditsMenu.transform.LeanMove(_initialPositionCredits, 1f).setEaseOutQuad().setOnComplete(() => CreditsPanel.SetActive(false));
    }

    public void OnDonateClick()
    {
        Application.OpenURL("https://buymeacoffee.com/cupwalk1");
    }

}