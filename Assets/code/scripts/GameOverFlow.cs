using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameOverFlow
{
    private const string EndSceneName = "Endscreen";
    private static bool _triggered; // only one end transition per run
    private static bool _playerWon;
    private static string _reason;

    public static bool PlayerWon => _playerWon;
    public static string Reason => _reason;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState() // editor play mode resets statics cleanly
    {
        _triggered = false;
        _playerWon = false;
        _reason = string.Empty;
    }

    public static void TriggerWin(string reason)
    {
        Trigger(true, reason);
    }

    public static void TriggerLose(string reason)
    {
        Trigger(false, reason);
    }

    public static void Clear() // before loading next scene so end can fire again later
    {
        _triggered = false;
        _playerWon = false;
        _reason = string.Empty;
    }

    private static void Trigger(bool playerWon, string reason) // loads Endscreen once
    {
        if (_triggered)
            return;

        if (!Application.CanStreamedLevelBeLoaded(EndSceneName))
        {
            Debug.LogError($"[GameOverFlow] Cannot load '{EndSceneName}'. Add it to Build Settings. Reason: {reason}");
            return;
        }

        _triggered = true;
        _playerWon = playerWon;
        _reason = reason;
        Debug.Log($"[GameOverFlow] Triggered: {reason}");

        // Keep BaseLevel1 visible behind the end UI (dark overlay + buttons).
        if (SceneManager.GetActiveScene().name == "BaseLevel1")
            SceneManager.LoadScene(EndSceneName, LoadSceneMode.Additive);
        else
            SceneManager.LoadScene(EndSceneName, LoadSceneMode.Single);
    }
}
