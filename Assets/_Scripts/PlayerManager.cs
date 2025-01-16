using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static bool _shouldMove;
    public static bool ShouldMove
    {
        get => _shouldMove;
        set => _shouldMove = value;
    }

    private static float _health;
    public static float Health
    {
        get => _health;
        set => _health = Mathf.Clamp(value, 0, 1);
    }
}
