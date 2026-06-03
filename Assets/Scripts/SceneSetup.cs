using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    void Awake()
    {
        SetupLayers();
        SetupLighting();
        SetupManagers();
    }

    void SetupLayers()
    {
        // Ensure layers exist (can't create at runtime, but we can warn)
        // Layers should be set up in Unity Editor:
        // Layer 6: Book
        // Layer 7: Floor
        // Layer 8: Shelf
    }

    void SetupLighting()
    {
        // Directional light
        var lightObj = GameObject.Find("Directional Light");
        if (lightObj == null)
        {
            lightObj = new GameObject("Directional Light");
            var light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.color = new Color(1f, 0.98f, 0.95f);
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
        }

        // Ambient light
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 0.8f;

        // Global light (for 6000 series)
        var globalLight = GameObject.Find("GlobalLight");
        if (globalLight == null)
        {
            globalLight = new GameObject("GlobalLight");
            var gl = globalLight.AddComponent<Light>();
            gl.type = LightType.Directional;
            gl.intensity = 0.3f;
            gl.color = new Color(0.8f, 0.85f, 1f);
            gl.transform.rotation = Quaternion.Euler(-30, 120, 0);
        }
    }

    void SetupManagers()
    {
        // UIManager
        if (FindAnyObjectByType<UIManager>() == null)
        {
            GameObject uiMgr = new GameObject("UIManager");
            uiMgr.AddComponent<UIManager>();
        }

        // GameManager
        if (FindAnyObjectByType<GameManager>() == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }
    }
}
