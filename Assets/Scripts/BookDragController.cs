using UnityEngine;

public class BookDragController : MonoBehaviour
{
    [Header("Camera")]
    public Camera gameCamera;

    [Header("Settings")]
    public float dragHeight = 1.0f;
    public float snapDistance = 0.5f;
    public float dragStartDelay = 0.12f;
    public float dragStartDistance = 0.03f;
    public KeyCode readKey = KeyCode.F;

    private Book hoveredBook;
    private Book draggedBook;
    private Book clickedBook;
    private Bookshelf targetBookshelf;
    private float clickStartTime;
    private Vector3 clickStartMousePos;
    private Vector3 dragOffset;
    private float dragTargetY;
    private int bookLayer;
    private int floorLayer;

    void Awake()
    {
        bookLayer = LayerMask.GetMask("Book");
        floorLayer = LayerMask.GetMask("Floor");
        if (bookLayer == 0) bookLayer = ~0;
        if (floorLayer == 0) floorLayer = ~0;
    }

    void Update()
    {
        HandleHover();
        HandleInput();
        HandleDrag();
    }

    void HandleHover()
    {
        if (draggedBook != null) return;

        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, bookLayer))
        {
            Book book = hit.collider.GetComponentInParent<Book>();
            if (book != null && book != hoveredBook)
            {
                hoveredBook?.OnHoverExit();
                hoveredBook = book;
                hoveredBook.OnHoverEnter();
            }
        }
        else
        {
            if (hoveredBook != null)
            {
                hoveredBook.OnHoverExit();
                hoveredBook = null;
            }
        }
    }

    void HandleInput()
    {
        // Keyboard: read hovered book
        if (Input.GetKeyDown(readKey) && hoveredBook != null && draggedBook == null)
        {
            BookReaderUI.Instance?.OpenBook(hoveredBook.data);
        }

        // Mouse down: record potential click target
        if (Input.GetMouseButtonDown(0))
        {
            if (hoveredBook != null)
            {
                clickedBook = hoveredBook;
                clickStartTime = Time.time;
                clickStartMousePos = Input.mousePosition;
            }
        }

        // Mouse up: end drag
        if (Input.GetMouseButtonUp(0))
        {
            if (draggedBook != null)
            {
                bool placed = TryPlaceOnShelf();
                if (!placed) draggedBook.EndDrag();
                draggedBook = null;
                targetBookshelf?.ClearHighlights();
            }
            clickedBook = null;
        }
    }

    void HandleDrag()
    {
        // Start drag if mouse held down long enough or moved far enough
        if (clickedBook != null && draggedBook == null && Input.GetMouseButton(0))
        {
            float holdTime = Time.time - clickStartTime;
            float moveDist = Vector2.Distance(Input.mousePosition, clickStartMousePos);

            if (holdTime > dragStartDelay || moveDist > dragStartDistance * Screen.height)
            {
                draggedBook = clickedBook;
                draggedBook.StartDrag();

                Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, floorLayer))
                {
                    dragOffset = draggedBook.transform.position - hit.point;
                    dragTargetY = dragHeight;
                }
                else
                {
                    dragOffset = Vector3.zero;
                    dragTargetY = dragHeight;
                }
            }
        }

        if (draggedBook == null) return;

        Ray dragRay = gameCamera.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, dragTargetY, 0));

        if (dragPlane.Raycast(dragRay, out float enter))
        {
            Vector3 targetPos = dragRay.GetPoint(enter) + dragOffset;
            targetPos.y = dragTargetY;
            Vector3 force = (targetPos - draggedBook.transform.position) * 15f;
            draggedBook.GetComponent<Rigidbody>().linearVelocity = force;
        }

        if (targetBookshelf != null)
        {
            targetBookshelf.HighlightNearestSlot(draggedBook.transform.position, snapDistance);
        }
    }

    bool TryPlaceOnShelf()
    {
        if (targetBookshelf == null) return false;
        var slot = targetBookshelf.FindNearestEmptySlot(draggedBook.transform.position, snapDistance);
        if (slot != null)
        {
            slot.PlaceBook(draggedBook);
            return true;
        }
        return false;
    }

    public void SetTargetBookshelf(Bookshelf shelf)
    {
        targetBookshelf = shelf;
    }
}
