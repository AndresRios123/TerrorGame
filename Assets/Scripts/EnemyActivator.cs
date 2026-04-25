using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    public EnemyController enemy;  // Arrastra el enemigo aquí

    private void Start()
    {
        // El enemigo empieza desactivado
        enemy.enabled = false;
        enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.enabled = true;
            enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;

            // Ya no necesitamos el trigger
            gameObject.SetActive(false);
        }
    }
}