using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WakeUpAnimation : MonoBehaviour
{
    [Header("Párpados")]
    public RectTransform panelArriba;
    public RectTransform panelAbajo;

    [Header("Cámara")]
    public Transform cameraTransform;
    public float pitchInicial = -70f;
    public float pitchFinal = 0f;

    [Header("Tiempos")]
    public float esperaInicial = 1f;
    public float duracionParpadeo = 3f;
    public float duracionLevantarse = 4f;

    private PlayerMovement playerMovement;
    private float alturaPanel;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.animacionActiva = true;

        alturaPanel = Screen.height / 2f;

        panelArriba.sizeDelta = new Vector2(Screen.width, alturaPanel);
        panelAbajo.sizeDelta  = new Vector2(Screen.width, alturaPanel);
        panelArriba.anchoredPosition = Vector2.zero;
        panelAbajo.anchoredPosition  = Vector2.zero;

        cameraTransform.localRotation = Quaternion.Euler(pitchInicial, 0f, 0f);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(SecuenciaDespertar());
    }

    IEnumerator SecuenciaDespertar()
    {
        yield return new WaitForSeconds(esperaInicial);
        yield return StartCoroutine(AnimarParpadeos());
        yield return StartCoroutine(LevantarVista());
    }

    IEnumerator AnimarParpadeos()
    {
        yield return StartCoroutine(AbrirOjos(0.3f, 0.4f));
        yield return StartCoroutine(CerrarOjos(0.2f));
        yield return new WaitForSeconds(0.15f);

        yield return StartCoroutine(AbrirOjos(0.55f, 0.45f));
        yield return StartCoroutine(CerrarOjos(0.25f));
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(AbrirOjos(1f, duracionParpadeo * 0.5f));
    }

    IEnumerator AbrirOjos(float aperturaMax, float duracion)
    {
        float tiempo = 0f;
        float desplazamiento = alturaPanel * aperturaMax;

        Vector2 posArribaInicial = panelArriba.anchoredPosition;
        Vector2 posAbajoInicial  = panelAbajo.anchoredPosition;

        Vector2 posArribaObjetivo = new Vector2(0f,  desplazamiento);
        Vector2 posAbajoObjetivo  = new Vector2(0f, -desplazamiento);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, tiempo / duracion);

            panelArriba.anchoredPosition = Vector2.Lerp(posArribaInicial, posArribaObjetivo, t);
            panelAbajo.anchoredPosition  = Vector2.Lerp(posAbajoInicial,  posAbajoObjetivo,  t);

            yield return null;
        }
    }

    IEnumerator CerrarOjos(float duracion)
    {
        float tiempo = 0f;

        Vector2 posArribaInicial = panelArriba.anchoredPosition;
        Vector2 posAbajoInicial  = panelAbajo.anchoredPosition;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, tiempo / duracion);

            panelArriba.anchoredPosition = Vector2.Lerp(posArribaInicial, Vector2.zero, t);
            panelAbajo.anchoredPosition  = Vector2.Lerp(posAbajoInicial,  Vector2.zero, t);

            yield return null;
        }
    }

    IEnumerator LevantarVista()
    {
        float tiempo = 0f;
        float pitchActual = pitchInicial;

        while (tiempo < duracionLevantarse)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, tiempo / duracionLevantarse);
            float pitch = Mathf.Lerp(pitchActual, pitchFinal, t);
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            yield return null;
        }

        cameraTransform.localRotation = Quaternion.Euler(pitchFinal, 0f, 0f);
        playerMovement.animacionActiva = false;
    }
}