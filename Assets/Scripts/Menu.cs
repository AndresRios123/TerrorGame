using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Jugar()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene("SampleScene");
        else
            SceneManager.LoadScene("SampleScene");
    }

    public void Salir()
    {
        Application.Quit();
    }
}