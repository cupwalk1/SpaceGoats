using System.Collections.Generic;
using UnityEngine;

public class Contacts
{
   
   
   public enum PlayerState
   {
      Grounded,
      Airborne,
      Wall,
      Corner
   }

   public PlayerState State
   {
      get
      {
         return contacts switch
         {
            { x: 1, y: -1 } or { x: -1, y: -1 } => PlayerState.Corner,
            { y: < 0 } => PlayerState.Grounded,
            { x: > 0 } or { x: < 0 } => PlayerState.Wall,
            _ => PlayerState.Airborne
         };
      }
   }

   public Vector2 contacts;

   public float x
   {
      get { return contacts.x; }
      set { contacts.x = value; }
   }

   public float y
   {
      get { return contacts.y; }
      set { contacts.y = value; }
   }
   
   
   
   
   
   
}