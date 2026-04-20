using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// End screen: reads win/lose from GameOverFlow, swaps TMP labels, then loads menu or retry
public class buttonrestart : MonoBehaviour
{
    [SerializeField] private string retrySceneName = "BaseLevel1";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string loseTitleText = "Game Over";
    [SerializeField] private string winTitleText = "You Win !";
    [SerializeField] private string loseButtonText = "Try Again";
    [SerializeField] private string winButtonText = "Return to Main Menu";

    private Button _button;
    private TMP_Text _titleLabel;
    private TMP_Text _buttonLabel;

    void Start()
    {
        _button = GetComponent<Button>();
        CacheUiReferences();
        ApplyEndStateText();

        if (_button != null)
        {
            _button.onClick.RemoveListener(HandleButtonClick);
            _button.onClick.AddListener(HandleButtonClick);
        }
    }

    private void CacheUiReferences() // button caption vs big title on canvas
    {
        _buttonLabel = GetComponentInChildren<TMP_Text>(true);

        if (_titleLabel != null)
            return;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        TMP_Text[] labels = canvas.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i] == null || labels[i] == _buttonLabel)
                continue;

            if (labels[i].text.Contains("Game Over"))
            {
                _titleLabel = labels[i];
                return;
            }
        }

        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i] == null || labels[i] == _buttonLabel)
                continue;

            _titleLabel = labels[i];
            return;
        }
    }

    private void ApplyEndStateText()
    {
        bool playerWon = GameOverFlow.PlayerWon;

        if (_titleLabel != null)
            _titleLabel.text = playerWon ? winTitleText : loseTitleText;

        if (_buttonLabel != null)
            _buttonLabel.text = playerWon ? winButtonText : loseButtonText;
    }

    private void HandleButtonClick()
    {
        bool playerWon = GameOverFlow.PlayerWon; // from TriggerWin / TriggerLose when level ended
        string targetScene = playerWon ? mainMenuSceneName : retrySceneName;

        if (!Application.CanStreamedLevelBeLoaded(targetScene))
        {
            Debug.LogError($"[buttonrestart] Cannot load scene '{targetScene}'. Add it to Build Settings.");
            return;
        }

        GameOverFlow.Clear();
        SceneManager.LoadScene(targetScene);
    }
}
