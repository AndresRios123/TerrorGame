using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("MenuNiveles");
    }

    public void Salir()
    {
        Application.Quit();
    }
}