using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bookPrefab;

    [Header("Bookshelf")]
    public Bookshelf bookshelfPrefab;
    public Transform bookshelfSpawnPoint;

    [Header("Floor")]
    public Transform floor;
    public Vector2 floorBounds = new Vector2(8f, 6f);

    [Header("Books")]
    public int bookCount = 20;
    public List<BookData> bookDataList = new List<BookData>();

    [Header("Camera")]
    public OrbitCamera orbitCamera;

    private List<Book> spawnedBooks = new List<Book>();
    private Bookshelf activeShelf;

    void Start()
    {
        CreateFloor();
        CreateBookshelf();
        CreateBooks();
        SetupCamera();
        SetupController();
    }

    void CreateFloor()
    {
        GameObject existingFloor = GameObject.Find("Floor");
        if (existingFloor != null)
        {
            floor = existingFloor.transform;
            return;
        }

        GameObject floorObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floorObj.name = "Floor";
        floorObj.transform.position = Vector3.zero;
        floorObj.transform.localScale = new Vector3(floorBounds.x / 5f, 1f, floorBounds.y / 5f);
        
        var renderer = floorObj.GetComponent<Renderer>();
        var mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.85f, 0.78f, 0.68f);
        renderer.material = mat;
        
        floor = floorObj.transform;
    }

    void CreateBookshelf()
    {
        if (bookshelfPrefab != null && bookshelfSpawnPoint != null)
        {
            activeShelf = Instantiate(bookshelfPrefab, bookshelfSpawnPoint.position, bookshelfSpawnPoint.rotation);
        }
        else
        {
            Vector3 spawnPos = bookshelfSpawnPoint != null ? bookshelfSpawnPoint.position : new Vector3(0, 0, -3f);
            Quaternion spawnRot = bookshelfSpawnPoint != null ? bookshelfSpawnPoint.rotation : Quaternion.identity;
            GameObject shelfObj = new GameObject("Bookshelf");
            shelfObj.transform.position = spawnPos;
            shelfObj.transform.rotation = spawnRot;
            activeShelf = shelfObj.AddComponent<Bookshelf>();
            activeShelf.rows = 4;
            activeShelf.columns = 8;
            activeShelf.shelfWidth = 4f;
            activeShelf.shelfHeight = 2.8f;
            activeShelf.shelfDepth = 0.45f;
        }
    }

    void CreateBooks()
    {
        if (bookPrefab == null)
        {
            bookPrefab = CreateBookPrefab();
        }

        // Generate default book data if none provided
        if (bookDataList == null || bookDataList.Count == 0)
        {
            bookDataList = GenerateDefaultBookData();
        }

        for (int i = 0; i < bookCount; i++)
        {
            Vector3 spawnPos = GetRandomFloorPosition();
            spawnPos.y = 0.15f;

            GameObject bookObj = Instantiate(bookPrefab, spawnPos, Random.rotation);
            bookObj.SetActive(true);
            Book book = bookObj.GetComponent<Book>();
            
            if (book != null && i < bookDataList.Count)
            {
                book.Initialize(bookDataList[i]);
            }
            else if (book != null)
            {
                var fallbackData = ScriptableObject.CreateInstance<BookData>();
                fallbackData.bookTitle = $"书籍 #{i + 1}";
                fallbackData.author = "匿名";
                fallbackData.content = "这是一本神秘的书籍，内容等待你去发现。";
                fallbackData.coverColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.3f, 0.8f);
                book.Initialize(fallbackData);
            }

            spawnedBooks.Add(book);
        }
    }

    Vector3 GetRandomFloorPosition()
    {
        float x = Random.Range(-floorBounds.x / 2f + 1f, floorBounds.x / 2f - 1f);
        float z = Random.Range(-floorBounds.y / 2f + 1f, floorBounds.y / 2f - 1f);
        
        // Avoid bookshelf area
        if (activeShelf != null)
        {
            Vector3 shelfPos = activeShelf.transform.position;
            float shelfWidth = activeShelf.shelfWidth + 1f;
            float shelfDepth = activeShelf.shelfDepth + 1f;
            
            if (Mathf.Abs(x - shelfPos.x) < shelfWidth / 2f && Mathf.Abs(z - shelfPos.z) < shelfDepth / 2f + 1f)
            {
                x += shelfWidth;
            }
        }
        
        return new Vector3(x, 0, z);
    }

    void SetupCamera()
    {
        if (orbitCamera == null)
        {
            var camObj = Camera.main;
            if (camObj == null)
            {
                camObj = new GameObject("Main Camera").AddComponent<Camera>();
                camObj.tag = "MainCamera";
                camObj.transform.position = new Vector3(0, 5f, -6f);
            }
            
            orbitCamera = camObj.gameObject.AddComponent<OrbitCamera>();
        }

        if (orbitCamera.target == null)
        {
            GameObject targetObj = new GameObject("CameraTarget");
            targetObj.transform.position = new Vector3(0, 1f, 0);
            orbitCamera.target = targetObj.transform;
        }
    }

    void SetupController()
    {
        var controller = FindAnyObjectByType<BookDragController>();
        if (controller == null)
        {
            GameObject ctrlObj = new GameObject("BookDragController");
            controller = ctrlObj.AddComponent<BookDragController>();
        }

        controller.gameCamera = Camera.main;
        controller.SetTargetBookshelf(activeShelf);
    }

    GameObject CreateBookPrefab()
    {
        GameObject bookObj = new GameObject("BookPrefab");
        bookObj.SetActive(false);

        // Main body
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(bookObj.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.25f, 0.35f, 0.08f);
        Destroy(body.GetComponent<Collider>());

        // Cover (front)
        GameObject coverFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
        coverFront.name = "CoverFront";
        coverFront.transform.SetParent(bookObj.transform);
        coverFront.transform.localPosition = new Vector3(0, 0, 0.041f);
        coverFront.transform.localScale = new Vector3(0.26f, 0.36f, 0.002f);
        Destroy(coverFront.GetComponent<Collider>());

        // Cover (back)
        GameObject coverBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        coverBack.name = "CoverBack";
        coverBack.transform.SetParent(bookObj.transform);
        coverBack.transform.localPosition = new Vector3(0, 0, -0.041f);
        coverBack.transform.localScale = new Vector3(0.26f, 0.36f, 0.002f);
        Destroy(coverBack.GetComponent<Collider>());

        // Spine
        GameObject spine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        spine.name = "Spine";
        spine.transform.SetParent(bookObj.transform);
        spine.transform.localPosition = new Vector3(-0.126f, 0, 0);
        spine.transform.localScale = new Vector3(0.002f, 0.36f, 0.082f);
        Destroy(spine.GetComponent<Collider>());

        // Pages
        GameObject pages = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pages.name = "Pages";
        pages.transform.SetParent(bookObj.transform);
        pages.transform.localPosition = new Vector3(0.005f, 0, 0);
        pages.transform.localScale = new Vector3(0.24f, 0.34f, 0.076f);
        Destroy(pages.GetComponent<Collider>());

        // Setup materials
        var pageMat = new Material(Shader.Find("Standard"));
        pageMat.color = new Color(0.95f, 0.92f, 0.88f);
        pages.GetComponent<Renderer>().material = pageMat;

        var coverMat = new Material(Shader.Find("Standard"));
        coverMat.color = Color.red;
        coverFront.GetComponent<Renderer>().material = coverMat;
        coverBack.GetComponent<Renderer>().material = coverMat;

        var spineMat = new Material(Shader.Find("Standard"));
        spineMat.color = Color.gray;
        spine.GetComponent<Renderer>().material = spineMat;

        var bodyMat = new Material(Shader.Find("Standard"));
        bodyMat.color = new Color(0.95f, 0.92f, 0.88f);
        body.GetComponent<Renderer>().material = bodyMat;

        // Collider
        var boxCollider = bookObj.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(0.26f, 0.36f, 0.084f);
        boxCollider.center = Vector3.zero;

        // Rigidbody
        var rb = bookObj.AddComponent<Rigidbody>();
        rb.mass = 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Book script
        var book = bookObj.AddComponent<Book>();
        book.coverFront = coverFront.transform;
        book.coverBack = coverBack.transform;
        book.spine = spine.transform;
        book.pages = pages.transform;
        book.coverRenderer = coverFront.GetComponent<Renderer>();
        book.spineRenderer = spine.GetComponent<Renderer>();
        book.pagesRenderer = pages.GetComponent<Renderer>();

        // Layer
        bookObj.layer = LayerMask.NameToLayer("Book");
        foreach (Transform child in bookObj.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Book");
        }

        return bookObj;
    }

    List<BookData> GenerateDefaultBookData()
    {
        var list = new List<BookData>();
        
        string[] titles = new string[]
        {
            "红楼梦", "西游记", "三国演义", "水浒传",
            "百年孤独", "追风筝的人", "小王子", "老人与海",
            "傲慢与偏见", "简·爱", "呼啸山庄", "了不起的盖茨比",
            "1984", "动物农场", "麦田里的守望者", "杀死一只知更鸟",
            "局外人", "挪威的森林", "活着", "围城"
        };

        string[] authors = new string[]
        {
            "曹雪芹", "吴承恩", "罗贯中", "施耐庵",
            "加西亚·马尔克斯", "卡勒德·胡赛尼", "安托万·德·圣埃克苏佩里", "海明威",
            "简·奥斯汀", "夏洛蒂·勃朗特", "艾米莉·勃朗特", "菲茨杰拉德",
            "乔治·奥威尔", "乔治·奥威尔", "塞林格", "哈珀·李",
            "加缪", "村上春树", "余华", "钱钟书"
        };

        string[] contents = new string[]
        {
            "满纸荒唐言，一把辛酸泪。都云作者痴，谁解其中味？\n\n《红楼梦》是中国古典小说的巅峰之作，以贾宝玉、林黛玉、薛宝钗的爱情婚姻悲剧为主线，描绘了一个封建大家族的兴衰历程。",
            "你挑着担，我牵着马，迎来日出送走晚霞。\n\n《西游记》讲述了唐僧师徒四人历经九九八十一难，西天取经的故事。孙悟空的神通广大、猪八戒的憨厚可爱、沙僧的忠厚老实，都给读者留下了深刻印象。",
            "滚滚长江东逝水，浪花淘尽英雄。\n\n《三国演义》描写了从东汉末年到西晋初年之间近百年的历史风云，塑造了诸葛亮、曹操、关羽等众多栩栩如生的人物形象。",
            "路见不平一声吼，该出手时就出手。\n\n《水浒传》描写了北宋末年以宋江为首的108位好汉在梁山聚义，以及聚义之后接受招安、四处征战的故事。",
            "多年以后，面对行刑队，奥雷里亚诺·布恩迪亚上校将会回想起父亲带他去见识冰块的那个遥远的下午。\n\n《百年孤独》是魔幻现实主义的代表作，讲述了布恩迪亚家族七代人的传奇故事。",
            "为你，千千万万遍。\n\n《追风筝的人》讲述了阿富汗少年阿米尔与仆人哈桑之间的友谊与背叛，以及成年后的救赎之旅。",
            "只有用心才能看得清。实质性的东西，用眼睛是看不见的。\n\n《小王子》是一部充满哲理的童话，通过小王子的星际旅行，探讨了爱、责任和生命的意义。",
            "人不是为失败而生的，一个人可以被毁灭，但不能被打败。\n\n《老人与海》讲述了一位老年古巴渔夫与一条巨大的马林鱼在离岸很远的湾流中搏斗的故事。",
            "凡是有钱的单身汉，总想娶位太太，这已经成了一条举世公认的真理。\n\n《傲慢与偏见》描写了小乡绅班纳特家五个女儿的婚事，着重表现了伊丽莎白和达西之间因傲慢与偏见而产生的爱情波折。",
            "你以为，因为我穷、低微、不美、矮小，我就没有灵魂没有心吗？\n\n《简·爱》讲述了一位从小变成孤儿的英国女子在各种磨难中不断追求自由与尊严的故事。",
            "他永远也不会知道我爱他，我爱他不是因为他长得英俊，而是因为他比我更像我自己。\n\n《呼啸山庄》描写了吉卜赛弃儿希斯克利夫被山庄老主人收养后，因受辱和恋爱不遂，外出致富，回来后对与其女友凯瑟琳结婚的地主林顿及其子女进行报复的故事。",
            "每逢你想要批评任何人的时候，你就记住，这个世界上所有的人，并不是个个都有过你拥有的那些优越条件。\n\n《了不起的盖茨比》以20世纪20年代的纽约市及长岛为背景，揭示了美国梦的幻灭。",
            "战争即和平，自由即奴役，无知即力量。\n\n《1984》是一部反乌托邦小说，描绘了一个极权主义社会的恐怖景象。",
            "所有动物一律平等，但有些动物比其他动物更加平等。\n\n《动物农场》是一部政治寓言小说，通过动物革命的故事讽刺了苏联的极权主义。",
            "一个不成熟男子的标志是他愿意为某种事业英勇地死去，一个成熟男子的标志是他愿意为某种事业卑贱地活着。\n\n《麦田里的守望者》通过16岁少年霍尔顿·考尔菲德的视角，展现了青春期的困惑与迷茫。",
            "勇敢是，当你还未开始就已知道自己会输，可你依然要去做，而且无论如何都要把它坚持到底。\n\n《杀死一只知更鸟》通过一个小女孩的视角，展现了美国南部种族歧视的问题。",
            "今天，妈妈死了。也许是昨天，我不知道。\n\n《局外人》是存在主义文学的代表作，讲述了主人公默尔索对社会规范的冷漠与反抗。",
            "死并非生的对立面，而作为生的一部分永存。\n\n《挪威的森林》是一部动人心弦的恋爱小说，讲述了主人公渡边在两个女孩之间的情感纠葛。",
            "人是为了活着本身而活着的，而不是为了活着之外的任何事物所活着。\n\n《活着》讲述了福贵一生的悲欢离合，展现了人在极端困境中的生存意志。",
            "婚姻是一座围城，城外的人想进去，城里的人想出来。\n\n《围城》以幽默讽刺的笔调，描写了抗战初期知识分子的群像。"
        };

        Color[] colors = new Color[]
        {
            new Color(0.8f, 0.2f, 0.2f), new Color(0.2f, 0.5f, 0.8f), new Color(0.3f, 0.7f, 0.3f),
            new Color(0.6f, 0.3f, 0.1f), new Color(0.5f, 0.2f, 0.6f), new Color(0.9f, 0.6f, 0.1f),
            new Color(0.2f, 0.6f, 0.7f), new Color(0.7f, 0.2f, 0.4f), new Color(0.4f, 0.4f, 0.5f),
            new Color(0.8f, 0.5f, 0.3f), new Color(0.3f, 0.5f, 0.4f), new Color(0.6f, 0.4f, 0.7f),
            new Color(0.5f, 0.6f, 0.2f), new Color(0.7f, 0.3f, 0.3f), new Color(0.2f, 0.4f, 0.6f),
            new Color(0.8f, 0.7f, 0.2f), new Color(0.4f, 0.7f, 0.6f), new Color(0.6f, 0.2f, 0.5f),
            new Color(0.3f, 0.6f, 0.5f), new Color(0.7f, 0.5f, 0.4f)
        };

        for (int i = 0; i < titles.Length && i < bookCount; i++)
        {
            var data = ScriptableObject.CreateInstance<BookData>();
            data.bookTitle = titles[i];
            data.author = authors[i];
            data.content = contents[i];
            data.coverColor = colors[i % colors.Length];
            data.spineColor = Color.Lerp(data.coverColor, Color.black, 0.3f);
            data.thickness = Random.Range(0.04f, 0.12f);
            list.Add(data);
        }

        return list;
    }
}
