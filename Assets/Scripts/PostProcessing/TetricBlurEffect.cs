using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TetricBlurEffect : MonoBehaviour
{
    [Header("Material de Blur")]
    [Tooltip("Arrastra aqui el TetricBlurMaterial")]
    public Material blurMaterial;

    [Header("Intensidad")]
    [Tooltip("0 = sin blur, 1 = muy borroso"), Range(0f, 1f)]
    public float intensity = 0.3f;

    [Header("Activar/Desactivar")]
    [Tooltip("Desmarca para desactivar el efecto")]
    public bool enable = true;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!enable || blurMaterial == null || intensity <= 0f)
        {
            Graphics.Blit(source, destination);
            return;
        }

        blurMaterial.SetFloat("_Intensity", intensity);
        Graphics.Blit(source, destination, blurMaterial);
    }
}
