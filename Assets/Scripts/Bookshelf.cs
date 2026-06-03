using UnityEngine;

public class Bookshelf : MonoBehaviour
{
    [Header("Dimensions")]
    public int rows = 4;
    public int columns = 8;
    public float shelfWidth = 3f;
    public float shelfHeight = 2.5f;
    public float shelfDepth = 0.4f;
    public float thickness = 0.05f;

    [Header("Materials")]
    public Material woodMaterial;

    private BookshelfSlot[,] slots;

    void Start()
    {
        BuildShelf();
    }

    void BuildShelf()
    {
        slots = new BookshelfSlot[rows, columns];

        // Create frame parent
        Transform frame = new GameObject("Frame").transform;
        frame.SetParent(transform);
        frame.localPosition = Vector3.zero;

        // Build sides
        CreateCube("LeftSide", new Vector3(-shelfWidth/2 - thickness/2, shelfHeight/2, 0), 
            new Vector3(thickness, shelfHeight + thickness*2, shelfDepth), frame);
        CreateCube("RightSide", new Vector3(shelfWidth/2 + thickness/2, shelfHeight/2, 0), 
            new Vector3(thickness, shelfHeight + thickness*2, shelfDepth), frame);
        CreateCube("Back", new Vector3(0, shelfHeight/2, -shelfDepth/2 - thickness/2), 
            new Vector3(shelfWidth + thickness*2, shelfHeight + thickness*2, thickness), frame);
        CreateCube("Top", new Vector3(0, shelfHeight + thickness/2, 0), 
            new Vector3(shelfWidth + thickness*2, thickness, shelfDepth), frame);
        CreateCube("Bottom", new Vector3(0, -thickness/2, 0), 
            new Vector3(shelfWidth + thickness*2, thickness, shelfDepth), frame);

        // Build shelves and slots
        float rowHeight = shelfHeight / rows;
        float colWidth = shelfWidth / columns;

        for (int r = 0; r < rows; r++)
        {
            float y = r * rowHeight;
            if (r > 0)
            {
                CreateCube($"Shelf_{r}", new Vector3(0, y, 0), 
                    new Vector3(shelfWidth + thickness*2, thickness, shelfDepth), frame);
            }

            for (int c = 0; c < columns; c++)
            {
                float x = -shelfWidth/2 + c * colWidth + colWidth/2;
                float slotY = y + rowHeight/2;
                
                GameObject slotObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slotObj.name = $"Slot_{r}_{c}";
                slotObj.transform.SetParent(transform);
                slotObj.transform.localPosition = new Vector3(x, slotY, 0);
                slotObj.transform.localScale = new Vector3(colWidth * 0.9f, rowHeight * 0.85f, shelfDepth * 0.8f);
                
                var slotRenderer = slotObj.GetComponent<Renderer>();
                var slotMat = new Material(Shader.Find("Standard"));
                slotMat.color = new Color(1f, 1f, 1f, 0.1f);
                slotMat.SetFloat("_Mode", 3);
                slotMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                slotMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                slotMat.SetInt("_ZWrite", 0);
                slotMat.DisableKeyword("_ALPHATEST_ON");
                slotMat.EnableKeyword("_ALPHABLEND_ON");
                slotMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                slotMat.renderQueue = 3000;
                slotRenderer.material = slotMat;

                Destroy(slotObj.GetComponent<Collider>());
                var slot = slotObj.AddComponent<BookshelfSlot>();
                slots[r, c] = slot;
            }
        }
    }

    void CreateCube(string name, Vector3 localPos, Vector3 localScale, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent);
        cube.transform.localPosition = localPos;
        cube.transform.localScale = localScale;
        
        if (woodMaterial != null)
        {
            cube.GetComponent<Renderer>().material = woodMaterial;
        }
        else
        {
            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.55f, 0.35f, 0.18f);
            cube.GetComponent<Renderer>().material = mat;
        }
    }

    public BookshelfSlot FindNearestEmptySlot(Vector3 position, float maxDistance = 0.5f)
    {
        BookshelfSlot nearest = null;
        float nearestDist = maxDistance;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var slot = slots[r, c];
                if (slot == null || slot.IsOccupied) continue;

                float dist = Vector3.Distance(position, slot.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = slot;
                }
            }
        }
        return nearest;
    }

    public void HighlightNearestSlot(Vector3 position, float maxDistance = 0.5f)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var slot = slots[r, c];
                if (slot == null) continue;
                slot.Highlight(false);
            }
        }

        var nearest = FindNearestEmptySlot(position, maxDistance);
        nearest?.Highlight(true);
    }

    public void ClearHighlights()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                slots[r, c]?.Highlight(false);
            }
        }
    }
}
