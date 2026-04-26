using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    [Header("Linterna")]
    public Light spotLight;
    public KeyCode toggleKey = KeyCode.F;

    [Header("Batería")]
    public float maxBattery = 100f;
    public float drainRate = 5f;
    public float lowBatteryThreshold = 20f;

    [Header("Parpadeo batería baja")]
    public float flickerSpeed = 0.1f;

    [Header("UI")]
    public Slider batterySlider;
    public Image batteryFill;
    public Color fullColor = Color.green;
    public Color lowColor = Color.red;

    private float currentBattery;
    private bool isOn = false;
    private bool isHeld = false;
    private float flickerTimer = 0f;

    private void Start()
    {
        currentBattery = maxBattery;
        spotLight.enabled = false;
        UpdateUI();
    }

    private void Update()
    {
        if (!isHeld)
        {
            if (isOn) TurnOff();
            return;
        }

        if (Input.GetKeyDown(toggleKey))
        {
            if (!isOn && currentBattery > 0f)
                TurnOn();
            else
                TurnOff();
        }

        if (isOn && currentBattery > 0f)
        {
            currentBattery -= drainRate * Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

            if (currentBattery <= 0f)
                TurnOff();

            if (currentBattery <= lowBatteryThreshold)
                HandleFlicker();

            UpdateUI();
        }
    }

    public void SetHeld(bool held)
    {
        isHeld = held;
        if (!held) TurnOff();
    }

    private void TurnOn()
    {
        isOn = true;
        spotLight.enabled = true;
    }

    private void TurnOff()
    {
        isOn = false;
        spotLight.enabled = false;
    }

    private void HandleFlicker()
    {
        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0f)
        {
            spotLight.enabled = !spotLight.enabled;
            float batteryPercent = currentBattery / lowBatteryThreshold;
            flickerTimer = flickerSpeed * batteryPercent;
        }
    }

    public void AddBattery(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0f, maxBattery);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (batterySlider != null)
            batterySlider.value = currentBattery / maxBattery;

        if (batteryFill != null)
            batteryFill.color = Color.Lerp(lowColor, fullColor, currentBattery / maxBattery);
    }
}