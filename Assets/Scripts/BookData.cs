using UnityEngine;

[CreateAssetMenu(fileName = "BookData", menuName = "Library/Book Data")]
public class BookData : ScriptableObject
{
    public string bookTitle = "Untitled";
    public string author = "Unknown";
    [TextArea(5, 20)]
    public string content = "No content available.";
    public Color coverColor = Color.red;
    public Color spineColor = Color.gray;
    public float thickness = 0.08f;
}
