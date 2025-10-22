using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitBox : MonoBehaviour
{

    public LayerMask HurtMask;        
    public GameObject RootAttacker;     

    Collider _col;
    bool _active;
    WeaponStats _stats;

    HashSet<IDamageable> _damagedSet;

    public void SetStats(WeaponStats stats)
    {
        _stats = stats;
    } 
    
    public void Activate()
    {
        _active = true;
        _col.enabled = true;
    }
    public void Deactivate()
    {
        _active = false;
        _col.enabled = false;
        _damagedSet.Clear();
    }


    void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
        _damagedSet = new HashSet<IDamageable>();
        Deactivate();
    }

    private void Start()
    {
        
        if (_stats == null) Debug.LogError("Stats is not set by attacker");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_active) return;
        if (((1 << other.gameObject.layer) & HurtMask.value) == 0) return;
        if (!other.TryGetComponent<IDamageable>(out var dmgable)) return;
        if (_damagedSet.Contains(dmgable)) return;

        float dmg;
        bool isCrit;
        if (Random.value <= _stats.CritChance) {
            dmg = _stats.Damage* _stats.CritMult;
            isCrit = true;
        } else
        {
            dmg = _stats.Damage;
            isCrit = false;
        }

        DamageInfo info = new DamageInfo
        {
            Attacker = RootAttacker ? RootAttacker : gameObject,
            Amount = dmg,
            IsCrit = isCrit
        };

        dmgable.TakeDamage(info);
        _damagedSet.Add(dmgable);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
