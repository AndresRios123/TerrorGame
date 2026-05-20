using UnityEngine;

public class PostProcessController : MonoBehaviour
{
    public static PostProcessController Instance;

    [Header("Material de Blur")]
    [Tooltip("Arrastra aqui el material TetricBlurMaterial")]
    public Material blurMaterial;

    [Header("Intensidades")]
    [Tooltip("Blur normal durante el juego"), Range(0f, 1f)]
    public float normalIntensity = 0.2f;

    [Tooltip("Blur cuando el enemigo te persigue"), Range(0f, 1f)]
    public float chaseIntensity = 0.6f;

    [Tooltip("Velocidad de transicion entre intensidades")]
    public float transitionSpeed = 2f;

    private float targetIntensity;
    private float currentIntensity;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        targetIntensity = normalIntensity;
        currentIntensity = normalIntensity;
    }

    void Update()
    {
        currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, transitionSpeed * Time.deltaTime);

        if (blurMaterial != null)
            blurMaterial.SetFloat("_Intensity", currentIntensity);
    }

    public void SetNormal()
    {
        targetIntensity = normalIntensity;
    }

    public void SetChase()
    {
        targetIntensity = chaseIntensity;
    }

    public void SetIntensity(float intensity)
    {
        targetIntensity = Mathf.Clamp01(intensity);
    }
}
