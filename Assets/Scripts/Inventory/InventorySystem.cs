using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [Header("UI")]
    public Image[] slots = new Image[4];
    public Sprite emptySlotSprite;
    public Color emptyColor = new Color(1, 1, 1, 0.3f);
    public Color filledColor = Color.white;
    public Color selectedColor = Color.yellow;

    [Header("Selección")]
    public int selectedSlot = 0;

    [Header("Referencias")]
    public Selected playerSelected;

    private GameObject[] items = new GameObject[4];
    private int currentSlots = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            selectedSlot--;
            if (selectedSlot < 0) selectedSlot = 3;
            EquiparSlotSeleccionado();
        }
        else if (scroll < 0f)
        {
            selectedSlot++;
            if (selectedSlot > 3) selectedSlot = 0;
            EquiparSlotSeleccionado();
        }

        // Guardar con G sigue funcionando
        if (Input.GetKeyDown(KeyCode.G) && playerSelected != null)
        {
            if (playerSelected.ObjetoAgarrado != null)
            {
                InventoryItem invItem = playerSelected.ObjetoAgarrado.GetComponent<InventoryItem>();
                if (invItem != null)
                {
                    AddItem(playerSelected.ObjetoAgarrado);
                    playerSelected.LimpiarMano();
                }
            }
        }
    }

    private void EquiparSlotSeleccionado()
    {
        if (playerSelected == null) return;

        // Guarda el objeto de la mano si tiene uno
        if (playerSelected.ObjetoAgarrado != null)
        {
            GameObject actual = playerSelected.ObjetoAgarrado;
            playerSelected.LimpiarMano();
            GuardarSinSeleccionar(actual);
        }

        // Equipa el objeto del slot seleccionado
        if (items[selectedSlot] != null)
        {
            GameObject item = items[selectedSlot];
            items[selectedSlot] = null;

            currentSlots = 0;
            for (int i = 0; i < 4; i++)
                if (items[i] != null) currentSlots = i + 1;

            item.transform.position = playerSelected.transform.position;
            item.layer = LayerMask.NameToLayer("Agarrado");
            playerSelected.AgarrarDesdeInventario(item);
        }

        RefreshUI();
    }

    private void GuardarSinSeleccionar(GameObject item)
    {
        if (currentSlots >= 4) return;

        item.layer = 0;
        item.transform.position = new Vector3(0, -1000f, 0);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = item.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Busca el primer slot vacío que no sea el seleccionado
        for (int i = 0; i < 4; i++)
        {
            if (items[i] == null && i != selectedSlot)
            {
                items[i] = item;
                if (i >= currentSlots) currentSlots = i + 1;
                return;
            }
        }
    }

    public bool AddItem(GameObject item)
    {
        if (currentSlots >= 4)
        {
            Debug.Log("Inventario lleno");
            return false;
        }

        item.layer = 0;
        item.transform.position = new Vector3(0, -1000f, 0);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = item.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        items[currentSlots] = item;
        currentSlots++;

        RefreshUI();
        return true;
    }

    public bool HasItem(string tag)
    {
        for (int i = 0; i < 4; i++)
            if (items[i] != null && items[i].CompareTag(tag))
                return true;
        return false;
    }

    public bool RemoveItem(string tag)
    {
        for (int i = 0; i < 4; i++)
        {
            if (items[i] != null && items[i].CompareTag(tag))
            {
                Destroy(items[i]);
                for (int j = i; j < currentSlots - 1; j++)
                    items[j] = items[j + 1];
                items[currentSlots - 1] = null;
                currentSlots--;
                RefreshUI();
                return true;
            }
        }
        return false;
    }

    private void RefreshUI()
    {
        if (slots == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            if (items[i] != null)
            {
                InventoryItem invItem = items[i].GetComponent<InventoryItem>();
                slots[i].sprite = invItem != null && invItem.icon != null
                    ? invItem.icon
                    : emptySlotSprite;
                slots[i].color = i == selectedSlot ? selectedColor : filledColor;
            }
            else
            {
                slots[i].sprite = emptySlotSprite;
                slots[i].color = i == selectedSlot
                    ? new Color(selectedColor.r, selectedColor.g, selectedColor.b, 0.5f)
                    : emptyColor;
            }
        }
    }
}