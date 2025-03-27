using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
   public GoatStats GoatStats;
   public ResourceInfo ResourceInfo;
   public UpgradeStack[] UpgradeStacks;

   public GameObject UpgradePrefab;
   public GameObject MaxedOutPrefab;

   public UpgradeSlider SpeedSlider;
   public UpgradeSlider JumpSlider;
   
   public static UpgradeManager Instance;
   GameManager _gm;

   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
   }


   private void Start()
   {
      
      _gm = GameManager.Instance;
      _gm.MenuLoaded.AddListener(InstantiateStacks);
      _gm.ResetGame.AddListener(ResetGame);
      
      SpeedSlider.DisablePanel();
      SpeedSlider.Slider.onValueChanged.AddListener(VelocityChange);
      SpeedSlider.Slider.value = UnParseVelocity(GoatStats.speed);
      SpeedSlider.UpdateSlider();
      
      JumpSlider.DisablePanel();
      JumpSlider.Slider.onValueChanged.AddListener(value => JumpChange(value));
      JumpSlider.Slider.value = UnParseJump(GoatStats.jumpForce);
      JumpSlider.UpdateSlider();
      
      InstantiateStacks();
   }

   private void JumpChange(float value)
   {
      GoatStats.jumpForce = ParseJump(value);
      JumpSlider.UpdateSlider();
   }
   
   private float ParseJump(float value)
   {
      return 15 + (value-1)* (GoatStats.maxJumpForce-15) /5;
   }
   
   public float UnParseJump(float value)
   {
      return 5*(value-15)/(GoatStats.maxJumpForce-15)+1;
   }

   private void ResetGame()
   {
      SpeedSlider.DisablePanel();
      SpeedSlider.Slider.maxValue = 1;
      UpgradeManager.Instance.UpdateStacks();
   }

   private void InstantiateStacks()
   {
      foreach (var stack in UpgradeStacks)
      {
         stack.InstantiateStack();
      }
   }
   
   public void UpdateStacks()
   {
      foreach (var stack in UpgradeStacks) stack.UpdateStack();
   }
   
   private void VelocityChange(float value)
   {
      GoatStats.speed = ParseVelocity(value);
      SpeedSlider.UpdateSlider();
   }
   
   public float ParseVelocity(float value)
   {
      return 7+ (value-1)* (GoatStats.maxSpeed-7) /5;
   }
   
   public float UnParseVelocity(float value)
   {
      return 5*(value-7)/(GoatStats.maxSpeed-7)+1;
   }
}

[System.Serializable]
public class UpgradeStack
{
   public UpgradeData[] Upgrades;
   public bool IsRequiredForVictory;

   public int UpgradeLevel
   {
   
      get { return PlayerPrefs.GetInt(Stack.name); }
      set
      {
         PlayerPrefs.SetInt(Stack.name, value);
         PlayerPrefs.Save();
         UpdateStack();
      }
   }

   public GameObject Stack;
   public void UpdateStack()
   {
      for (int i = 0; i < Stack.transform.childCount; i++)
      {
         Stack.transform.GetChild(i).gameObject.SetActive(false);
      }

      if (UpgradeLevel >= Upgrades.Length)
      {
         Object.Instantiate(UpgradeManager.Instance.MaxedOutPrefab, Stack.transform);
      }
      else Stack.transform.GetChild(UpgradeLevel).gameObject.SetActive(true);
   }

   public void InstantiateStack()
   {
      if(Stack.transform.childCount != 0) return;
      for (int i = 0; i < Upgrades.Length; i++)
      {
         var g = GameObject.Instantiate(UpgradeManager.Instance.UpgradePrefab, Stack.transform);
         g.GetComponent<UpgradeScript>().UpgradeData = Upgrades[i];
         g.GetComponent<UpgradeScript>().UpgradeStack = this;
         g.SetActive(false);
         if(g.GetComponent<UpgradeScript>().UpgradeData.IsTemporary && UpgradeLevel > i)
            g.GetComponent<UpgradeScript>().UpgradeData.OnUpgrade.Invoke();
      }
      UpdateStack();
   }
}