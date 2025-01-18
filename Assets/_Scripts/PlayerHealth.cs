using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    private PlayerManager PlayerManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created;
    [SerializeField] private List<GameObject> obstacles= new();
    [SerializeField] float oxygenLevel;
    int health {get { return PlayerManager.Health;} set{PlayerManager.Health = value;}}
    [SerializeField] int spikeDamage = 1;
    void Start()
    {
        PlayerManager = GetComponent<PlayerManager>();
        health = PlayerManager.MaxHearts;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (true) 
        {
            health -= spikeDamage;
            PlayerManager.OnTakeDamage.Invoke();
        }
        else
        {
            health = 0;
            PlayerManager.OnPlayerDie.Invoke();
        }
    }
}
