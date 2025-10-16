using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float turnSpeed;

    public float aggressiveRange;

    [Header("Idle data")]
    public float idleTime;

    [Header("Move data")]
    public float moveSpeed;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }


    protected virtual void Start()
    {
        InitializePatrolPoints();
    }


    protected virtual void Update()
    {

    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public bool PlayerInAggresionRange() => Vector3.Distance(transform.position, player.position) < aggressiveRange;

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;

        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }


    private void InitializePatrolPoints()
    {
        foreach (var t in patrolPoints)
        {
            t.parent = null;

        }
    }

    public void RotateFace(Vector3 target)
    {
        Vector3 moveDir = (target - transform.position).normalized;
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, aggressiveRange);
    }


}
