using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public int MaxHearts = 4;
    
    public UnityEvent OnTakeDamage = new UnityEvent();
    public UnityEvent OnPlayerDie = new UnityEvent();
    
    [SerializeField] private bool _shouldMove;
    public bool ShouldMove
    {
        get => _shouldMove;
        set => _shouldMove = value;
    }

    [SerializeField] private int _health;
    public int Health
    {
        get => _health;
        set
        { 
            _health = Mathf.Clamp(value, 0, MaxHearts);
        }
    }
}

