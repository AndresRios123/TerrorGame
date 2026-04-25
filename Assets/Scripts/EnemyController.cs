using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
    public int disappearAtWaypoint = -1;    // -1 = no desaparece

    [Header("Investigation")]
    public float investigateTime = 5f;
    public float investigateRadius = 4f;

    [Header("Game Over")]
    public float catchDistance = 2f;
    public float screamDuration = 2f;
    public string gameOverScene = "GameOver";
    public AudioClip screamSound;

    [Header("Movimiento por pasos")]
    public float stepDuration = 0.4f;
    public float pauseDuration = 0.3f;

    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private Transform player;

    private EnemyState currentState;
    private int waypointIndex = 0;
    private float idleTimer = 0f;
    private float investigateTimer = 0f;
    private Vector3 lastKnownPosition;
    private bool gameOverTriggered = false;

    private float stepTimer = 0f;
    private bool isStepping = false;
    private float originalSpeed;

    private static readonly int SpeedParam  = Animator.StringToHash("Speed");
    private static readonly int ScreamParam = Animator.StringToHash("Scream");

    private void Awake()
    {
        agent       = GetComponent<NavMeshAgent>();
        animator    = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player      = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        originalSpeed = agent.speed;
        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        if (player == null || gameOverTriggered) return;

        switch (currentState)
        {
            case EnemyState.Idle:        UpdateIdle();        break;
            case EnemyState.Patrol:      UpdatePatrol();      break;
            case EnemyState.Chase:       UpdateChase();       break;
            case EnemyState.Investigate: UpdateInvestigate(); break;
        }

        float animSpeed = (currentState == EnemyState.Chase || currentState == EnemyState.Patrol)
            ? 1f
            : 0f;
        animator.SetFloat(SpeedParam, animSpeed);
    }

    // ─────────────────────────────────
    //  ESTADOS
    // ─────────────────────────────────

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

        HandleStepMovement();

        if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            // ¿Es el waypoint donde debe desaparecer?
            if (disappearAtWaypoint >= 0 && waypointIndex == disappearAtWaypoint)
            {
                Destroy(gameObject);
                return;
            }

            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            ChangeState(EnemyState.Idle);
        }
    }

    private void UpdateChase()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= catchDistance)
        {
            TriggerGameOver();
            return;
        }

        if (CanSeePlayer())
        {
            lastKnownPosition = player.position;
            agent.SetDestination(player.position);
        }
        else
        {
            ChangeState(EnemyState.Investigate);
            return;
        }

        HandleStepMovement();
    }

    private void UpdateInvestigate()
    {
        if (CanSeePlayer()) { ChangeState(EnemyState.Chase); return; }

        investigateTimer -= Time.deltaTime;

        if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            if (investigateTimer > 0f)
            {
                Vector3 randomPoint = lastKnownPosition +
                    new Vector3(
                        Random.Range(-investigateRadius, investigateRadius),
                        0f,
                        Random.Range(-investigateRadius, investigateRadius)
                    );

                if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out var hit, investigateRadius, UnityEngine.AI.NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
            }
            else
            {
                ChangeState(EnemyState.Patrol);
            }
        }

        if (investigateTimer <= 0f && agent.remainingDistance <= waypointTolerance)
            ChangeState(EnemyState.Patrol);
    }

    // ─────────────────────────────────
    //  MOVIMIENTO POR PASOS
    // ─────────────────────────────────

    private void HandleStepMovement()
    {
        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            isStepping = !isStepping;

            if (isStepping)
            {
                agent.speed = originalSpeed;
                stepTimer = stepDuration;
            }
            else
            {
                agent.speed = 0f;
                stepTimer = pauseDuration;
            }
        }
    }

    // ─────────────────────────────────
    //  GAME OVER
    // ─────────────────────────────────

    private void TriggerGameOver()
    {
        gameOverTriggered = true;
        agent.isStopped = true;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        animator.SetTrigger(ScreamParam);

        var playerMovement = player.GetComponent<MonoBehaviour>();
        if (playerMovement != null) playerMovement.enabled = false;

        var gameOverCamera = FindObjectOfType<GameOverCamera>();
        if (gameOverCamera != null)
            gameOverCamera.TriggerGameOver(transform);
    }

    private void LoadGameOver()
    {
        SceneManager.LoadScene(gameOverScene);
    }

    // ─────────────────────────────────
    //  VISIÓN
    // ─────────────────────────────────

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

    // ─────────────────────────────────
    //  CAMBIO DE ESTADO
    // ─────────────────────────────────

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case EnemyState.Idle:
                agent.ResetPath();
                agent.speed = originalSpeed;
                idleTimer = idleTimeAtWaypoint;
                break;

            case EnemyState.Patrol:
                agent.isStopped = false;
                agent.speed = originalSpeed;
                stepTimer = 0f;
                isStepping = false;
                if (waypoints.Length > 0)
                    agent.SetDestination(waypoints[waypointIndex].position);
                break;

            case EnemyState.Chase:
                agent.isStopped = false;
                agent.speed = originalSpeed;
                stepTimer = 0f;
                isStepping = false;
                break;

            case EnemyState.Investigate:
                agent.isStopped = false;
                agent.speed = originalSpeed;
                investigateTimer = investigateTime;
                agent.SetDestination(lastKnownPosition);
                break;
        }
    }

    // ─────────────────────────────────
    //  GIZMOS
    // ─────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.cyan;
        Vector3 left  = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0,  fieldOfViewAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, left  * detectionRange);
        Gizmos.DrawRay(transform.position, right * detectionRange);

        if (currentState == EnemyState.Investigate)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastKnownPosition, 0.3f);
            Gizmos.DrawWireSphere(lastKnownPosition, investigateRadius);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
    }
}

public enum EnemyState { Idle, Patrol, Chase, Investigate }