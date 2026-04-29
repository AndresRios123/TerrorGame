using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI promptText;
    public Selected selected;

    [Header("Mensajes")]
    public float distancia = 5f;
    public LayerMask mask;

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(selected.transform.position, selected.transform.forward, out hit, distancia, ~LayerMask.GetMask("Player", "Agarrado")))
        {
            string tag = hit.collider.tag;
            string mensaje = "";

            switch (tag)
            {
                case "Pickable":
                    InventoryItem invItem = hit.collider.GetComponent<InventoryItem>();
                    mensaje = invItem != null ? "Presiona E para recoger" : "Presiona E para agarrar";
                    break;
                case "Llave":
                    mensaje = "Presiona E para agarrar la llave";
                    break;
                case "Door":
                    mensaje = "Presiona E para abrir/cerrar";
                    break;
                case "Cajon":
                    mensaje = "Presiona E para abrir/cerrar";
                    break;
                default:
                    mensaje = "";
                    break;
            }

            promptText.text = mensaje;
            promptText.gameObject.SetActive(mensaje != "");
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }
}