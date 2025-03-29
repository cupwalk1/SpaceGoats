using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void Menu()
    {
        SceneManager.LoadScene("Scenes/MenuScene");
    }
}
