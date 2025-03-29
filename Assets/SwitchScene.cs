using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button button;
    public void Menu()
    {
        button.interactable = false;
        SceneManager.LoadScene("Scenes/MenuScene");
    }
}
