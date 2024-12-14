using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton instance for global access
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory")]
    [SerializeField] private List<EvidenceData> collectedEvidence = new List<EvidenceData>();

    // Event triggered when new evidence is added
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
            Destroy(gameObject);
            return;
        }

        // Optional: Persist the instance across scenes
        // DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Adds evidence data to the player's inventory and notifies listeners.
    /// </summary>
    /// <param name="evidenceData">The evidence data to add.</param>
    public void AddEvidenceToInventory(EvidenceData evidenceData)
    {
        if (evidenceData == null)
        {
            Debug.LogWarning("Attempted to add null evidence to inventory.");
            return;
        }

        // Add evidence data to the inventory
        collectedEvidence.Add(evidenceData);
        Debug.Log($"Evidence added to inventory: {evidenceData.clueName}");

        // Trigger the EvidenceAdded event to notify listeners (e.g., UI updates)
        OnEvidenceAdded?.Invoke(evidenceData);
    }

    /// <summary>
    /// Retrieves the list of collected evidence data.
    /// </summary>
    /// <returns>List of collected evidence data.</returns>
    public List<EvidenceData> GetCollectedEvidence()
    {
        return new List<EvidenceData>(collectedEvidence);
    }
}
