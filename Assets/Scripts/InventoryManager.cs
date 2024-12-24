using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's inventory, specifically evidence data collected during gameplay.
/// Implements a singleton pattern for global access.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    // Singleton instance for global access
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory")]
    [SerializeField]
    private List<EvidenceData> collectedEvidence = new List<EvidenceData>(); // Stores collected evidence data.

    // Event triggered when new evidence is added to the inventory
    public static event Action<EvidenceData> OnEvidenceAdded;

    private void Awake()
    {
        // Ensure only one instance of InventoryManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
            return;
        }

        // Optional: Uncomment the line below to persist the instance across scenes.
        // DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Adds evidence data to the player's inventory and notifies listeners via an event.
    /// </summary>
    /// <param name="evidenceData">The evidence data to add.</param>
    public void AddEvidenceToInventory(EvidenceData evidenceData)
    {
        if (evidenceData == null)
        {
            Debug.LogWarning("Attempted to add null evidence to inventory."); // Log a warning for invalid input.
            return;
        }

        // Add the evidence data to the inventory
        collectedEvidence.Add(evidenceData);
        Debug.Log($"Evidence added to inventory: {evidenceData.clueName}");

        // Notify subscribers (e.g., UI) of the new evidence
        OnEvidenceAdded?.Invoke(evidenceData);
    }

    /// <summary>
    /// Retrieves a copy of the list of collected evidence data.
    /// </summary>
    /// <returns>A new list containing the collected evidence data.</returns>
    public List<EvidenceData> GetCollectedEvidence()
    {
        return new List<EvidenceData>(collectedEvidence); // Return a copy to preserve encapsulation.
    }
}
