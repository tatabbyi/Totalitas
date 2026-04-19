using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameOverFlow
{
    private const string EndSceneName = "Endscreen";
    private static bool _triggered;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        _triggered = false;
    }

    public static void Trigger(string reason)
    {
        if (_triggered)
            return;

        if (!Application.CanStreamedLevelBeLoaded(EndSceneName))
        {
            Debug.LogError($"[GameOverFlow] Cannot load '{EndSceneName}'. Add it to Build Settings. Reason: {reason}");
            return;
        }

        _triggered = true;
        Debug.Log($"[GameOverFlow] Triggered: {reason}");
        SceneManager.LoadScene(EndSceneName);
    }
}
