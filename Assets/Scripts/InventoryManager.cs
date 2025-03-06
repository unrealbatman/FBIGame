using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory")]
    [SerializeField]
    private List<EvidenceData> collectedEvidence = new List<EvidenceData>();

    public static event Action<EvidenceData,GameObject> OnEvidenceAdded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject); // Optional: Makes InventoryManager persist across scenes
    }

    public void AddEvidenceToInventory(EvidenceData evidenceData,GameObject evidenceInfoCanvas)
    {
        if (evidenceData == null)
        {
            Debug.LogWarning("Attempted to add null evidence to inventory.");
            return;
        }

        if (collectedEvidence.Contains(evidenceData))
        {
            Debug.LogWarning($"Evidence '{evidenceData.clueName}' is already collected.");
            return;
        }

        collectedEvidence.Add(evidenceData);
        Debug.Log($"Evidence added to inventory: {evidenceData.clueName}");

        OnEvidenceAdded?.Invoke(evidenceData, evidenceInfoCanvas);
    }

    public IReadOnlyList<EvidenceData> GetCollectedEvidence()
    {
        return collectedEvidence.AsReadOnly(); // Returns a read-only collection to prevent external modification
    }
}
