using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Destino")]
    public string sceneToLoad;

    [Header("Opciones")]
    public bool requireKeyPress = false;
    public string keyPrompt = "Presiona E para continuar";

    private bool playerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (requireKeyPress)
        {
            playerInside = true;
        }
        else
        {
            LoadNextScene();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
    }

    void Update()
    {
        if (playerInside && requireKeyPress && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("SceneTransitionTrigger: No se ha asignado ninguna escena.");
            return;
        }

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("SceneTransitionTrigger: SceneLoader no encontrado. Usando carga directa.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}
