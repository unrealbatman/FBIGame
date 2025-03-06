using System.Collections;
using UnityEngine;

public class Evidence : MonoBehaviour, IExaminable
{
    [Header("Evidence Details")]
    public string clueName; 
    public string clueDescription; 
    public Sprite clueIcon; 

    [Header("Examination Settings")]
    [SerializeField] private float examinationDelay = 2f; 
    [SerializeField] private float vibrationIntensity = 0.1f; 
    [SerializeField] private float vibrationSpeed = 20f;

    [SerializeField] private GameObject evidenceInfoCanvas;

    private bool isVibrating = false; 
    private Vector3 originalPosition; 

    [SerializeField] private GameObject examineFX; 

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

        yield return new WaitForSeconds(examinationDelay);

        // Add the evidence to the inventory
        var evidenceData = new EvidenceData(clueName, clueDescription, clueIcon);
        InventoryManager.Instance.AddEvidenceToInventory(evidenceData, evidenceInfoCanvas);
        LevelManager.Instance.CollectEvidence(); // Notify the level manager of the collection

        Debug.Log($"Evidence '{clueName}' added to inventory after {examinationDelay} seconds.");

        isVibrating = false; // Stop vibration

        //Destroy(gameObject);
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
