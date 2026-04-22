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

    [Header("Llave")]
    public bool requiereLlave = false;
    private bool abiertaConLlave = false;

    [Header("Bloqueada")]
    public AudioClip bloqueada;

    private Quaternion rotacionInicial;
    private Quaternion rotacionAbierta;

    void Start()
    {
        col = GetComponentInChildren<Collider>();
        rotacionInicial = transform.localRotation;
        rotacionAbierta = rotacionInicial * Quaternion.Euler(0, doorOpenAngle, 0);
    }

    public void ChangeDoorState(Selected player)
    {
        if (requiereLlave)
        {
            if (!player.TenerYUsarLlave())
            {
                // 🔊 Sonido de puerta bloqueada
                if (bloqueada != null)
                    AudioSource.PlayClipAtPoint(bloqueada, transform.position, 1);

                Debug.Log("Necesitas la llave");
                return;
            }

            abiertaConLlave = true;
        }

        doorOpen = true;
        timer = tiempoParaCerrar;

        if (col != null)
            col.enabled = false;

        if (openDoor != null)
            AudioSource.PlayClipAtPoint(openDoor, transform.position, 1);
    }

    void Update()
    {
        Quaternion targetRotation;

        if (doorOpen)
        {
            targetRotation = rotacionAbierta;

            if (!abiertaConLlave)
            {
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    doorOpen = false;

                    if (col != null)
                        col.enabled = true;

                    if (closeDoor != null)
                        AudioSource.PlayClipAtPoint(closeDoor, transform.position, 1);
                }
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