using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Queues guide flow from the menu. Loads BaseLevel1 first with maze UI hidden, then Guide additively on top.
/// When the guide finishes, Guide unloads and maze UI appears (same BaseLevel1 instance).
/// </summary>
public static class GuideFlow
{
    public const string NextSceneKey = "NextSceneAfterGuide";
    /// <summary>When 1, BaseLevel1 should hide preset UI and load Guide additively.</summary>
    public const string GuideOverlayPendingKey = "GuideOverlayPending";

    /// <summary>
    /// Loads <paramref name="sceneAfterGuide"/> (usually BaseLevel1) as the active scene, with PlayerPrefs set so
    /// <see cref="GuideLevelBootstrap"/> loads <paramref name="guideSceneName"/> additively on top.
    /// </summary>
    public static void GoToGuideThen(string sceneAfterGuide, string guideSceneName = "Guide")
    {
        PlayerPrefs.SetString(NextSceneKey, sceneAfterGuide);
        PlayerPrefs.SetInt(GuideOverlayPendingKey, 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneAfterGuide);
    }

    public static string ConsumeNextSceneOrDefault(string defaultIfMissing)
    {
        if (!PlayerPrefs.HasKey(NextSceneKey))
            return defaultIfMissing;
        string next = PlayerPrefs.GetString(NextSceneKey, defaultIfMissing);
        PlayerPrefs.DeleteKey(NextSceneKey);
        PlayerPrefs.Save();
        return next;
    }

    /// <summary>Used by BaseLevel1 bootstrap only.</summary>
    public static bool TryBeginGuideOverlayOnThisLevel(string guideSceneName, GameObject mazeUiRoot)
    {
        if (PlayerPrefs.GetInt(GuideOverlayPendingKey, 0) != 1)
            return false;

        string expected = PlayerPrefs.GetString(NextSceneKey, "");
        if (!string.IsNullOrEmpty(expected) && expected != SceneManager.GetActiveScene().name)
            return false;

        PlayerPrefs.DeleteKey(GuideOverlayPendingKey);
        PlayerPrefs.Save();

        if (mazeUiRoot != null)
            mazeUiRoot.SetActive(false);

        if (!Application.CanStreamedLevelBeLoaded(guideSceneName))
        {
            Debug.LogError($"Guide scene '{guideSceneName}' is not in Build Settings.");
            if (mazeUiRoot != null)
                mazeUiRoot.SetActive(true);
            return false;
        }

        SceneManager.LoadSceneAsync(guideSceneName, LoadSceneMode.Additive);
        return true;
    }

    /// <summary>
    /// If Guide was loaded additively over <paramref name="nextSceneName"/>, enables maze UI and unloads Guide.
    /// Returns true if that path ran (no full scene reload).
    /// </summary>
    public static bool TryFinishGuideOverlay(string nextSceneName, string guideSceneName = "Guide")
    {
        if (SceneManager.sceneCount <= 1)
            return false;

        Scene guideScene = SceneManager.GetSceneByName(guideSceneName);
        if (!guideScene.isLoaded)
            return false;

        if (SceneManager.GetSceneAt(0).name != nextSceneName)
            return false;

        var bootstrap = Object.FindFirstObjectByType<GuideLevelBootstrap>();
        if (bootstrap != null)
            bootstrap.ShowMazeUiAfterGuide();

        SceneManager.UnloadSceneAsync(guideScene);
        return true;
    }

    /// <summary>Disables Main Camera and duplicate AudioListener in the Guide scene when it is stacked over the level.</summary>
    public static void DisableGuideWorldRenderingIfOverlay(Scene guideScene)
    {
        if (SceneManager.sceneCount <= 1)
            return;

        foreach (var cam in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            if (cam != null && cam.gameObject.scene == guideScene)
                cam.enabled = false;
        }

        foreach (var al in Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None))
        {
            if (al != null && al.gameObject.scene == guideScene)
                al.enabled = false;
        }
    }
}
