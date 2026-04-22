using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMoveToPoint : MonoBehaviour
{
    public Transform destination;

    [Header("Movimiento zombie")]
    public float moveTime = 0.35f;   // tiempo que avanza
    public float stopTime = 0.45f;   // tiempo que se queda quieto

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (destination == null)
        {
            Debug.LogError("No asignaste el PuntoDestino");
            return;
        }

        StartCoroutine(ZombieWalk());
    }

    IEnumerator ZombieWalk()
    {
        while (true)
        {
            if (destination == null) yield break;

            float distance = Vector3.Distance(transform.position, destination.position);

            if (distance <= agent.stoppingDistance + 0.1f)
            {
                agent.isStopped = true;
                yield break;
            }

            agent.isStopped = false;
            agent.SetDestination(destination.position);
            yield return new WaitForSeconds(moveTime);

            agent.isStopped = true;
            yield return new WaitForSeconds(stopTime);
        }
    }
}