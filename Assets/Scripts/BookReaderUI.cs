using UnityEngine;
using UnityEngine.UI;

public class BookReaderUI : MonoBehaviour
{
    public static BookReaderUI Instance { get; private set; }

    [Header("UI References")]
    public CanvasGroup canvasGroup;
    public RectTransform panel;
    public Text titleText;
    public Text authorText;
    public Text contentText;
    public Button closeButton;
    public Image coverImage;
    public ScrollRect scrollRect;

    [Header("Animation")]
    public float fadeSpeed = 8f;
    public float scaleSpeed = 12f;

    private bool isOpen = false;
    private Vector3 targetScale = Vector3.one;
    private Vector3 closedScale = new Vector3(0.8f, 0.8f, 0.8f);

    void Awake()
    {
        Instance = this;
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        panel.localScale = closedScale;

        closeButton?.onClick.AddListener(CloseBook);
    }

    void Update()
    {
        float targetAlpha = isOpen ? 1f : 0f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        
        Vector3 target = isOpen ? targetScale : closedScale;
        panel.localScale = Vector3.Lerp(panel.localScale, target, Time.deltaTime * scaleSpeed);

        if (!isOpen && canvasGroup.alpha < 0.01f)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
        {
            CloseBook();
        }
    }

    public void OpenBook(BookData data)
    {
        if (data == null) return;

        titleText.text = data.bookTitle;
        authorText.text = "作者: " + data.author;
        contentText.text = data.content;
        coverImage.color = data.coverColor;

        scrollRect.normalizedPosition = new Vector2(0, 1);

        isOpen = true;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void CloseBook()
    {
        isOpen = false;
    }
}
