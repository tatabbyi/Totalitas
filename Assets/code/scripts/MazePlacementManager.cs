using UnityEngine;
using Unity.AI.Navigation;
using System;

public class MazePlacementManager : MonoBehaviour
{
    [SerializeField] private Transform placementAnchor;
    [SerializeField] private MazePreset[] availablePresets;
    [SerializeField] private NavMeshSurface navMeshSurface;

    private GameObject _currentMazeInstance;
    private bool _placementLocked;
    private MazePreset _selectedPreset;

    public MazePreset[] AvailablePresets => availablePresets;
    public bool HasPlacedPreset => _currentMazeInstance != null;
    public bool RunStarted { get; private set; }
    public MazePreset SelectedPreset => _selectedPreset;

    public event Action OnPresetPlaced;
    public event Action OnRunStarted;

    private void Awake()
    {
        RunStarted = false;
        _placementLocked = false;
        _currentMazeInstance = null;
        _selectedPreset = null;
    }

    public bool TryPlacePreset(MazePreset preset)
    {
        if (RunStarted || _placementLocked || preset == null || preset.mazePrefab == null || placementAnchor == null)
            return false;

        if (_currentMazeInstance != null)
            Destroy(_currentMazeInstance);

        _currentMazeInstance = Instantiate(
            preset.mazePrefab,
            placementAnchor.position,
            placementAnchor.rotation
        );

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();

        _selectedPreset = preset;
        OnPresetPlaced?.Invoke();
        return true;
    }

    public bool TryStartRun()
    {
        if (RunStarted || !HasPlacedPreset)
            return false;

        RunStarted = true;
        LockPlacement();
        OnRunStarted?.Invoke();
        return true;
    }

    public void LockPlacement()
    {
        _placementLocked = true;
    }

    public void UnlockPlacement()
    {
        _placementLocked = false;
    }
}
