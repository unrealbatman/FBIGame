using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory")]
    [SerializeField]
    private List<EvidenceData> collectedEvidence = new List<EvidenceData>();

    public static event Action<EvidenceData> OnEvidenceAdded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddEvidenceToInventory(EvidenceData evidenceData)
    {
        if (evidenceData == null)
        {
            Debug.LogWarning("Attempted to add null evidence to inventory.");
            return;
        }

        collectedEvidence.Add(evidenceData);
        Debug.Log($"Evidence added to inventory: {evidenceData.clueName}");

        OnEvidenceAdded?.Invoke(evidenceData);
    }

    public List<EvidenceData> GetCollectedEvidence()
    {
        return new List<EvidenceData>(collectedEvidence);
    }
}
