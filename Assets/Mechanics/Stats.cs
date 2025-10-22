using UnityEngine;

[System.Serializable]               
public class Stats
{
    public float Health = 100;
    public float Mana = 50;
}

[System.Serializable]               
public class WeaponStats
{
    public float Damage = 10f;
    public float CritChance = 0.1f;
    public float CritMult = 2.0f;

}
