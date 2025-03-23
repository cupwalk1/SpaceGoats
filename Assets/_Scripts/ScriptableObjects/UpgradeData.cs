
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]

public struct ResourceCost {
   public int materials;   // If you have “Materiali”
   public int fruits;      // If “banane” or any fruit resource
   public float power;     // in kW
}

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "ScriptableObjects/UpgradeData")]
public class UpgradeData : ScriptableObject {
   [Header("Display Info")]
   public string Name;       // e.g. "Pasti Salutari"
   [TextArea] public string Description; // e.g. "Aggiunge +1 ..."
   
   public bool IsTemporary; 
   
   public ResourceCost Cost; 
   
   public UnityEvent OnUpgrade;
   
}
