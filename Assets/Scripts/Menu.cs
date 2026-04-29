using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Salir()
    {
        Application.Quit();
    }
}