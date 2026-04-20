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
    [SerializeField] private string winButtonText = "Return to Menu";

    [SerializeField] private float menuStyleButtonMinWidth = 420f;
    [SerializeField] private float menuStyleButtonMinHeight = 82f;

    private Button _button;
    private TMP_Text _titleLabel;
    private TMP_Text _buttonLabel;

    void Start()
    {
        _button = GetComponent<Button>();
        CacheUiReferences();
        ApplyEndStateText();
        ApplyMainMenuStyleButton();

        if (_button != null)
        {
            _button.onClick.RemoveListener(HandleButtonClick);
            _button.onClick.AddListener(HandleButtonClick);
        }
    }

    /// <summary>Matches MainMenuLabel.uss .button: rgba backgrounds, white bold 32px label (LiberationSans path in USS).</summary>
    void ApplyMainMenuStyleButton()
    {
        if (_button == null)
            return;

        var rt = _button.GetComponent<RectTransform>();
        if (rt != null)
        {
            var sd = rt.sizeDelta;
            sd.x = Mathf.Max(sd.x, menuStyleButtonMinWidth);
            sd.y = Mathf.Max(sd.y, menuStyleButtonMinHeight);
            rt.sizeDelta = sd;
        }

        // ColorTint multiplies into the target graphic — keep Image white so tints match USS rgba values.
        var img = _button.GetComponent<Image>();
        if (img != null)
            img.color = Color.white;

        var cb = _button.colors;
        cb.fadeDuration = 0.18f;
        cb.colorMultiplier = 1f;
        cb.normalColor = new Color(33f / 255f, 46f / 255f, 91f / 255f, 0.72f);
        cb.highlightedColor = new Color(56f / 255f, 74f / 255f, 136f / 255f, 0.95f);
        cb.pressedColor = new Color(26f / 255f, 37f / 255f, 77f / 255f, 0.95f);
        cb.selectedColor = cb.highlightedColor;
        cb.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        _button.colors = cb;

        if (_buttonLabel != null)
        {
            _buttonLabel.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
            _buttonLabel.fontSize = 32;
            _buttonLabel.fontStyle = TMPro.FontStyles.Bold;
            _buttonLabel.color = Color.white;
            _buttonLabel.enableAutoSizing = false;
        }

        if (_titleLabel != null)
            _titleLabel.color = Color.white;
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
        bool playerWon = GameOverFlow.PlayerWon;
        GameOverFlow.Clear();

        string targetScene = playerWon ? mainMenuSceneName : retrySceneName;
        if (!Application.CanStreamedLevelBeLoaded(targetScene))
        {
            Debug.LogError($"[buttonrestart] Cannot load scene '{targetScene}'. Add it to Build Settings.");
            return;
        }

        SceneManager.LoadScene(targetScene);
    }
}
