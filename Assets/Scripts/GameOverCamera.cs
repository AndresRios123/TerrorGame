using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverCamera : MonoBehaviour
{
    [Header("Referencias")]
    public Transform enemy;              // Arrastra el enemigo aquí
    public Camera mainCamera;            // La cámara principal de la escena
    public AudioClip screamSound;        // El mismo audio del scream

    [Header("Configuración")]
    public float zoomDuration = 1f;      // Cuánto tarda en acercarse
    public float screamDuration = 2.5f;  // Cuánto dura el scream antes del Game Over
    public string gameOverScene = "GameOver";

    // Posición y rotación final de la cámara frente al monstruo
    public Vector3 cameraOffset = new Vector3(0f, 1.6f, 2f); // Frente al monstruo

    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void TriggerGameOver(Transform enemyTransform)
    {
        enemy = enemyTransform;
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // Guarda la posición original de la cámara
        originalCameraPos = mainCamera.transform.position;
        originalCameraRot = mainCamera.transform.rotation;

        // Calcula la posición final frente al monstruo
        // La cámara queda DETRÁS del monstruo mirando su cara
        Vector3 targetPos = enemy.position 
                          + enemy.forward  * cameraOffset.z 
                          + Vector3.up     * cameraOffset.y;
        Quaternion targetRot = Quaternion.LookRotation(enemy.position + Vector3.up * 1.6f - targetPos);

        // Mueve la cámara suavemente hacia el monstruo
        float elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / zoomDuration);

            mainCamera.transform.position = Vector3.Lerp(originalCameraPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(originalCameraRot, targetRot, t);

            yield return null;
        }

        // Reproduce el scream
        if (screamSound != null)
            audioSource.PlayOneShot(screamSound);

        // Espera que termine el scream
        yield return new WaitForSeconds(screamDuration);

        // Carga Game Over
        SceneManager.LoadScene(gameOverScene);
    }
}