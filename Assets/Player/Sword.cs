using UnityEngine;

public class Sword : MonoBehaviour
{
    public WeaponStats _stats;
    private HitBox _hitBox;


    public void Activate()
    {
        _hitBox.Activate();
    }
    public void Deactivate()
    {
        _hitBox.Deactivate();
    }


    private void Awake()
    {
        _hitBox = GetComponentInChildren<HitBox>();
        _hitBox.SetStats(_stats);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
