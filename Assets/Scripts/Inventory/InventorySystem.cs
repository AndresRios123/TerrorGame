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
        Instance = this;
    }

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        // Debug — presiona T para ver el estado del array
        if (Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < 4; i++)
                Debug.Log($"Slot {i}: {(items[i] != null ? items[i].name : "vacío")}");
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            selectedSlot--;
            if (selectedSlot < 0) selectedSlot = 3;
            RefreshUI();
        }
        else if (scroll < 0f)
        {
            selectedSlot++;
            if (selectedSlot > 3) selectedSlot = 0;
            RefreshUI();
        }

        if (Input.GetKeyDown(KeyCode.R) && playerSelected != null)
        {
            Debug.Log($"R presionada | selectedSlot: {selectedSlot} | item en slot: {(items[selectedSlot] != null ? items[selectedSlot].name : "vacío")} | ObjetoAgarrado: {(playerSelected.ObjetoAgarrado != null ? playerSelected.ObjetoAgarrado.name : "ninguno")}");

            if (playerSelected.ObjetoAgarrado == null && items[selectedSlot] != null)
                EquiparSlotSeleccionado();
        }
    }

    private void EquiparSlotSeleccionado()
    {
        if (playerSelected == null) return;
        if (items[selectedSlot] == null) return;

        GameObject item = items[selectedSlot];
        items[selectedSlot] = null;

        // Recalcula currentSlots
        currentSlots = 0;
        for (int i = 0; i < 4; i++)
            if (items[i] != null) currentSlots = i + 1;

        // Activa y equipa
        item.layer = LayerMask.NameToLayer("Agarrado");
        item.SetActive(true);
        playerSelected.AgarrarDesdeInventario(item);

        RefreshUI();
    }

    public bool AddItem(GameObject item)
    {
        if (currentSlots >= 4)
        {
            Debug.Log("Inventario lleno");
            return false;
        }

        // Restaura el layer a Default antes de guardar
        item.layer = 0;
        item.SetActive(false);

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