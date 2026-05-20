using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    public float batteryAmount = 30f;
    public float rotationSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void UsarBateria()
    {
        Debug.Log("UsarBateria llamado");

        Selected selected = FindObjectOfType<Selected>();
        Debug.Log("Selected: " + (selected != null));

        Flashlight flashlight = FindObjectOfType<Flashlight>();
        Debug.Log("Flashlight encontrada: " + (flashlight != null));

        if (flashlight == null) return;

        flashlight.AddBattery(batteryAmount);
        Debug.Log("Batería recargada: +" + batteryAmount);

        if (InventorySystem.Instance != null)
            InventorySystem.Instance.RemoveItem(gameObject.tag);
    }
}