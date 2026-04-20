using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Runs early on BaseLevel1: if the menu queued a guide overlay, hides maze preset UI and loads Guide additively
/// so the real level renders behind the guide. Call <see cref="ShowMazeUiAfterGuide"/> when Guide ends.
/// </summary>
[DefaultExecutionOrder(-500)]
public class GuideLevelBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject mazePresetUiRoot;
    [SerializeField] private string guideSceneName = "Guide";

    private void Awake()
    {
        GuideFlow.TryBeginGuideOverlayOnThisLevel(guideSceneName, mazePresetUiRoot);
    }

    public void ShowMazeUiAfterGuide()
    {
        if (mazePresetUiRoot != null)
            mazePresetUiRoot.SetActive(true);
    }
}
