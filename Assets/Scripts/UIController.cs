using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the user interface elements, such as loading bars, inventory, evidence popups,
/// and other UI features for the game.
/// </summary>
public class UIController : MonoBehaviour
{
    [Header("Loading Bar References")]
    [SerializeField] private Image loadingBar; // Reference to the loading bar image.
    [SerializeField] private RectTransform fxHolder; // Holder for the loading effects rotation.
    [SerializeField] private TextMeshProUGUI loadingProgressText; // Text displaying loading progress percentage.
    [SerializeField] private GameObject crosshair; // Crosshair UI element.

    [Header("FX Settings")]
    [SerializeField] private ParticleSystem loadingFXParticle; // Particle effect for loading visual feedback.

    [Header("Evidence Popup References")]
    [SerializeField] private GameObject evidencePopup; // Popup UI for evidence details.
    [SerializeField] private Image evidenceIcon; // Icon of the evidence.
    [SerializeField] private TextMeshProUGUI evidenceNameText; // Name of the evidence.
    [SerializeField] private TextMeshProUGUI evidenceDescriptionText; // Description of the evidence.

    [Header("Inventory Menu References")]
    [SerializeField] private GameObject inventoryMenu; // The inventory UI panel.
    [SerializeField] private Transform inventoryListParent; // Parent transform for dynamically added inventory buttons.
    [SerializeField] private Button evidenceButtonPrefab; // Prefab for evidence buttons.

    [Header("Evidence Collection UI")]
    [SerializeField] private TextMeshProUGUI evidenceProgressText; // Text displaying collected evidence progress.

    private RawImage crosshairImage; // Cached reference to the crosshair's RawImage component.

    private void Awake()
    {
        // Cache the RawImage component of the crosshair and warn if not found.
        crosshairImage = crosshair.GetComponent<RawImage>();
        if (crosshairImage == null)
        {
            Debug.LogWarning("Crosshair does not have a RawImage component!");
        }
    }

    private void OnEnable()
    {
        // Subscribe to all necessary events.
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        // Unsubscribe from all events to avoid memory leaks.
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        // Subscribing to various game events to handle UI updates.
        LoadingManager.OnLoadingProgress += UpdateLoadingBar;
        LoadingManager.OnLoadingComplete += HandleLoadingComplete;
        LoadingManager.OnLoadingCancelled += HandleLoadingCancelled;
        InventoryManager.OnEvidenceAdded += ShowEvidencePopup;
        PlayerController.onShowInventory += ToggleInventoryMenu;
        EvidenceButton.OnButton += ShowEvidencePopup;
        Teleportal.OnTeleportalEnabled += ShowTeleportalLoadUI;
        LevelManager.OnEvidenceProgressUpdated += UpdateEvidenceProgress;
    }

    private void UnsubscribeFromEvents()
    {
        // Unsubscribing from all game events.
        LoadingManager.OnLoadingProgress -= UpdateLoadingBar;
        LoadingManager.OnLoadingComplete -= HandleLoadingComplete;
        LoadingManager.OnLoadingCancelled -= HandleLoadingCancelled;
        InventoryManager.OnEvidenceAdded -= ShowEvidencePopup;
        PlayerController.onShowInventory -= ToggleInventoryMenu;
        EvidenceButton.OnButton -= ShowEvidencePopup;
        Teleportal.OnTeleportalEnabled -= ShowTeleportalLoadUI;
        LevelManager.OnEvidenceProgressUpdated -= UpdateEvidenceProgress;
    }

    #region Loading UI

    /// <summary>
    /// Updates the loading bar and related visual elements.
    /// </summary>
    private void UpdateLoadingBar(float progress)
    {
        loadingBar.GetComponentsInChildren<Image>()[1].fillAmount = progress; // Updates the loading bar fill amount.
        loadingProgressText.text = Mathf.Floor(progress * 100).ToString(); // Updates the progress text.
        ToggleLoadingUI(true); // Shows the loading UI.
        fxHolder.rotation = Quaternion.Euler(0f, 0f, -progress * 360); // Rotates the loading FX holder.

        if (!loadingFXParticle.isPlaying)
        {
            loadingFXParticle.Play(); // Starts the loading particle effect.
        }

        SetCrosshairVisibility(0f); // Hides the crosshair during loading.
    }

