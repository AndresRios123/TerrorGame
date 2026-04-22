using UnityEngine;

public class SystemDrawer : MonoBehaviour
{
    public bool drawerOpen = false;

    public float openDistance = 0.5f; // Qué tanto se abre el cajón
    public float smooth = 3f;

    public float tiempoParaCerrar = 3f;
    private float timer = 0f;

    public AudioClip openDrawer;
    public AudioClip closeDrawer;

    private Vector3 posicionInicial;
    private Vector3 posicionAbierta;

    void Start()
    {
        // 🔥 Guardar posición inicial
        posicionInicial = transform.localPosition;

        // 🔥 Calcular posición abierta (ajusta el eje según tu modelo)
        posicionAbierta = posicionInicial + new Vector3(0, 0, openDistance);
        // Si se abre hacia otro lado, cambia esto:
        // new Vector3(openDistance, 0, 0);
    }

    public void ChangeDrawerState()
    {
        drawerOpen = true;
        timer = tiempoParaCerrar;

        // 🔊 Sonido al abrir
        if (openDrawer != null)
            AudioSource.PlayClipAtPoint(openDrawer, transform.position, 1);
    }

    void Update()
    {
        Vector3 targetPosition;

        if (drawerOpen)
        {
            targetPosition = posicionAbierta;

            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                drawerOpen = false;

                // 🔊 Sonido al cerrar
                if (closeDrawer != null)
                    AudioSource.PlayClipAtPoint(closeDrawer, transform.position, 1);
            }
        }
        else
        {
            targetPosition = posicionInicial;
        }

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            smooth * Time.deltaTime
        );
    }
}