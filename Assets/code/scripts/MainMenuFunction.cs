using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuFunction : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private string level2SceneName = "Level2";

    private Button _startButton;
    private Button _settingsButton;
    private Button _level1Button;
    private Button _level2Button;
    private Button _backFromLevelsButton;
    private Button _backFromSettingsButton;
    private Slider _volumeSlider;
    private Slider _brightnessSlider;
    private VisualElement _mainMenuPanel;
    private VisualElement _levelSelectPanel;
    private VisualElement _settingsPanel;
    private VisualElement _brightnessOverlay;

    private bool _callbacksRegistered;
    private const string VolumePrefKey = "menu_volume";
    private const string BrightnessPrefKey = "menu_brightness";

    private void OnEnable()
    {
        if (document == null)
            document = GetComponent<UIDocument>();

        if (document == null)
        {
            Debug.LogError("MainMenuFunction requires a UIDocument on the same object or in the Document field.");
            return;
        }

        VisualElement root = document.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("MainMenuFunction could not read UIDocument rootVisualElement.");
            return;
        }

        _mainMenuPanel = root.Q<VisualElement>("MainMenu") ?? root.Q<VisualElement>("MainMenuPanel");
        _levelSelectPanel = root.Q<VisualElement>("levelSelect") ?? root.Q<VisualElement>("LevelSelectPanel");
        _settingsPanel = root.Q<VisualElement>("settingsPanel") ?? root.Q<VisualElement>("SettingsPanel");
        _brightnessOverlay = root.Q<VisualElement>("brightnessOverlay") ?? root.Q<VisualElement>("BrightnessOverlay");
        if (_brightnessOverlay != null) _brightnessOverlay.pickingMode = PickingMode.Ignore;

        _startButton = root.Q<Button>("startButton") ?? root.Q<Button>("Start");
        _settingsButton = root.Q<Button>("settingsButton") ?? root.Q<Button>("Settings");
        _level1Button = root.Q<Button>("level1Button") ?? root.Q<Button>("Level1Button");
        _level2Button = root.Q<Button>("level2Button") ?? root.Q<Button>("Level2Button");
        _backFromLevelsButton = root.Q<Button>("backFromLevelSelect") ?? root.Q<Button>("BackFromLevelsButton");
        _backFromSettingsButton = root.Q<Button>("backFromSettings") ?? root.Q<Button>("BackFromSettingsButton");
        _volumeSlider = root.Q<Slider>("volumeSlider") ?? root.Q<Slider>("VolumeSlider");
        _brightnessSlider = root.Q<Slider>("brightnessSlider") ?? root.Q<Slider>("BrightnessSlider");

        if (_startButton == null)
        {
            Debug.LogError("MainMenuFunction could not find start button in the UXML.");
            return;
        }

        ShowMainMenu();
        InitializeSettings();

        _startButton.clicked += OnPlayGameClick;
        if (_settingsButton != null)
            _settingsButton.clicked += OnSettingsClick;
        if (_level1Button != null)
            _level1Button.clicked += OnLevel1Click;
        if (_level2Button != null)
            _level2Button.clicked += OnLevel2Click;
        if (_backFromLevelsButton != null)
            _backFromLevelsButton.clicked += ShowMainMenu;
        if (_backFromSettingsButton != null)
            _backFromSettingsButton.clicked += ShowMainMenu;
        if (_volumeSlider != null)
            _volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
        if (_brightnessSlider != null)
            _brightnessSlider.RegisterValueChangedCallback(OnBrightnessChanged);

        _callbacksRegistered = true;
    }

    private void OnDisable()
    {
        if (!_callbacksRegistered)
            return;

        if (_startButton != null)
            _startButton.clicked -= OnPlayGameClick;
        if (_settingsButton != null)
            _settingsButton.clicked -= OnSettingsClick;
        if (_level1Button != null)
            _level1Button.clicked -= OnLevel1Click;
        if (_level2Button != null)
            _level2Button.clicked -= OnLevel2Click;
        if (_backFromLevelsButton != null)
            _backFromLevelsButton.clicked -= ShowMainMenu;
        if (_backFromSettingsButton != null)
            _backFromSettingsButton.clicked -= ShowMainMenu;
        if (_volumeSlider != null)
            _volumeSlider.UnregisterValueChangedCallback(OnVolumeChanged);
        if (_brightnessSlider != null)
            _brightnessSlider.UnregisterValueChangedCallback(OnBrightnessChanged);

        _callbacksRegistered = false;
    }

    private void OnPlayGameClick()
    {
        ShowLevelSelect();
    }

    private void OnSettingsClick()
    {
        ShowSettings();
    }

    private void OnLevel1Click()
    {
        LoadSceneIfAvailable("BaseLevel1");
    }

    private void OnLevel2Click()
    {
        LoadSceneIfAvailable(level2SceneName);
    }

    private void ShowMainMenu()
    {
        SetPanelVisible(_mainMenuPanel, true);
        SetPanelVisible(_levelSelectPanel, false);
        SetPanelVisible(_settingsPanel, false);
    }

    private void ShowLevelSelect()
    {
        SetPanelVisible(_mainMenuPanel, false);
        SetPanelVisible(_levelSelectPanel, true);
        SetPanelVisible(_settingsPanel, false);
    }

    private void ShowSettings()
    {
        SetPanelVisible(_mainMenuPanel, false);
        SetPanelVisible(_levelSelectPanel, false);
        SetPanelVisible(_settingsPanel, true);
    }

    private static void SetPanelVisible(VisualElement panel, bool visible)
    {
        if (panel != null)
            panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void InitializeSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 1f);
        float savedBrightness = PlayerPrefs.GetFloat(BrightnessPrefKey, 1f);

        AudioListener.volume = savedVolume;
        ApplyBrightness(savedBrightness);

        if (_volumeSlider != null) _volumeSlider.SetValueWithoutNotify(savedVolume);
        if (_brightnessSlider != null) _brightnessSlider.SetValueWithoutNotify(savedBrightness);
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        AudioListener.volume = evt.newValue;
        PlayerPrefs.SetFloat(VolumePrefKey, evt.newValue);
    }

    private void OnBrightnessChanged(ChangeEvent<float> evt)
    {
        ApplyBrightness(evt.newValue);
        PlayerPrefs.SetFloat(BrightnessPrefKey, evt.newValue);
    }

    private void ApplyBrightness(float brightness)
    {
        if (_brightnessOverlay == null)
            return;

        float alpha = Mathf.Clamp01(1f - brightness) * 0.55f;
        _brightnessOverlay.style.backgroundColor = new Color(0f, 0f, 0f, alpha);
    }

    private void LoadSceneIfAvailable(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Scene name is empty.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is not in Build Settings or cannot be loaded.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
