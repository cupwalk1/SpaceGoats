using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
   public GoatStats GoatStats;
   public ResourceInfo ResourceInfo;
   public UpgradeStack[] UpgradeStacks;

   public GameObject UpgradePrefab;
   public GameObject MaxedOutPrefab;


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
      InstantiateStacks();
      _gm.MenuLoaded.AddListener(InstantiateStacks);
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
         if(g.GetComponent<UpgradeScript>().UpgradeData.IsTemporary && UpgradeLevel >= i)
            g.GetComponent<UpgradeScript>().UpgradeData.OnUpgrade.Invoke();
      }
      UpdateStack();
   }
}