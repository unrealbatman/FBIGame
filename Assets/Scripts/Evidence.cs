using System.Collections;
using UnityEngine;

public class Evidence : MonoBehaviour, IExaminable
{
    [Header("Evidence Details")]
    public string clueName; // Name of the evidence
    public string clueDescription; // Description of the evidence
    public Sprite clueIcon; // Icon representing the evidence

    [Header("Examination Settings")]
    [SerializeField] private float examinationDelay = 2f; // Delay in seconds before adding evidence to the inventory
    [SerializeField] private float vibrationIntensity = 0.1f; // Intensity of vibration
    [SerializeField] private float vibrationSpeed = 20f; // Speed of vibration

    private bool isVibrating = false; // Indicates whether the evidence is vibrating
    private Vector3 originalPosition; // Stores the original position of the evidence

    [SerializeField] private GameObject ExamineFX;

    private void Start()
    {
        // Cache the original position
        originalPosition = transform.localPosition;
        ExamineFX.SetActive(false);
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
    /// Coroutine to manage the examination process with vibration and destruction.
    /// </summary>
    private IEnumerator HandleExamination()
    {
        // Start vibrating during zoom-in
        isVibrating = true;
        StartCoroutine(Vibrate());

        ExamineFX.SetActive(true);
        ExamineFX.GetComponent<ParticleSystem>().Play();
        // Wait for the specified delay
        yield return new WaitForSeconds(examinationDelay);


        // Create evidence data and add it to the inventory
        EvidenceData evidenceData = new EvidenceData(clueName, clueDescription, clueIcon);
        InventoryManager.Instance.AddEvidenceToInventory(evidenceData);

        Debug.Log($"Evidence {clueName} added to inventory after {examinationDelay} seconds.");

        // Stop vibrating when zoom-out starts
        isVibrating = false;

        /*// Wait for camera to complete zoom-out (adjust as needed)
        yield return new WaitForSeconds(0.5f);*/

        
       
        // Destroy the evidence object
        Destroy(gameObject);
    }

    /// <summary>
    /// Vibrates the evidence object while zooming in.
    /// </summary>
    private IEnumerator Vibrate()
    {
        while (isVibrating)
        {
            float offsetX = Mathf.Sin(Time.time * vibrationSpeed) * vibrationIntensity;
            float offsetY = Mathf.Cos(Time.time * vibrationSpeed) * vibrationIntensity;
            float offsetZ = Mathf.Sin(Time.time * vibrationSpeed * 0.5f) * vibrationIntensity;

            // Apply the vibration offset to the object's position
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, offsetZ);

            yield return null;
        }

        // Reset position when vibration stops
        transform.localPosition = originalPosition;
    }
}
