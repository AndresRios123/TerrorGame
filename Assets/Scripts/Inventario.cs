using UnityEngine;

public class Inventario : MonoBehaviour
{
    public static Inventario instancia;

    [Header("UI")]
    public GameObject iconoLlave;

    private bool tieneLlave = false;

    void Awake()
    {
        instancia = this;

        // Asegura que el ícono esté oculto desde el primer frame
        if (iconoLlave != null)
            iconoLlave.SetActive(false);
    }

    public void AgregarLlave()
    {
        tieneLlave = true;

        if (iconoLlave != null)
            iconoLlave.SetActive(true);
    }

    public bool TieneLlave()
    {
        return tieneLlave;
    }

    public void UsarLlave()
    {
        tieneLlave = false;

        if (iconoLlave != null)
            iconoLlave.SetActive(false);
    }
}