using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        GameObject bootstrap = new GameObject("__Bootstrap__");
        bootstrap.AddComponent<Bootstrap>();
        DontDestroyOnLoad(bootstrap);
    }

    void Awake()
    {
        SetupCamera();
        SetupLighting();
        SetupManagers();
        SetupFloor();
        Destroy(gameObject);
    }

    void SetupCamera()
    {
        if (Camera.main == null)
        {
            GameObject camObj = new GameObject("MainCamera");
            camObj.tag = "MainCamera";
            Camera cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.backgroundColor = new Color(0.15f, 0.12f, 0.1f);
            cam.transform.position = new Vector3(0, 3.5f, -7f);
            cam.transform.rotation = Quaternion.Euler(25, 0, 0);
            cam.fieldOfView = 50;
            // AudioListener requires Audio module, skip to avoid dependency
        }
        else
        {
            Camera.main.transform.position = new Vector3(0, 3.5f, -7f);
            Camera.main.transform.rotation = Quaternion.Euler(25, 0, 0);
            Camera.main.fieldOfView = 50;
        }
    }

    void SetupLighting()
    {
        // Remove any existing lights first to avoid duplicates
        var existingLights = FindObjectsByType<Light>();
        foreach (var l in existingLights)
        {
            if (l.gameObject.name.Contains("Directional") || l.gameObject.name.Contains("Global"))
            {
                Destroy(l.gameObject);
            }
        }

        // Main directional light - positioned far away and angled to illuminate the bookshelf
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        light.color = new Color(1f, 0.97f, 0.9f);
        light.shadows = LightShadows.Soft;
        // Position far above and behind camera to illuminate bookshelf at z=-3
        lightObj.transform.position = new Vector3(0, 20, -20);
        lightObj.transform.rotation = Quaternion.Euler(45, 0, 0);

        // Fill light from the side for softer shadows
        GameObject fillLightObj = new GameObject("Fill Light");
        Light fillLight = fillLightObj.AddComponent<Light>();
        fillLight.type = LightType.Directional;
        fillLight.intensity = 0.4f;
        fillLight.color = new Color(0.85f, 0.9f, 1f);
        fillLightObj.transform.position = new Vector3(-10, 10, -5);
        fillLightObj.transform.rotation = Quaternion.Euler(30, 60, 0);

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 0.5f;
    }

    void SetupManagers()
    {
        if (FindAnyObjectByType<UIManager>() == null)
        {
            GameObject uiMgr = new GameObject("UIManager");
            uiMgr.AddComponent<UIManager>();
            DontDestroyOnLoad(uiMgr);
        }

        if (FindAnyObjectByType<GameManager>() == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }
    }

    void SetupFloor()
    {
        if (GameObject.Find("Floor") == null)
        {
            GameObject floorObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floorObj.name = "Floor";
            floorObj.transform.position = Vector3.zero;
            floorObj.transform.localScale = new Vector3(2f, 1f, 1.5f);
            
            var renderer = floorObj.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.82f, 0.75f, 0.65f);
            renderer.material = mat;
        }
    }
}
