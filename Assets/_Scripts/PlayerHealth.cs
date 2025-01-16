using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private List<GameObject> obstacles= new();
    [SerializeField] float oxygenLevel;
    [SerializeField] float health;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
