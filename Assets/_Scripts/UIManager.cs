using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
   // Start is called once before the first execution of Update after the MonoBehaviour is created

   public void LoadGame(string level)
   {
      SceneManager.LoadScene(level);
   }

   // Update is called once per frame
   void Update()
   {
      
   }
}