using System;
using System.Collections.Generic;
using UnityEngine;
using static Evidence;

public class InventoryManager : MonoBehaviour
{
    // Singleton instance for global access
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory")]
    [SerializeField]
    private List<EvidenceData> collectedEvidence = new List<EvidenceData>(); // List of collected evidence data

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
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    /// <summary>
    /// Adds evidence data to the player's inventory.
    /// </summary>
    /// <param name="evidenceData">The evidence data to add.</param>
    public void AddEvidenceToInventory(EvidenceData evidenceData)
    {
        if (evidenceData == null) return;

        // Add evidence data to the inventory
        collectedEvidence.Add(evidenceData);
        Debug.Log($"Evidence added to inventory: {evidenceData.clueName}");

        // Trigger the EvidenceAdded event to notify listeners (e.g., UI updates)
        OnEvidenceAdded?.Invoke(evidenceData);
    }

    /// <summary>
    /// Retrieves the list of collected evidence data in the inventory.
    /// </summary>
    /// <returns>List of evidence data.</returns>
    public List<EvidenceData> GetCollectedEvidence()
    {
        return collectedEvidence;
    }
}
