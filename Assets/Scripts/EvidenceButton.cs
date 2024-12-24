using System;
using UnityEngine;

public class EvidenceButton : MonoBehaviour
{
    [Header("Evidence Details")]
    public string clueName; // The name of the evidence item.
    public string clueDescription; // A description of the evidence.
    public Sprite clueIcon; // The icon representing the evidence.

    private EvidenceData evidenceData; // Holds the associated evidence data for this button.

    // Event triggered when the button is clicked. Passes the EvidenceData as a parameter.
    public static event Action<EvidenceData> OnButton;

    /// <summary>
    /// Initializes the evidence button with the provided evidence data.
    /// This method should be called to set up the button before interaction.
    /// </summary>
    /// <param name="data">The evidence data to associate with this button.</param>
    public void Initialize(EvidenceData data)
    {
        // Ensure that the provided data is not null.
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "Evidence data cannot be null.");
        }

        evidenceData = data; // Assign the evidence data to the instance variable.
    }

    /// <summary>
    /// Invokes the OnButton event when this button is clicked.
    /// This method is tied to the button's click action.
    /// </summary>
    public void OnButtonClick()
    {
        // Check if evidence data is properly initialized.
        if (evidenceData == null)
        {
            Debug.LogWarning("Evidence data is null. Cannot invoke OnButton event.");
            return;
        }

        // Trigger the OnButton event, passing the associated evidence data.
        OnButton?.Invoke(evidenceData);
    }
}
