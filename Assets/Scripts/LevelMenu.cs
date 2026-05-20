using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    // Volver al menú principal
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("SceneMenu");
    }

    // Cargar niveles
    public void LoadLevel1()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Nivel2");
    }

    public void LoadLevel4()
    {
        SceneManager.LoadScene("Nivel4");
    }

    public void LoadLevel5()
    {
        SceneManager.LoadScene("Nivel5");
    }
}