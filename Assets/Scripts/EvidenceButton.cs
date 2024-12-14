using System;
using UnityEngine;

public class EvidenceButton : MonoBehaviour
{
    [Header("Evidence Details")]
    public string clueName; // Name of the evidence
    public string clueDescription; // Description of the evidence
    public Sprite clueIcon; // Icon representing the evidence

    private EvidenceData evidenceData; // Associated evidence data
    private UIController uiController; // Reference to the UIController

    // Event triggered when the button is clicked
    public static event Action<EvidenceData> OnButton;

    /// <summary>
    /// Initializes the evidence button with the provided data and UI controller.
    /// </summary>
    /// <param name="data">The evidence data associated with this button.</param>
    /// <param name="controller">Reference to the UIController managing this button.</param>
    public void Initialize(EvidenceData data, UIController controller)
    {
        evidenceData = data ?? throw new ArgumentNullException(nameof(data), "Evidence data cannot be null.");
        uiController = controller ?? throw new ArgumentNullException(nameof(controller), "UIController cannot be null.");
    }

    /// <summary>
    /// Invokes the OnButton event when this button is clicked.
    /// </summary>
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
