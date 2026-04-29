using UnityEngine;

public class Selected : MonoBehaviour
{
    public float distancia = 5f;

    [Header("Agarre")]
    public Transform holdPoint;
    public float throwForce = 500f;

    private GameObject objetoAgarrado;
    private Rigidbody rbObjeto;
    private Collider colObjeto;
    private int layerOriginal;

    public GameObject ObjetoAgarrado => objetoAgarrado;

    void Update()
    {
        RaycastHit hit;
        LayerMask mask = ~LayerMask.GetMask("Player", "Agarrado");

        if (Physics.Raycast(transform.position, transform.forward, out hit, distancia, mask))
        {
            Debug.Log("Detectando: " + hit.collider.name + " | tag: " + hit.collider.tag);
            if (Input.GetKeyDown(KeyCode.E))
            {
                // LLAVE — siempre se agarra físicamente
                if (hit.collider.CompareTag("Llave") && objetoAgarrado == null)
                {
                    AgarrarObjeto(hit.collider.gameObject);
                }

                // PICKABLE — si tiene InventoryItem va al inventario, si no se agarra físicamente
                else if (hit.collider.CompareTag("Pickable") && objetoAgarrado == null)
                {
                    InventoryItem invItem = hit.collider.GetComponent<InventoryItem>();
                    if (invItem != null && InventorySystem.Instance != null)
                    {
                        bool agregado = InventorySystem.Instance.AddItem(hit.collider.gameObject);
                        if (!agregado)
                            Debug.Log("Inventario lleno");
                    }
                    else
                    {
                        AgarrarObjeto(hit.collider.gameObject);
                    }
                }

                // PICKABLE con inventario aunque tengas algo en la mano
                else if (hit.collider.CompareTag("Pickable") && objetoAgarrado != null)
                {
                    InventoryItem invItem = hit.collider.GetComponent<InventoryItem>();
                    if (invItem != null && InventorySystem.Instance != null)
                    {
                        bool agregado = InventorySystem.Instance.AddItem(hit.collider.gameObject);
                        if (!agregado)
                            Debug.Log("Inventario lleno");
                    }
                }

                // PUERTA
                else if (hit.collider.CompareTag("Door"))
                {
                    SystemDoor door = hit.collider.GetComponent<SystemDoor>();
                    if (door == null) door = hit.collider.GetComponentInParent<SystemDoor>();
                    if (door == null) door = hit.collider.GetComponentInChildren<SystemDoor>();
                    if (door != null) door.ChangeDoorState(this);
                }

                // CAJÓN
                else if (hit.collider.CompareTag("Cajon"))
                {
                    SystemDrawer drawer = hit.collider.GetComponentInParent<SystemDrawer>();
                    if (drawer != null) drawer.ChangeDrawerState();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && objetoAgarrado != null)
            SoltarObjeto();

        if (Input.GetMouseButtonDown(0) && objetoAgarrado != null)
            LanzarObjeto();

        // Activa la linterna si está siendo agarrada
        if (objetoAgarrado != null)
        {
            Flashlight fl = objetoAgarrado.GetComponent<Flashlight>();
            if (fl != null) fl.SetHeld(true);
        }
    }

    void LateUpdate()
    {
        if (objetoAgarrado == null) return;
        objetoAgarrado.transform.position = holdPoint.position;
        objetoAgarrado.transform.rotation = holdPoint.rotation;
    }

    void AgarrarObjeto(GameObject obj)
    {
        obj.transform.SetParent(null);
        rbObjeto = obj.GetComponent<Rigidbody>();
        colObjeto = obj.GetComponent<Collider>();

        if (rbObjeto == null)
        {
            Debug.LogWarning("Sin Rigidbody: " + obj.name);
            return;
        }

        objetoAgarrado = obj;
        rbObjeto.isKinematic = true;
        rbObjeto.useGravity = false;

        if (colObjeto != null)
            colObjeto.enabled = false;

        layerOriginal = obj.layer;
        obj.layer = LayerMask.NameToLayer("Agarrado");
    }

    public void LimpiarMano()
    {
        if (objetoAgarrado == null) return;

        Flashlight fl = objetoAgarrado.GetComponent<Flashlight>();
        if (fl != null) fl.SetHeld(false);

        if (rbObjeto != null)
        {
            rbObjeto.isKinematic = true;
            rbObjeto.useGravity = false;
        }

        if (colObjeto != null)
            colObjeto.enabled = false;

        objetoAgarrado.layer = 0;
        objetoAgarrado = null;
        rbObjeto = null;
        colObjeto = null;
    }

    public void SoltarAlInventario()
    {
        if (objetoAgarrado == null) return;

        Flashlight fl = objetoAgarrado.GetComponent<Flashlight>();
        if (fl != null) fl.SetHeld(false);

        if (rbObjeto != null)
        {
            rbObjeto.isKinematic = true;
            rbObjeto.useGravity = false;
        }

        if (colObjeto != null)
            colObjeto.enabled = false;

        objetoAgarrado.layer = 0;
        objetoAgarrado = null;
        rbObjeto = null;
        colObjeto = null;
    }

    public void AgarrarDesdeInventario(GameObject obj)
    {
        rbObjeto = obj.GetComponent<Rigidbody>();
        colObjeto = obj.GetComponent<Collider>();

        if (rbObjeto == null)
        {
            Debug.LogWarning("Sin Rigidbody en objeto del inventario: " + obj.name);
            return;
        }

        objetoAgarrado = obj;
        rbObjeto.isKinematic = true;
        rbObjeto.useGravity = false;

        if (colObjeto != null)
            colObjeto.enabled = false;

        layerOriginal = 0;
        obj.layer = LayerMask.NameToLayer("Agarrado");

        Flashlight fl = obj.GetComponent<Flashlight>();
        if (fl != null) fl.SetHeld(true);
    }

    void SoltarObjeto()
    {
        if (objetoAgarrado == null) return;

        Flashlight fl = objetoAgarrado.GetComponent<Flashlight>();
        if (fl != null) fl.SetHeld(false);

        rbObjeto.isKinematic = false;
        rbObjeto.useGravity = true;

        if (colObjeto != null)
            colObjeto.enabled = true;

        objetoAgarrado.layer = layerOriginal;
        objetoAgarrado = null;
        rbObjeto = null;
        colObjeto = null;
    }

    void LanzarObjeto()
    {
        Flashlight fl = objetoAgarrado.GetComponent<Flashlight>();
        if (fl != null) fl.SetHeld(false);

        rbObjeto.isKinematic = false;
        rbObjeto.useGravity = true;
        rbObjeto.AddForce(transform.forward * throwForce);

        if (colObjeto != null)
            colObjeto.enabled = true;

        objetoAgarrado.layer = layerOriginal;
        objetoAgarrado = null;
        rbObjeto = null;
        colObjeto = null;
    }

    public bool TenerYUsarLlave()
    {
        if (objetoAgarrado != null && objetoAgarrado.CompareTag("Llave"))
        {
            Destroy(objetoAgarrado);
            objetoAgarrado = null;
            rbObjeto = null;
            colObjeto = null;
            return true;
        }
        return false;
    }
}