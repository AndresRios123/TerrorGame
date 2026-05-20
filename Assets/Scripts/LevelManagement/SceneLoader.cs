using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("UI de Carga")]
    public GameObject loadingPanel;
    public Slider progressBar;
    public TextMeshProUGUI loadingText;

    [Header("Configuración")]
    public float minDisplayTime = 1.5f;
    public string loadingMessage = "Cargando...";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StopAllCoroutines();
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Asegurar que el juego no esté pausado
        Time.timeScale = 1f;

        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        if (loadingText != null)
            loadingText.text = loadingMessage;

        if (progressBar != null)
            progressBar.value = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float timer = 0f;

        while (!operation.isDone)
        {
            // La carga real se queda en 0.9 hasta que allowSceneActivation = true
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressBar != null)
                progressBar.value = progress;

            timer += Time.unscaledDeltaTime;

            // Esperar a que la carga esté casi lista Y haya pasado el tiempo mínimo
            if (operation.progress >= 0.9f && timer >= minDisplayTime)
            {
                if (progressBar != null)
                    progressBar.value = 1f;

                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Pequeña espera extra por si la activación fue inmediata
        yield return new WaitForSecondsRealtime(0.2f);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }
}
