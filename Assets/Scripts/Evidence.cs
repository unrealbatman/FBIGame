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
    [SerializeField] private float vibrationIntensity = 0.1f; // Intensity of vibration
    [SerializeField] private float vibrationSpeed = 20f; // Speed of vibration

    private bool isVibrating = false; // Indicates if the evidence is vibrating
    private Vector3 originalPosition; // Original position of the evidence

    [SerializeField] private GameObject examineFX; // Effect to play during examination

    private void Start()
    {
        // Cache the original position
        originalPosition = transform.localPosition;

        // Ensure the examination effect is initially inactive
        if (examineFX != null)
        {
            examineFX.SetActive(false);
        }
    }

    /// <summary>
    /// Starts the examination process for the evidence.
    /// </summary>
    public void Interact()
    {
        Debug.Log($"Examining Evidence: {clueName}");
        StartCoroutine(HandleExamination());
    }

    /// <summary>
    /// Manages the examination process with vibration and inventory addition.
    /// </summary>
    private IEnumerator HandleExamination()
    {
        isVibrating = true;
        StartCoroutine(Vibrate());

        // Activate and play the examination effect
        if (examineFX != null)
        {
            examineFX.SetActive(true);
            ParticleSystem particleSystem = examineFX.GetComponent<ParticleSystem>();
            particleSystem?.Play();
        }

        // Wait for the examination delay
        yield return new WaitForSeconds(examinationDelay);

        // Add the evidence to the inventory
        var evidenceData = new EvidenceData(clueName, clueDescription, clueIcon);
        InventoryManager.Instance.AddEvidenceToInventory(evidenceData);

        Debug.Log($"Evidence '{clueName}' added to inventory after {examinationDelay} seconds.");

        isVibrating = false;

        // Destroy the evidence object
        Destroy(gameObject);
    }

    /// <summary>
    /// Vibrates the evidence object while being examined.
    /// </summary>
    private IEnumerator Vibrate()
    {
        while (isVibrating)
        {
            float offsetX = Mathf.Sin(Time.time * vibrationSpeed) * vibrationIntensity;
            float offsetY = Mathf.Cos(Time.time * vibrationSpeed) * vibrationIntensity;
            float offsetZ = Mathf.Sin(Time.time * vibrationSpeed * 0.5f) * vibrationIntensity;

            // Apply the vibration offset
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, offsetZ);

            yield return null;
        }

        // Reset position when vibration stops
        transform.localPosition = originalPosition;
    }
}
