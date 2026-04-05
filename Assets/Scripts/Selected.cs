using UnityEngine;

public class Selected : MonoBehaviour
{
    public float distancia = 3f;

    void Update()
    {
        RaycastHit hit;

        // Ignorar la layer "Player"
        LayerMask mask = ~LayerMask.GetMask("Player");

        // Raycast hacia adelante
        if (Physics.Raycast(transform.position, transform.forward, out hit, distancia, mask))
        {
            Debug.Log("Detectando: " + hit.collider.name);

            if (hit.collider.CompareTag("Door"))
            {
                Debug.Log("Es una puerta");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Presionaste E");

                    // Buscar el script incluso en el padre
                    SystemDoor door = hit.collider.GetComponentInParent<SystemDoor>();

                    if (door != null)
                    {
                        door.ChangeDoorState();
                    }
                    else
                    {
                        Debug.Log("No tiene SystemDoor");
                    }
                }
            }
        }
    }
}