using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pullWeapon;

    [Header("Idle data")]
    public float idleTime;
    public float aggressiveRange;

    [Header("Move data")]
    public float moveSpeed;
    public float chaseSpeed;
    public float turnSpeed;
    private bool manualMovement;
    private bool manualRotation;

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

    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;

    public bool ManualMovementActive() => manualMovement;

    public void ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;

    public bool ManualRotationActive() => manualRotation;

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

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggressiveRange);
        Gizmos.color = Color.yellow;


    }
    public void PullWeapon()
    {
        hiddenWeapon.gameObject.SetActive(false);
        pullWeapon.gameObject.SetActive(true);
    }
    public void HiddenWeapon()
    {
        hiddenWeapon.gameObject.SetActive(true);
        pullWeapon.gameObject.SetActive(false);
    }

}
