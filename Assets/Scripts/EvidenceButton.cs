using System;
using UnityEngine;

public class EvidenceButton : MonoBehaviour
{
    [Header("Evidence Details")]
    public string clueName;
    public string clueDescription;
    public Sprite clueIcon;

    private EvidenceData evidenceData;

    public static event Action<EvidenceData> OnButton;

    public void Initialize(EvidenceData data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "Evidence data cannot be null.");
        }

        evidenceData = data;
    }

    public void OnButtonClick()
    {
        if (evidenceData == null)
        {
            Debug.LogWarning("Evidence data is null. Cannot invoke OnButton event.");
            return;
        }

        OnButton?.Invoke(evidenceData);
    }
}
