using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    void Awake()
    {
        CreateBookReaderUI();
        CreateInstructionsUI();
    }

    void CreateBookReaderUI()
    {
        // Canvas
        GameObject canvasObj = new GameObject("BookReaderCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        // CanvasGroup
        CanvasGroup cg = canvasObj.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        // Background dim
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Panel
        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvasObj.transform);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.12f, 0.08f);
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(700, 500);
        panelRect.anchoredPosition = Vector2.zero;

        // Cover image
        GameObject coverObj = new GameObject("CoverImage");
        coverObj.transform.SetParent(panelObj.transform);
        Image coverImage = coverObj.AddComponent<Image>();
        coverImage.color = Color.red;
        RectTransform coverRect = coverObj.GetComponent<RectTransform>();
        coverRect.anchorMin = new Vector2(0, 1);
        coverRect.anchorMax = new Vector2(0, 1);
        coverRect.pivot = new Vector2(0, 1);
        coverRect.sizeDelta = new Vector2(120, 160);
        coverRect.anchoredPosition = new Vector2(30, -30);

        // Title
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panelObj.transform);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 32;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = new Color(0.95f, 0.9f, 0.8f);
        titleText.alignment = TextAnchor.MiddleLeft;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0, 1);
        titleRect.sizeDelta = new Vector2(-200, 50);
        titleRect.anchoredPosition = new Vector2(170, -35);

        // Author
        GameObject authorObj = new GameObject("AuthorText");
        authorObj.transform.SetParent(panelObj.transform);
        Text authorText = authorObj.AddComponent<Text>();
        authorText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        authorText.fontSize = 18;
        authorText.color = new Color(0.7f, 0.65f, 0.55f);
        authorText.alignment = TextAnchor.MiddleLeft;
        RectTransform authorRect = authorObj.GetComponent<RectTransform>();
        authorRect.anchorMin = new Vector2(0, 1);
        authorRect.anchorMax = new Vector2(1, 1);
        authorRect.pivot = new Vector2(0, 1);
        authorRect.sizeDelta = new Vector2(-200, 30);
        authorRect.anchoredPosition = new Vector2(170, -80);

        // Content background
        GameObject contentBgObj = new GameObject("ContentBackground");
        contentBgObj.transform.SetParent(panelObj.transform);
        Image contentBg = contentBgObj.AddComponent<Image>();
        contentBg.color = new Color(0.95f, 0.92f, 0.88f);
        RectTransform contentBgRect = contentBgObj.GetComponent<RectTransform>();
        contentBgRect.anchorMin = new Vector2(0, 0);
        contentBgRect.anchorMax = new Vector2(1, 1);
        contentBgRect.pivot = new Vector2(0.5f, 0.5f);
        contentBgRect.offsetMin = new Vector2(30, 30);
        contentBgRect.offsetMax = new Vector2(-30, -210);

        // Scroll View
        GameObject scrollObj = new GameObject("ScrollView");
        scrollObj.transform.SetParent(contentBgObj.transform);
        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();
        RectTransform scrollRectTransform = scrollObj.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = Vector2.zero;
        scrollRectTransform.anchorMax = Vector2.one;
        scrollRectTransform.offsetMin = Vector2.zero;
        scrollRectTransform.offsetMax = Vector2.zero;

        // Viewport
        GameObject viewportObj = new GameObject("Viewport");
        viewportObj.transform.SetParent(scrollObj.transform);
        RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewportObj.AddComponent<Mask>().showMaskGraphic = false;
        Image viewportImage = viewportObj.AddComponent<Image>();
        viewportImage.color = new Color(1, 1, 1, 0.01f);
        scrollRect.viewport = viewportRect;

        // Content
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(viewportObj.transform);
        Text contentText = contentObj.AddComponent<Text>();
        contentText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        contentText.fontSize = 18;
        contentText.color = new Color(0.15f, 0.1f, 0.05f);
        contentText.alignment = TextAnchor.UpperLeft;
        RectTransform contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 0);
        contentRect.anchoredPosition = new Vector2(0, 0);
        scrollRect.content = contentRect;

        // Close button
        GameObject closeObj = new GameObject("CloseButton");
        closeObj.transform.SetParent(panelObj.transform);
        Button closeButton = closeObj.AddComponent<Button>();
        Image closeImage = closeObj.AddComponent<Image>();
        closeImage.color = new Color(0.6f, 0.2f, 0.2f);
        RectTransform closeRect = closeObj.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 1);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.pivot = new Vector2(1, 1);
        closeRect.sizeDelta = new Vector2(40, 40);
        closeRect.anchoredPosition = new Vector2(-10, -10);

        // Close button text
        GameObject closeTextObj = new GameObject("Text");
        closeTextObj.transform.SetParent(closeObj.transform);
        Text closeText = closeTextObj.AddComponent<Text>();
        closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        closeText.text = "×";
        closeText.fontSize = 28;
        closeText.color = Color.white;
        closeText.alignment = TextAnchor.MiddleCenter;
        RectTransform closeTextRect = closeTextObj.GetComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.offsetMin = Vector2.zero;
        closeTextRect.offsetMax = Vector2.zero;

        // BookReaderUI component
        BookReaderUI readerUI = canvasObj.AddComponent<BookReaderUI>();
        readerUI.canvasGroup = cg;
        readerUI.panel = panelRect;
        readerUI.titleText = titleText;
        readerUI.authorText = authorText;
        readerUI.contentText = contentText;
        readerUI.closeButton = closeButton;
        readerUI.coverImage = coverImage;
        readerUI.scrollRect = scrollRect;
    }

    void CreateInstructionsUI()
    {
        GameObject canvasObj = new GameObject("InstructionsCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvasObj.transform);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.5f);
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.sizeDelta = new Vector2(340, 150);
        panelRect.anchoredPosition = new Vector2(20, -20);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(panelObj.transform);
        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 16;
        text.color = new Color(0.9f, 0.9f, 0.85f);
        text.alignment = TextAnchor.UpperLeft;
        text.text = "<b>图书馆整理游戏</b>\n\n" +
                    "<color=#FFD700>左键拖拽</color> - 移动书籍\n" +
                    "<color=#FFD700>悬停 + F键</color> - 打开阅读\n" +
                    "<color=#FFD700>右键拖动</color> - 旋转视角\n" +
                    "<color=#FFD700>滚轮</color> - 缩放视角\n" +
                    "<color=#FFD700>ESC</color> - 关闭阅读界面";
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(15, 10);
        textRect.offsetMax = new Vector2(-15, -10);
    }
}
