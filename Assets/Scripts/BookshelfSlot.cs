using UnityEngine;

public class BookshelfSlot : MonoBehaviour
{
    public bool IsOccupied => currentBook != null;
    public Book CurrentBook => currentBook;

    private Book currentBook;
    private Renderer slotRenderer;
    private Material slotMat;

    void Awake()
    {
        slotRenderer = GetComponent<Renderer>();
        if (slotRenderer != null)
        {
            slotMat = new Material(slotRenderer.sharedMaterial);
            slotRenderer.material = slotMat;
        }
    }

    public void Highlight(bool active)
    {
        if (slotMat == null) return;
        if (active)
        {
            slotMat.color = new Color(0.3f, 1f, 0.3f, 0.5f);
            slotMat.EnableKeyword("_EMISSION");
            slotMat.SetColor("_EmissionColor", Color.green * 0.3f);
        }
        else
        {
            slotMat.color = new Color(1f, 1f, 1f, 0.1f);
            slotMat.DisableKeyword("_EMISSION");
        }
    }

    public bool CanPlaceBook(Book book)
    {
        return currentBook == null && book != null;
    }

    public void PlaceBook(Book book)
    {
        if (!CanPlaceBook(book)) return;
        currentBook = book;
        book.PlaceOnSlot(this);
        Highlight(false);
    }

    public void ReleaseBook()
    {
        currentBook = null;
    }
}