    /// <summary>
    /// Handles actions when loading is completed.
    /// </summary>
    private void HandleLoadingComplete()
    {
        ToggleLoadingUI(false); // Hides the loading UI.
        loadingFXParticle.Stop(); // Stops the loading particle effect.
    }

    /// <summary>
    /// Resets and hides the loading bar when loading is cancelled.
    /// </summary>
    private void HandleLoadingCancelled()
    {
        loadingBar.fillAmount = 0; // Resets the loading bar.
        ToggleLoadingUI(false); // Hides the loading UI.
        SetCrosshairVisibility(1f); // Restores the crosshair visibility.
    }

    /// <summary>
    /// Toggles the visibility of loading UI elements.
    /// </summary>
    private void ToggleLoadingUI(bool isVisible)
    {
        loadingBar.gameObject.SetActive(isVisible);
        loadingProgressText.gameObject.SetActive(isVisible);
        fxHolder.gameObject.SetActive(isVisible);
    }

    /// <summary>
    /// Sets the crosshair's visibility by adjusting its alpha.
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

    #endregion

    #region Inventory UI

    /// <summary>
    /// Toggles the inventory menu and locks/unlocks the camera accordingly.
    /// </summary>
    private void ToggleInventoryMenu(bool isActive)
    {
        if ((evidencePopup.activeSelf && isActive != true) || ZoomManager.isZooming || LoadingManager.isLoading)
        {
            return; // Prevents toggling inventory in specific conditions.
        }

        inventoryMenu.SetActive(isActive);
        LockCamera(isActive);

        if (isActive)
        {
            PopulateInventory(); // Populates the inventory list when activated.
        }
    }

    /// <summary>
    /// Locks or unlocks the camera based on the inventory state.
    /// </summary>
    private static void LockCamera(bool shouldLock)
    {
        PlayerController.isCameraLocked = shouldLock;
        Cursor.lockState = shouldLock ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = shouldLock;
    }

    /// <summary>
    /// Populates the inventory UI with collected evidence.
    /// </summary>
    private void PopulateInventory()
    {
        foreach (Transform child in inventoryListParent)
        {
            Destroy(child.gameObject); // Clears the inventory list.
        }

        foreach (var evidenceData in InventoryManager.Instance.GetCollectedEvidence())
        {
            Button evidenceButton = Instantiate(evidenceButtonPrefab, inventoryListParent);
            evidenceButton.name = evidenceData.clueName;

            TextMeshProUGUI buttonText = evidenceButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = evidenceData.clueName;
            }
            else
            {
                Debug.LogWarning("No TextMeshProUGUI component found on the button!");
            }

            EvidenceButton eb = evidenceButton.GetComponent<EvidenceButton>();
            eb.Initialize(evidenceData); // Initializes the button with evidence data.
        }
    }

    #endregion

    #region Evidence Popup

    /// <summary>
    /// Displays a popup with evidence details.
    /// </summary>
    private void ShowEvidencePopup(EvidenceData evidence)
    {
        if (inventoryMenu.activeSelf)
        {
            inventoryMenu.SetActive(false); // Hides the inventory if open.
        }

        evidenceIcon.sprite = evidence.clueIcon;
        evidenceNameText.text = evidence.clueName;
        evidenceDescriptionText.text = evidence.clueDescription;
        evidencePopup.SetActive(true); // Shows the evidence popup.
        LockCamera(true); // Locks the camera during evidence popup.
    }

    /// <summary>
    /// Hides the evidence popup and restores previous states.
    /// </summary>
    public void HideEvidencePopup()
    {
        if (!inventoryMenu.activeSelf && InventoryManager.Instance.GetCollectedEvidence().Count > 1)
        {
            ToggleInventoryMenu(true); // Reopens the inventory if there is more evidence.
        }

        evidencePopup.SetActive(false);
        LockCamera(inventoryMenu.activeSelf); // Restores the camera state based on inventory.
    }

    /// <summary>
    /// Updates the evidence collection progress UI.
    /// </summary>
    private void UpdateEvidenceProgress(int collected, int total)
    {
        evidenceProgressText.text = $"{collected}/{total}";
    }

    #endregion

    #region Teleportal UI

    /// <summary>
    /// Toggles the teleportal UI visibility.
    /// </summary>
    private void ShowTeleportalLoadUI(bool isActive,GameObject teleportalCanvas)
    {
        teleportalCanvas.SetActive(isActive);
    }

    #endregion
}
