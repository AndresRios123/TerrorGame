using UnityEngine;

public class SystemDoor : MonoBehaviour
{
    public bool doorOpen = false;
    public float doorOpenAngle = 95f;
    public float smooth = 3f;

    public float tiempoParaCerrar = 3f;
    private float timer = 0f;

    private Collider col;

    public AudioClip openDoor;
    public AudioClip closeDoor;

    private Quaternion rotacionInicial;
    private Quaternion rotacionAbierta;

    void Start()
    {
        col = GetComponentInChildren<Collider>();

        // 🔥 Guardar rotación inicial (la que tú pusiste en Unity)
        rotacionInicial = transform.localRotation;

        // 🔥 Calcular rotación abierta basada en la inicial
        rotacionAbierta = rotacionInicial * Quaternion.Euler(0, doorOpenAngle, 0);
    }

    public void ChangeDoorState()
    {
        doorOpen = true;
        timer = tiempoParaCerrar;

        if (col != null)
            col.enabled = false;

        // 🔊 Sonido al abrir
        if (openDoor != null)
            AudioSource.PlayClipAtPoint(openDoor, transform.position, 1);
    }

    void Update()
    {
        Quaternion targetRotation;

        if (doorOpen)
        {
            targetRotation = rotacionAbierta;

            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                doorOpen = false;

                if (col != null)
                    col.enabled = true;

                // 🔊 Sonido al cerrar
                if (closeDoor != null)
                    AudioSource.PlayClipAtPoint(closeDoor, transform.position, 1);
            }
        }
        else
        {
            targetRotation = rotacionInicial;
        }

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            smooth * Time.deltaTime
        );
    }
}