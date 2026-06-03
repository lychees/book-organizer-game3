using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Book : MonoBehaviour
{
    [Header("Data")]
    public BookData data;

    [Header("Visual")]
    public Transform coverFront;
    public Transform coverBack;
    public Transform spine;
    public Transform pages;
    public Renderer coverRenderer;
    public Renderer spineRenderer;
    public Renderer pagesRenderer;

    [Header("State")]
    public bool isDragging = false;
    public bool isOnShelf = false;
    public BookshelfSlot currentSlot;

    private Rigidbody rb;
    private Material coverMat;
    private Material spineMat;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Transform originalParent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
        originalParent = transform.parent;
    }

    public void Initialize(BookData bookData)
    {
        data = bookData;
        if (data == null) return;

        // Update visuals
        if (coverRenderer != null)
        {
            coverMat = new Material(coverRenderer.sharedMaterial);
            coverMat.color = data.coverColor;
            coverRenderer.material = coverMat;
        }

        if (spineRenderer != null)
        {
            spineMat = new Material(spineRenderer.sharedMaterial);
            spineMat.color = data.spineColor;
            spineRenderer.material = spineMat;
        }

        // Set thickness
        float t = Mathf.Clamp(data.thickness, 0.03f, 0.3f);
        Vector3 scale = transform.localScale;
        scale.z = t;
        transform.localScale = scale;

        // Adjust page visual
        if (pages != null)
        {
            Vector3 ps = pages.localScale;
            ps.z = t - 0.02f;
            pages.localScale = ps;
        }

        gameObject.name = "Book: " + data.bookTitle;
    }

    public void OnHoverEnter()
    {
        if (isDragging) return;
        if (coverMat != null)
        {
            coverMat.SetFloat("_Metallic", 0.5f);
            coverMat.EnableKeyword("_EMISSION");
            coverMat.SetColor("_EmissionColor", Color.white * 0.15f);
        }
        transform.localScale = originalScale * 1.05f;
    }

    public void OnHoverExit()
    {
        if (isDragging) return;
        if (coverMat != null)
        {
            coverMat.SetFloat("_Metallic", 0.0f);
            coverMat.DisableKeyword("_EMISSION");
        }
        transform.localScale = originalScale;
    }

    public void StartDrag()
    {
        isDragging = true;
        rb.useGravity = false;
        rb.linearDamping = 10f;
        rb.angularDamping = 10f;
        originalRotation = transform.rotation;

        // Detach from slot
        if (currentSlot != null)
        {
            currentSlot.ReleaseBook();
            currentSlot = null;
            isOnShelf = false;
        }

        // Lift slightly
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    public void EndDrag()
    {
        isDragging = false;
        rb.useGravity = true;
        rb.linearDamping = 0.05f;
        rb.angularDamping = 0.05f;
    }

    public void PlaceOnSlot(BookshelfSlot slot)
    {
        isOnShelf = true;
        currentSlot = slot;
        isDragging = false;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Snap to slot
        transform.SetParent(slot.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;
    }

    public void DetachFromShelf()
    {
        if (currentSlot != null)
        {
            currentSlot.ReleaseBook();
            currentSlot = null;
        }
        isOnShelf = false;
        transform.SetParent(originalParent);
        rb.useGravity = true;
    }

    public void ResetHighlight()
    {
        if (coverMat != null)
        {
            coverMat.SetFloat("_Metallic", 0.0f);
            coverMat.DisableKeyword("_EMISSION");
        }
        transform.localScale = originalScale;
    }
}
