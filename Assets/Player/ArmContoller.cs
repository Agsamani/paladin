using UnityEngine;
using UnityEngine.InputSystem;

public class ArmContoller : MonoBehaviour
{
    private InputSystem_Actions _input;
    private InputAction _look;
    private InputAction _move;
    
    private CharacterController _controller;

    public GameObject HandTarget;
    Vector3 _handTargetBasePos;
    Vector3 _handSmoothedPos, _handVel;
    public float HandPosSmoothTime = 0.08f;   // tune ~0.06–0.15
    public float HandMaxSpeed = Mathf.Infinity; // optional clamp


    private Animator _animator;

    private string _currentAnimationState;

    [Header("Sway")]
    public float SwayStep = 0.01f;
    public float MaxSwayStep = 0.06f;
    public float SwayPosIntensity = 10.0f;
    Vector3 _swayPos;
    
    public float SwayRotStep = 0.01f;
    public float MaxSwayRotStep = 0.06f;
    public float SwayRotIntensity = 10.0f;
    Vector3 _swayRot;

    Vector3 _basePos;
    Quaternion _baseRot;

    Vector3 _swayPosVel;
    private Vector3 _smoothedSwayPos;

    [Header("Bob")]
    public float BobCurveSpeed = 6f;
    float curveSin => Mathf.Sin(_bobPhase);
    float curveCos => Mathf.Cos(_bobPhase);

    public Vector3 TravelLimit = Vector3.one * 0.025f;
    public Vector3 BobLimit = Vector3.one * 0.01f;

    Vector3 _bobPos;
    public Vector3 BobRotMult = Vector3.one;

    Vector3 _bobRot;
    float _bobPhase;

    bool _isAttacking = false;

    private PlayerInteractions _interaction;


    private void Awake()
    {
        _input = new InputSystem_Actions();
        _look = _input.Player.Look;
        _move = _input.Player.Move;

        _controller = GetComponentInParent<CharacterController>();
        _animator = GetComponent<Animator>();

        _basePos = transform.localPosition;
        _baseRot = transform.localRotation;
        _smoothedSwayPos = _basePos;

        _handTargetBasePos = HandTarget.transform.localPosition;
        _handSmoothedPos = _handTargetBasePos;
        _handVel = Vector3.zero;

        _interaction = GetComponentInParent<PlayerInteractions>();
    }

    void Start()
    {
    }

    void Update()
    {
        Sway();
        //if(!_isAttacking) Bob();
    }
    void LateUpdate()
    {
        ApplyEffects();
    }
    void Sway()
    {
        Vector3 look = _look.ReadValue<Vector2>() * -SwayStep;
        look.x = Mathf.Clamp(look.x, -MaxSwayStep, MaxSwayStep);
        look.y = Mathf.Clamp(look.y, -MaxSwayStep, MaxSwayStep);

        _swayPos = new Vector3(look.x, look.y, 0);

        look = _look.ReadValue<Vector2>() * -SwayRotStep;
        look.x = Mathf.Clamp(look.x, -MaxSwayRotStep, MaxSwayRotStep);
        look.y = Mathf.Clamp(look.y, -MaxSwayRotStep, MaxSwayRotStep);

        _swayRot = new Vector3(look.y, look.x, look.x);

    }

    void Bob()
    {
        float speed = _controller.velocity.magnitude;
        float hz = BobCurveSpeed + speed * 0.25f;
        _bobPhase += hz * 2f * Mathf.PI * Time.deltaTime;

        Vector2 move = _move.ReadValue<Vector2>();
        _bobPos.x = curveCos * BobLimit.x - move.x * TravelLimit.x;
        _bobPos.y = Mathf.Abs(curveSin) * BobLimit.y - _controller.velocity.y * TravelLimit.y;
        _bobPos.z = -move.y * TravelLimit.z;

        //if (move != Vector2.zero)
        //    _bobRot = Vector3.Scale(new Vector3(Mathf.Sin(2f * _bobPhase), curveCos, curveCos * move.x), BobRotMult);
        //else
        //    _bobRot = new Vector3(Mathf.Sin(2f * _bobPhase) * 0.5f, 0f, 0f);
    }

    void ApplyEffects()
    {
        Vector3 slowTargetPos = _basePos + _swayPos;

        _smoothedSwayPos = Vector3.SmoothDamp(
            _smoothedSwayPos,           
            slowTargetPos,
            ref _swayPosVel,
            1f / Mathf.Max(0.0001f, SwayPosIntensity)
        );

        transform.localPosition = _smoothedSwayPos;


        if(!_isAttacking)
        {
            var handTarget = _handTargetBasePos + _bobPos; // local space
            _handSmoothedPos = Vector3.SmoothDamp(
            _handSmoothedPos,
            handTarget,
            ref _handVel,
            HandPosSmoothTime,
            HandMaxSpeed,
            Time.deltaTime
        );
            HandTarget.transform.localPosition = _handSmoothedPos;

        }
        
        Quaternion slowRotTarget = _baseRot * Quaternion.Euler(_swayRot);

        Quaternion smoothedRot = Quaternion.Slerp(
            transform.localRotation,
            slowRotTarget,
            Time.deltaTime * SwayRotIntensity
        );
        transform.localRotation = smoothedRot;// * Quaternion.Euler(_bobRot);
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        ChangeAnimationState("ArmsAttack1");
        _isAttacking = true;
    }

    private void OnSwordUp()
    {

        _interaction.ActivateSword();
    }

    private void OnAttackDone()
    {
        ChangeAnimationState("ArmsIdle");
        _isAttacking = false;
        _handSmoothedPos = _handTargetBasePos;
        _handVel = Vector3.zero;
        _interaction.DeactivateSword();

    }
    private void ChangeAnimationState(string newState)
    {
        if (_currentAnimationState == newState) return;

        _currentAnimationState = newState;
        _animator.CrossFadeInFixedTime(_currentAnimationState, 0.2f);
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        _input.Disable();
        _input.Player.Attack.performed -= OnAttack;
    }
}
