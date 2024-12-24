using UnityEngine;
using System;

/// <summary>
/// Manages level-specific functionalities such as tracking evidence collection.
/// Implements a singleton pattern for global access within the level.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Evidence Settings")]
    [SerializeField] private int totalEvidences; // Total number of evidences in the level, set via the Inspector.
    private int collectedEvidences; // Tracks the number of evidences collected.

    // Event to notify subscribers of evidence collection progress updates.
    public static event Action<int, int> OnEvidenceProgressUpdated;

    /// <summary>
    /// Singleton instance of the LevelManager.
    /// Ensures only one instance exists at a time.
    /// </summary>
    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of LevelManager exists.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Optional: Uncomment the line below to persist the LevelManager across scenes.
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeLevel();
    }

    /// <summary>
    /// Initializes level data such as resetting evidence collection progress.
    /// </summary>
    private void InitializeLevel()
    {
        collectedEvidences = 0; // Reset collected evidences at the start of the level.
        NotifyEvidenceProgress(); // Notify UI and other systems of the initial progress.
    }

    /// <summary>
    /// Updates the collected evidence count and notifies subscribers of the change.
    /// </summary>
    public void CollectEvidence()
    {
        collectedEvidences++; // Increment collected evidences count.
        NotifyEvidenceProgress(); // Notify subscribers of the updated progress.
    }

    /// <summary>
    /// Invokes the evidence progress update event with the current and total evidence counts.
    /// </summary>
    private void NotifyEvidenceProgress()
    {
        OnEvidenceProgressUpdated?.Invoke(collectedEvidences, totalEvidences);
    }
}
