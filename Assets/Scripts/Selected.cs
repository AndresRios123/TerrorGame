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
            if (hit.collider.CompareTag("Llave") && objetoAgarrado == null)
                if (Input.GetKeyDown(KeyCode.E))
                    AgarrarObjeto(hit.collider.gameObject);

            if (hit.collider.CompareTag("Pickable") && objetoAgarrado == null)
                if (Input.GetKeyDown(KeyCode.E))
                    AgarrarObjeto(hit.collider.gameObject);

            if (hit.collider.CompareTag("Door") && Input.GetKeyDown(KeyCode.E))
            {
                SystemDoor door = hit.collider.GetComponent<SystemDoor>();
                if (door == null) door = hit.collider.GetComponentInParent<SystemDoor>();
                if (door == null) door = hit.collider.GetComponentInChildren<SystemDoor>();
                if (door != null) door.ChangeDoorState(this);
            }

            if (hit.collider.CompareTag("Cajon") && Input.GetKeyDown(KeyCode.E))
            {
                SystemDrawer drawer = hit.collider.GetComponentInParent<SystemDrawer>();
                if (drawer != null) drawer.ChangeDrawerState();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && objetoAgarrado != null)
            SoltarObjeto();

        if (Input.GetMouseButtonDown(0) && objetoAgarrado != null)
            LanzarObjeto();

        // Guardar objeto agarrado en el inventario con G
        if (Input.GetKeyDown(KeyCode.G) && objetoAgarrado != null)
        {
            InventoryItem invItem = objetoAgarrado.GetComponent<InventoryItem>();
            if (invItem != null && InventorySystem.Instance != null)
            {
                bool agregado = InventorySystem.Instance.AddItem(objetoAgarrado);
                if (agregado)
                {
                    objetoAgarrado = null;
                    rbObjeto = null;
                    colObjeto = null;
                }
            }
        }

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

    public void SoltarAlInventario()
    {
        if (objetoAgarrado == null) return;

        Flashlight fl = objetoAgarrado.GetComponent<Flashlight>();
        if (fl != null) fl.SetHeld(false);

        rbObjeto.isKinematic = true;
        rbObjeto.useGravity = false;

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