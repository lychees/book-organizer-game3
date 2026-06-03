using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class BuildScript
{
    public static void CompileCheck()
    {
        Debug.Log("Compilation successful!");
    }
    
    public static void RunSceneTest()
    {
        string scenePath = "Assets/Scenes/MainScene.unity";
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        
        // Enter play mode to trigger Bootstrap
        EditorApplication.EnterPlaymode();
        EditorApplication.update += OnEditorUpdate;
    }
    
    static int frameCount = 0;
    static void OnEditorUpdate()
    {
        if (!EditorApplication.isPlaying) return;
        
        frameCount++;
        if (frameCount < 10) return; // Wait for play mode to initialize
        
        Debug.Log("=== Library Game Scene Test ===");
        
        var gm = Object.FindAnyObjectByType<GameManager>();
        Debug.Log($"GameManager: {gm != null}");
        
        var books = Object.FindObjectsByType<Book>(FindObjectsSortMode.None);
        Debug.Log($"Books: {books.Length}");
        
        var shelf = Object.FindAnyObjectByType<Bookshelf>();
        Debug.Log($"Bookshelf: {shelf != null}");
        
        var ui = Object.FindAnyObjectByType<BookReaderUI>();
        Debug.Log($"BookReaderUI: {ui != null}");
        
        var cam = Object.FindAnyObjectByType<OrbitCamera>();
        Debug.Log($"OrbitCamera: {cam != null}");
        
        var ctrl = Object.FindAnyObjectByType<BookDragController>();
        Debug.Log($"BookDragController: {ctrl != null}");
        
        var floor = GameObject.Find("Floor");
        Debug.Log($"Floor: {floor != null}");
        
        Debug.Log("=== Test Complete ===");
        
        EditorApplication.update -= OnEditorUpdate;
        EditorApplication.Exit(0);
    }
}
