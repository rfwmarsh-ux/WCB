using UnityEngine;

public class CricketBat : MonoBehaviour
{
    public static CricketBat Instance { get; private set; }

    public string Name => "Cricket Bat";
    public float Damage => 12f;
    public float Range => 1.8f;
    public float AttackCooldown => 0.6f;
    public bool IsMelee => true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}