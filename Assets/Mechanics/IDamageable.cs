using UnityEngine;

public struct DamageInfo
{
    public GameObject Attacker;
    public float Amount;
    public bool IsCrit;
}

public interface IDamageable
{
    void TakeDamage(DamageInfo dmg);
}
