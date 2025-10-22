using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteractions : MonoBehaviour, IDamageable
{
    private InputSystem_Actions _input;
    private InputAction _interact;

    private Sword _sword;
    public Transform SwordSlot;
    public Sword DefaultSwordPrefab;

    [SerializeField]
    private Stats _stats;


    public float InteractionRange = 1.0f;
    public LayerMask InteractMask;
    public Camera MainCamera;
    private IInteractable _interactable;


    public void EquipSword(Sword prefab)
    {
        if (_sword) Destroy(_sword.gameObject);
        _sword = Instantiate(prefab, SwordSlot);

    }

    void Awake()
    {
        _input = new InputSystem_Actions();
        _interact = _input.Player.Interact;

    }

    void Start()
    {
        if (DefaultSwordPrefab) EquipSword(DefaultSwordPrefab);
    }

    void Update()
    {
        Ray r = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
        Debug.DrawRay(MainCamera.transform.position, MainCamera.transform.forward);
        if(Physics.Raycast(r, out RaycastHit hit, InteractionRange, InteractMask))
        {
            var i = hit.collider.GetComponent<IInteractable>() ??
                hit.collider.GetComponentInParent<IInteractable>();

            if(i != null)
            {
                _interactable = i;
                i.OnRayHit(hit);
                EnableInteractableOutline();
            } else
            {
                if (_interactable != null)
                {
                    DisableInteractableOutline();
                    _interactable.OnRayExit();
                    _interactable = null;
                }
            }

        } else if (_interactable != null)
        {
            DisableInteractableOutline();
            _interactable.OnRayExit();
            _interactable = null;
        }

    }
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (_interactable == null) return;

        var info = new InteractionInfo
        {
            Interactor = this.gameObject
        };
        _interactable.Interact(info);

    }

    public void TakeDamage(DamageInfo dmg)
    {
    }

    void EnableInteractableOutline()
    {
        List<Material> mats = new List<Material>();
        foreach(var r in _interactable.ReturnSelf().GetComponentsInChildren<Renderer>())
        {
            mats.AddRange(r.materials);
        }
        foreach(var mat in mats)
        {

            mat.SetInt("_OutlineEnabled", 1);
        }

    }
    void DisableInteractableOutline()
    {   
        List<Material> mats = new List<Material>();

        foreach(var r in _interactable.ReturnSelf().GetComponentsInChildren<Renderer>())
        {
            mats.AddRange(r.materials);
        }
        foreach(var mat in mats)
        {

            mat.SetInt("_OutlineEnabled", 0);
        }


    }


    public void ActivateSword()
    {
        _sword.Activate(); 
    }
    public void DeactivateSword()
    {
        _sword.Deactivate(); 
    }


    private void OnEnable()
    {
        _input.Enable();
        _interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        _input.Disable();
        _interact.performed -= OnInteract;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawWireSphere(transform.position, InteractionRange);
    }

}
