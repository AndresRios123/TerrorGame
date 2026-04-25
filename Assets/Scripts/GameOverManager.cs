using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public string mainScene = "SampleScene"; // Nombre de tu escena principal

    public void Reintentar()
    {
        SceneManager.LoadScene(mainScene);
    }
}