using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Loading Bar References")]
    [SerializeField] private Image loadingBar; // Reference to the loading bar UI
    [SerializeField] private RectTransform fxHolder; // Rotating FX holder for visual effects
    [SerializeField] private TextMeshProUGUI loadingProgressText; // Displays loading progress as text
    [SerializeField] private GameObject crosshair; // Crosshair UI element

    [Header("FX Settings")]
    [SerializeField] private ParticleSystem loadingFXParticle; // Particle effect during loading

    [Header("Evidence Popup References")]
    [SerializeField] private GameObject evidencePopup; // Popup for displaying evidence details
    [SerializeField] private Image evidenceIcon; // Icon for the evidence
    [SerializeField] private TextMeshProUGUI evidenceNameText; // Name of the evidence
    [SerializeField] private TextMeshProUGUI evidenceDescriptionText; // Description of the evidence

    private RawImage crosshairImage; // Cached reference to the crosshair's RawImage component

    private void Awake()
    {
        // Cache the RawImage component of the crosshair
        crosshairImage = crosshair.GetComponent<RawImage>();
        if (crosshairImage == null)
        {
            Debug.LogWarning("Crosshair does not have a RawImage component!");
        }
    }

    private void OnEnable()
    {
        // Subscribe to loading and inventory events
        LoadingManager.OnLoadingProgress += UpdateLoadingBar;
        LoadingManager.OnLoadingComplete += HandleLoadingComplete;
        LoadingManager.OnLoadingCancelled += HandleLoadingCancelled;
        InventoryManager.OnEvidenceAdded += ShowEvidencePopup;
    }

    private void OnDisable()
    {
        // Unsubscribe from loading and inventory events
        LoadingManager.OnLoadingProgress -= UpdateLoadingBar;
        LoadingManager.OnLoadingComplete -= HandleLoadingComplete;
        LoadingManager.OnLoadingCancelled -= HandleLoadingCancelled;
        InventoryManager.OnEvidenceAdded -= ShowEvidencePopup;
    }

    /// <summary>
    /// Displays a popup with evidence details.
    /// </summary>
    private void ShowEvidencePopup(EvidenceData evidence)
    {
        evidenceIcon.sprite = evidence.clueIcon;
        evidenceNameText.text = evidence.clueName;
        evidenceDescriptionText.text = evidence.clueDescription;
        evidencePopup.SetActive(true);

        // Lock the camera
        PlayerController.isCameraLocked = true;

        // Optionally unlock the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Hides the evidence popup.
    /// </summary>
    public void HideEvidencePopup()
    {
        evidencePopup.SetActive(false);
        
        // Lock the camera
        PlayerController.isCameraLocked = false;

        // Optionally unlock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Updates the loading bar and associated UI elements.
    /// </summary>
    private void UpdateLoadingBar(float progress)
    {
        // Update loading bar fill amount
        loadingBar.GetComponentsInChildren<Image>()[1].fillAmount = progress;

        // Update progress text
        loadingProgressText.text = Mathf.Floor(progress * 100).ToString();

        // Activate loading UI elements
        ToggleLoadingUI(true);

        // Rotate FX holder for visual feedback
        fxHolder.rotation = Quaternion.Euler(new Vector3(0f, 0f, -progress * 360));

        // Play loading particle effect if not already playing
        if (!loadingFXParticle.isPlaying)
        {
            loadingFXParticle.Play();
        }

        // Make crosshair transparent during loading
        SetCrosshairVisibility(0f);
    }

    /// <summary>
    /// Handles loading completion by hiding the loading bar and stopping effects.
    /// </summary>
    private void HandleLoadingComplete()
    {
        // Deactivate loading UI elements
        ToggleLoadingUI(false);

        // Stop loading particle effect
        loadingFXParticle.Stop();
    }

    /// <summary>
    /// Handles loading cancellation by resetting the loading UI.
    /// </summary>
    private void HandleLoadingCancelled()
    {
        // Reset loading bar fill amount
        loadingBar.fillAmount = 0;

        // Deactivate loading UI elements
        ToggleLoadingUI(false);

        // Restore crosshair visibility
        SetCrosshairVisibility(1f);
    }

    /// <summary>
    /// Toggles the visibility of loading-related UI elements.
    /// </summary>
    private void ToggleLoadingUI(bool isVisible)
    {
        loadingBar.gameObject.SetActive(isVisible);
        loadingProgressText.gameObject.SetActive(isVisible);
        fxHolder.gameObject.SetActive(isVisible);
    }

    /// <summary>
    /// Adjusts the transparency of the crosshair.
    /// </summary>
    private void SetCrosshairVisibility(float alpha)
    {
        if (crosshairImage != null)
        {
            Color color = crosshairImage.color;
            color.a = alpha;
            crosshairImage.color = color;
        }
    }
}
