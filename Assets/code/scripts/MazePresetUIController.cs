using UnityEngine;
using UnityEngine.UIElements;

public class MazePresetUIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private MazePlacementManager placementManager;

    private Button[] _buttons = new Button[3];
    private VisualElement[] _images = new VisualElement[3];
    private Label[] _labels = new Label[3];
    private Button _startButton;
    private Label _statusLabel;
    private int _selectedIndex = -1;

    private void Awake()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null || placementManager == null)
        {
            Debug.LogError("MazePresetUIController requires UIDocument and MazePlacementManager references.");
            return;
        }

        var root = uiDocument.rootVisualElement;

        for (int i = 0; i < 3; i++)
        {
            _buttons[i] = root.Q<Button>($"PresetButton{i}");
            _images[i] = root.Q<VisualElement>($"PresetImage{i}");
            _labels[i] = root.Q<Label>($"PresetLabel{i}");
        }

        _startButton = root.Q<Button>("StartRunButton");
        _statusLabel = root.Q<Label>("BuildStatusLabel");

        BindPresets();
        BindStartButton();
        UpdateSelectionVisuals();
        UpdateStartButtonState();
        UpdateStatusText();

        placementManager.OnRunStarted += OnRunStarted;
    }

    private void BindPresets()
    {
        var presets = placementManager.AvailablePresets;

        for (int i = 0; i < 3; i++)
        {
            int idx = i;

            if (presets != null && idx < presets.Length && presets[idx] != null)
            {
                var preset = presets[idx];
                if (_buttons[idx] == null || _labels[idx] == null || _images[idx] == null)
                    continue;

                _labels[idx].text = preset.displayName;

                if (preset.previewImage != null)
                    _images[idx].style.backgroundImage = new StyleBackground(preset.previewImage);

                _buttons[idx].clicked += () => OnPresetClicked(idx, preset);
                _buttons[idx].SetEnabled(true);
            }
            else
            {
                if (_buttons[idx] == null || _labels[idx] == null)
                    continue;

                _labels[idx].text = "Empty";
                _buttons[idx].SetEnabled(false);
            }
        }
    }

    private void BindStartButton()
    {
        if (_startButton == null)
            return;

        _startButton.clicked += OnStartRunClicked;
    }

    private void OnPresetClicked(int presetIndex, MazePreset preset)
    {
        if (placementManager.TryPlacePreset(preset))
        {
            _selectedIndex = presetIndex;
            UpdateSelectionVisuals();
            UpdateStartButtonState();
            UpdateStatusText();
        }
    }

    private void OnStartRunClicked()
    {
        if (!placementManager.TryStartRun())
            return;

        UpdateStartButtonState();
        UpdateStatusText();
    }

    private void OnRunStarted()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i] != null)
                _buttons[i].SetEnabled(false);
        }

        UpdateStartButtonState();
        UpdateStatusText();
    }

    private void UpdateSelectionVisuals()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i] == null)
                continue;

            if (i == _selectedIndex)
                _buttons[i].AddToClassList("selected");
            else
                _buttons[i].RemoveFromClassList("selected");
        }
    }

    private void UpdateStartButtonState()
    {
        if (_startButton == null)
            return;

        _startButton.SetEnabled(placementManager.HasPlacedPreset && !placementManager.RunStarted);
    }

    private void UpdateStatusText()
    {
        if (_statusLabel == null)
            return;

        if (placementManager.RunStarted)
            _statusLabel.text = "Wave running.";
        else if (!placementManager.HasPlacedPreset)
            _statusLabel.text = "Select a maze preset, then press Start.";
        else
            _statusLabel.text = "Preset selected. Press Start.";
    }

    private void OnDestroy()
    {
        if (placementManager != null)
            placementManager.OnRunStarted -= OnRunStarted;
    }
}
