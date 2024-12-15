using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    List<float> playerSpeeds = new List<float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSpeeds.Add(13); //playerSpeeds[0]
        playerSpeeds.Add(15); //playerSpeeds[1]
        playerSpeeds.Add(17); //playerSpeeds[2]
        playerSpeeds.Add(19); //playerSpeeds[3]
        playerSpeeds.Add(21); //playerSpeeds[4]
        playerSpeeds.Add(23); //playerSpeeds[5]
        playerSpeeds.Add(25); //playerSpeeds[6]
        playerSpeeds.Add(27); //playerSpeeds[7]
        playerSpeeds.Add(29); //playerSpeeds[8]
        
        for (int i = 0; i < playerSpeeds.Count; i++)
        {
            if (playerSpeeds[i] > 20)
            {
                playerSpeeds.RemoveAt(i);
                i--;
            }
        }
        
        int i = 1;
        foreach (float f in playerSpeeds)
        {
            
            if (f > 20)
            {
                continue;
            }
            Debug.Log("Player " + i + " speed: " + f);
            i ++;

        }
        
    }
   
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
