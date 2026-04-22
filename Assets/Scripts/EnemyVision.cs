using UnityEngine;
using UnityEngine.AI;

public class EnemyVision : MonoBehaviour
{
    public Transform player;
    public float chaseDistance = 8f;
    public float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.stoppingDistance = 1.2f;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            RotateTowardsMovement();

            if (animator != null)
                animator.speed = 1f;
        }
        else
        {
            agent.isStopped = true;

            if (animator != null)
                animator.speed = 0f;
        }
    }

    void RotateTowardsMovement()
    {
        Vector3 direction = agent.velocity;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}