using Unity.VisualScripting;
using UnityEngine;

public class IKSolver : MonoBehaviour
{
    public LayerMask GroundMask;

    public Transform Torso;
    public Transform LockPos;
    public Transform ForwardRef;
    public float ForwardOffset = 0.3f;

    public float StepDistance = 1.0f;


    private Vector3 _oldPos = Vector3.zero;
    private float _lerp = 1.0f;

    public float WalkSpeed = 3.0f;
    public float StepHeight = 0.3f;

    public bool IsGrounded { get; private set; }
    public IKSolver OtherSolver;

    void Start()
    {
        Ray ray = new Ray(Torso.position, Vector3.down); 
        if(Physics.Raycast(ray, out RaycastHit info, 2.0f, GroundMask.value))
        {
            _oldPos = info.point;
            transform.position = _oldPos;
        }
    }

    void Update()
    {
        Ray ray = new Ray(Torso.position + ForwardRef.forward * ForwardOffset, Vector3.down); 
        if(OtherSolver.IsGrounded && Physics.Raycast(ray, out RaycastHit info, 2.0f, GroundMask.value))
        {
            if(Vector3.Distance(info.point, LockPos.position) > StepDistance)
            {
                if(_lerp >= 1.0) _lerp = 0;
                IsGrounded = false;
                LockPos.position = info.point;
            }
        }
        if(_lerp < 1)
        {
            Vector3 pos = Vector3.Lerp(_oldPos, LockPos.position, _lerp);
            pos.y += Mathf.Sin(Mathf.PI * _lerp) * StepHeight;

            transform.position = pos;
            _lerp += Time.deltaTime * WalkSpeed;

        } else
        {
            _oldPos = LockPos.position;
            IsGrounded = true;
        }
        Vector3 flat = new Vector3(ForwardRef.forward.x, 0f,ForwardRef.forward.z);
        if (flat.sqrMagnitude > 1e-6f)
            transform.rotation = Quaternion.LookRotation(flat, Vector3.up);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkOrchid;
        Gizmos.DrawCube(LockPos.position, Vector3.one * 0.08f);
    }
}
