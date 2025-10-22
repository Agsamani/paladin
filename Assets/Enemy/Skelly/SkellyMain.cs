using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;


public class SkellyMain : MonoBehaviour, IDamageable
{
    private NavMeshAgent _navMeshAgent;
    private Animator _AnimCtrl;
    private HitBox _hitBox;

    public Stats _stats;
    public WeaponStats _weaponStats;

    public float DeathTime = 3.0f;
    public AnimationCurve DeathCurve;

    public UndeadSoul UndeadSoulPrefab;

    private bool _dead = false;

    private ObjectPool<SkellyMain> _pool;
    public void SetPool(ObjectPool<SkellyMain> pool)
    {
        _pool = pool;
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _AnimCtrl = GetComponent<Animator>();
        _hitBox = GetComponentInChildren<HitBox>();
        _hitBox.SetStats(_weaponStats);
    }

    void Start()
    {
        SetRigidBodies(true);
        SetColliders(false);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(DamageInfo dmg)
    {
        _stats.Health -= dmg.Amount;
        Debug.Log(_stats.Health);
        if(_stats.Health <= 0)
        {
            OnDie();
        }
    }


    void OnDie()
    {
        StartCoroutine(ReleaseAfterSeconds(DeathTime + 0.1f));
        StartCoroutine(Dissolve());
        _AnimCtrl.enabled = false;
        _navMeshAgent.enabled = false;
        SetRigidBodies(false);
        SetColliders(true);
        _dead = true;
        GetComponent<SkellyAI>().enabled = false;
        SpawnSoul();
    }
    private IEnumerator Dissolve()
    {
        float time = 0f;

        List<Material> mats = new List<Material>();
        foreach(var r in GetComponentsInChildren<Renderer>())
        {
            mats.Add(r.material);
        }

        while (time < DeathTime)
        {
            time += Time.deltaTime;
            float u = Mathf.Clamp01(time / DeathTime);
            float a = DeathCurve.Evaluate(u); 
            foreach(var mat in mats)
            {

                mat.SetFloat("_DissolveAmount", a);
            }
            yield return null;
        }

    }


    IEnumerator ReleaseAfterSeconds(float t)
    {
        yield return new WaitForSeconds(t);
        _pool.Release(this);
    }


    void SpawnSoul()
    {
        Instantiate(UndeadSoulPrefab, transform.position, transform.rotation);

    }

    void SetRigidBodies(bool state)
    {
        Rigidbody[] bs = GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody b in bs)
        {
            b.isKinematic = state;
        }
    }
    void SetColliders(bool state)
    {
        Collider[] cs = GetComponentsInChildren<Collider>();
        foreach(Collider c in cs)
        {
            c.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;
    }
        
    private void OnEnable()
    {
        _stats = new Stats();
        _weaponStats = new WeaponStats();
        _AnimCtrl.enabled = true;
        _navMeshAgent.enabled = true;
        SetRigidBodies(true);
        SetColliders(false);
        _dead = true;
        GetComponent<SkellyAI>().enabled = true;

        _dead = false;
    }
 
    public bool IsDead() { return _dead; }
        

}
