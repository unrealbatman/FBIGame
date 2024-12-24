using System.Collections;
using UnityEngine;

public class Evidence : MonoBehaviour, IExaminable
{
    [Header("Evidence Details")]
    public string clueName; // Name of the evidence
    public string clueDescription; // Description of the evidence
    public Sprite clueIcon; // Icon representing the evidence

    [Header("Examination Settings")]
    [SerializeField] private float examinationDelay = 2f; // Delay before adding evidence to the inventory
    [SerializeField] private float vibrationIntensity = 0.1f; // Intensity of vibration effect
    [SerializeField] private float vibrationSpeed = 20f; // Speed at which the vibration occurs

    private bool isVibrating = false; // Flag indicating if the evidence is vibrating
    private Vector3 originalPosition; // Original position of the evidence object

    [SerializeField] private GameObject examineFX; // Effect to play during examination (e.g., particles)

    private void Start()
    {
        // Cache the original position of the evidence object
        originalPosition = transform.localPosition;

        // Ensure the examination effect (if any) is initially inactive
        if (examineFX != null)
        {
            examineFX.SetActive(false);
        }
    }

    /// <summary>
    /// Starts the examination process when the evidence is interacted with.
    /// </summary>
    public void Interact()
    {
        Debug.Log($"Examining Evidence: {clueName}");
        StartCoroutine(HandleExamination());
    }

    /// <summary>
    /// Handles the examination process, including vibration and adding evidence to inventory.
    /// </summary>
    private IEnumerator HandleExamination()
    {
        isVibrating = true; // Start the vibration effect
        StartCoroutine(Vibrate()); // Vibrate the evidence object

        // Activate and play the examination effect (if it exists)
        if (examineFX != null)
        {
            examineFX.SetActive(true);
            ParticleSystem particleSystem = examineFX.GetComponent<ParticleSystem>();
            particleSystem?.Play();
        }

        // Wait for the specified examination delay before proceeding
        yield return new WaitForSeconds(examinationDelay);

        // Add the evidence to the inventory
        var evidenceData = new EvidenceData(clueName, clueDescription, clueIcon);
        InventoryManager.Instance.AddEvidenceToInventory(evidenceData);
        LevelManager.Instance.CollectEvidence(); // Notify the level manager of the collection

        Debug.Log($"Evidence '{clueName}' added to inventory after {examinationDelay} seconds.");

        isVibrating = false; // Stop vibration

        // Destroy the evidence object from the scene
        Destroy(gameObject);
    }

    /// <summary>
    /// Vibrates the evidence object while it is being examined.
    /// </summary>
    private IEnumerator Vibrate()
    {
        while (isVibrating)
        {
            // Calculate vibration offsets in different directions using sine and cosine functions
            float offsetX = Mathf.Sin(Time.time * vibrationSpeed) * vibrationIntensity;
            float offsetY = Mathf.Cos(Time.time * vibrationSpeed) * vibrationIntensity;
            float offsetZ = Mathf.Sin(Time.time * vibrationSpeed * 0.5f) * vibrationIntensity;

            // Apply the vibration offset to the evidence's position
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, offsetZ);

            yield return null; // Wait for the next frame
        }

        // Reset the position of the evidence object after vibration stops
        transform.localPosition = originalPosition;
    }
}
