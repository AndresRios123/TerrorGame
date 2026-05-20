using UnityEngine;

public class ReadableNote : MonoBehaviour
{
    [Header("Contenido")]
    public string noteTitle = "Nota";

    [TextArea(5, 15)]
    public string noteContent = "Contenido de la nota...";

    [Header("Audio")]
    public AudioClip readSound;

    public void Read()
    {
        if (readSound != null)
            AudioSource.PlayClipAtPoint(readSound, transform.position, 1f);

        if (NoteReaderUI.Instance != null)
            NoteReaderUI.Instance.ShowNote(noteTitle, noteContent);
    }
}
