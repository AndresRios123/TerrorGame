using UnityEngine;
using TMPro;

public class NoteReaderUI : MonoBehaviour
{
    public static NoteReaderUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject notePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;
    public TextMeshProUGUI closePromptText;

    private PlayerMovement playerMovement;
    public bool IsReading { get; private set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (notePanel != null)
            notePanel.SetActive(false);
    }

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        if (!IsReading) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
        {
            HideNote();
        }
    }

    public void ShowNote(string title, string content)
    {
        if (notePanel == null || titleText == null || contentText == null)
        {
            Debug.LogWarning("NoteReaderUI: Faltan referencias de UI.");
            return;
        }

        titleText.text = title;
        contentText.text = content;
        notePanel.SetActive(true);

        if (closePromptText != null)
            closePromptText.gameObject.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsReading = true;

        if (playerMovement != null)
            playerMovement.enabled = false;
    }

    public void HideNote()
    {
        if (notePanel != null)
            notePanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsReading = false;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }
}
