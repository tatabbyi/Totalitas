using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// When Endscreen is loaded additively over BaseLevel1, disables this scene's camera, lights, and audio
/// so the level stays visible behind the UI. Keeps this scene's EventSystem for the end-screen buttons.
/// </summary>
public class EndscreenOverlaySetup : MonoBehaviour
{
    void Awake()
    {
        if (SceneManager.sceneCount <= 1)
            return;

        Scene s = gameObject.scene;

        foreach (var cam in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            if (cam != null && cam.gameObject.scene == s)
                cam.enabled = false;
        }

        foreach (var al in Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None))
        {
            if (al != null && al.gameObject.scene == s)
                al.enabled = false;
        }

        foreach (var light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            if (light != null && light.gameObject.scene == s && light.type == LightType.Directional)
                light.enabled = false;
        }

        var systems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        EventSystem keep = null;
        foreach (var es in systems)
        {
            if (es != null && es.gameObject.scene == s)
            {
                keep = es;
                break;
            }
        }

        if (keep != null)
        {
            foreach (var es in systems)
            {
                if (es != null && es != keep)
                    es.enabled = false;
            }
        }
    }
}
