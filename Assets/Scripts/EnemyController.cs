using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 15f;
    public float fieldOfViewAngle = 110f;
    public LayerMask obstacleMask;

    [Header("Patrol")]
    public Transform[] waypoints;
    public float waypointTolerance = 0.5f;
    public float idleTimeAtWaypoint = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    private EnemyState currentState;
    private int waypointIndex = 0;
    private float idleTimer = 0f;

    private static readonly int SpeedParam = Animator.StringToHash("Speed");

    private void Awake()
    {
        agent    = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player   = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start() => ChangeState(EnemyState.Idle);

    private void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case EnemyState.Idle:   UpdateIdle();   break;
            case EnemyState.Patrol: UpdatePatrol(); break;
            case EnemyState.Chase:  UpdateChase();  break;
        }

        animator.SetFloat(SpeedParam, agent.velocity.magnitude);

        // Debug temporal
        Debug.Log($"Estado: {currentState} | PosY: {transform.position.y:F3} | VelY: {agent.velocity.y:F3} | Animacion: {animator.GetCurrentAnimatorClipInfo(0)[0].clip.name}");
    }

    private void UpdateIdle()
    {
        if (CanSeePlayer()) { ChangeState(EnemyState.Chase); return; }

        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f && waypoints.Length > 0)
            ChangeState(EnemyState.Patrol);
    }

    private void UpdatePatrol()
    {
        if (CanSeePlayer()) { ChangeState(EnemyState.Chase); return; }

        if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            // Fuerza la posición exacta del waypoint antes de cambiar estado
            Vector3 targetPos = waypoints[waypointIndex].position;
            transform.position = new Vector3(
                transform.position.x,
                targetPos.y,           // Usa la Y del waypoint, no la del agente
                transform.position.z
            );

            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            ChangeState(EnemyState.Idle);
        }
    }

    private void UpdateChase()
    {
        if (!CanSeePlayer())
        {
            ChangeState(EnemyState.Patrol);
            return;
        }
        agent.SetDestination(player.position);
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = player.position - transform.position;
        float   distance    = dirToPlayer.magnitude;

        if (distance > detectionRange) return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > fieldOfViewAngle * 0.5f) return false;

        Vector3 eyePos = transform.position + Vector3.up * 1.6f;
        if (Physics.Raycast(eyePos, dirToPlayer.normalized, distance, obstacleMask))
            return false;

        return true;
    }

    private void ChangeState(EnemyState newState)
{
    currentState = newState;
    switch (newState)
    {
        case EnemyState.Idle:
            agent.ResetPath();      // En lugar de isStopped = true
            idleTimer = idleTimeAtWaypoint;
            break;
        case EnemyState.Patrol:
            agent.isStopped = false;
            if (waypoints.Length > 0)
                agent.SetDestination(waypoints[waypointIndex].position);
            break;
        case EnemyState.Chase:
            agent.isStopped = false;
            break;
    }
}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.cyan;
        Vector3 left  = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0,  fieldOfViewAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, left  * detectionRange);
        Gizmos.DrawRay(transform.position, right * detectionRange);
    }
}

public enum EnemyState { Idle, Patrol, Chase }