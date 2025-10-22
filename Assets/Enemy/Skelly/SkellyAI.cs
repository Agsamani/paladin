using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class SkellyAI : MonoBehaviour
{
    public float LookRadius = 10.0f;

    private NavMeshAgent _navMeshAgent;
    private Animator _AnimCtrl;

    private string _currentAnim = "";
    private HitBox _hitBox;


    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _AnimCtrl = GetComponent<Animator>();
        _AnimCtrl.applyRootMotion = true;
        _navMeshAgent.updatePosition = false;
        //_navMeshAgent.updateRotation = false;
        _hitBox = GetComponentInChildren<HitBox>();

    }


    void Start()
    {
        ChangeAnimation("skelly idle", 0.0f);
        
    }

    void Update()
    {

        GameObject Target = PlayerManager.instance.Player;
        float dist = Vector3.Distance(Target.transform.position, transform.position);

        if(dist <= LookRadius)
        {
            _navMeshAgent.SetDestination(Target.transform.position);
            FaceTarget();

            if(dist <= _navMeshAgent.stoppingDistance)
            {
                ChangeAnimation("skelly attack");
                _hitBox.Activate();
            } else
            {
                ChangeAnimation("skelly walk");
                _hitBox.Deactivate();
            }
        } else
        {
            ChangeAnimation("skelly idle");
            _hitBox.Deactivate();
        }

    }

    private void OnAnimatorMove()
    {

        Vector3 delta = _AnimCtrl.deltaPosition;
        //delta.y = 0.0f;
        transform.position += delta;

            Quaternion rot = _AnimCtrl.deltaRotation;
            transform.rotation *= rot;

        Vector3 temp = transform.localPosition; // todo:change
        temp.y = 0;
        transform.localPosition = temp;

        //_navMeshAgent.Warp(transform.position);
        _navMeshAgent.nextPosition = transform.position;
            

    }

    void ChangeAnimation(string state, float crossFade = 0.3f)
    {
        if(_currentAnim != state)
        {
            _AnimCtrl.CrossFade(state, crossFade);
            _currentAnim = state;
        }
    }

    void FaceTarget()
    {
        if(_currentAnim != "skelly attack")
        {

        GameObject Target = PlayerManager.instance.Player;
        Vector3 dir = (Target.transform.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * (_navMeshAgent.angularSpeed * Mathf.PI / 180.0f));
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.coral;
        Gizmos.DrawWireSphere(transform.position, LookRadius);
    }

    private void OnEnable()
    {
        ChangeAnimation("skelly idle", 0.0f);
    }

    private void OnDisable()
    {
        
    }
}
