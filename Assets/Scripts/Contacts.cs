using System.Collections.Generic;
using UnityEngine;

public class Contacts
{
   
   
   public enum PlayerState
   {
      Grounded,
      Ceiling,
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
            {y: > 0}  => PlayerState.Ceiling,
            { x: > 0 } or { x: < 0 } => PlayerState.Wall,
            _ => PlayerState.Airborne
         };
      }
   }

   public bool IsGrounded => State == PlayerState.Grounded;
   public bool IsAirborne => State == PlayerState.Airborne;
   public bool IsCeilingOrAirborne => State is PlayerState.Ceiling or PlayerState.Ceiling;
   public bool IsGroundedOrAirborne => State is PlayerState.Grounded or PlayerState.Airborne;
   public bool IsWall => State == PlayerState.Wall;
   public bool IsCorner => State == PlayerState.Corner;
   
   public bool IsCausedByJump = false;
   
   
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