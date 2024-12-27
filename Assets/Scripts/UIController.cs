using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Loading Bar References")]
    [SerializeField] private Image loadingBar;
    [SerializeField] private RectTransform fxHolder;
    [SerializeField] private TextMeshProUGUI loadingProgressText;
    [SerializeField] private GameObject crosshair;

    [Header("FX Settings")]
    [SerializeField] private ParticleSystem loadingFXParticle;

    [Header("Evidence Popup References")]
    [SerializeField] private GameObject evidencePopup;
    [SerializeField] private Image evidenceIcon;
    [SerializeField] private TextMeshProUGUI evidenceNameText;
    [SerializeField] private TextMeshProUGUI evidenceDescriptionText;

    [Header("Inventory Menu References")]
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private Transform inventoryListParent;
    [SerializeField] private Button evidenceButtonPrefab;

    [Header("Evidence Collection UI")]
    [SerializeField] private TextMeshProUGUI evidenceProgressText;

    private RawImage crosshairImage;

    private void Awake()
    {
        crosshairImage = crosshair.GetComponent<RawImage>();
        if (crosshairImage == null)
        {
            Debug.LogWarning("Crosshair does not have a RawImage component!");
        }
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
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

    private void UpdateLoadingBar(float progress)
    {
        loadingBar.GetComponentsInChildren<Image>()[1].fillAmount = progress;
        loadingProgressText.text = Mathf.Floor(progress * 100).ToString();
        ToggleLoadingUI(true);
        fxHolder.rotation = Quaternion.Euler(0f, 0f, -progress * 360);

        if (!loadingFXParticle.isPlaying)
        {
            loadingFXParticle.Play();
        }

        SetCrosshairVisibility(0f);
    }

    private void HandleLoadingComplete()
    {
        ToggleLoadingUI(false);
        loadingFXParticle.Stop();
    }

    private void HandleLoadingCancelled()
    {
        loadingBar.fillAmount = 0;
        ToggleLoadingUI(false);
        SetCrosshairVisibility(1f);
    }

    private void ToggleLoadingUI(bool isVisible)
    {
        loadingBar.gameObject.SetActive(isVisible);
        loadingProgressText.gameObject.SetActive(isVisible);
        fxHolder.gameObject.SetActive(isVisible);
    }

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

    private void ToggleInventoryMenu(bool isActive)
    {
        if ((evidencePopup.activeSelf && isActive != true) || ZoomManager.isZooming || LoadingManager.isLoading)
        {
            return;
        }

        inventoryMenu.SetActive(isActive);
        LockCamera(isActive);

        if (isActive)
        {
            PopulateInventory();
        }
    }

    private static void LockCamera(bool shouldLock)
    {
        PlayerController.isCameraLocked = shouldLock;
        Cursor.lockState = shouldLock ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = shouldLock;
    }

    private void PopulateInventory()
    {
        foreach (Transform child in inventoryListParent)
        {
            Destroy(child.gameObject);
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
            eb.Initialize(evidenceData);
        }
    }

    #endregion

    #region Evidence Popup

    private void ShowEvidencePopup(EvidenceData evidence)
    {
        if (inventoryMenu.activeSelf)
        {
            inventoryMenu.SetActive(false);
        }

        evidenceIcon.sprite = evidence.clueIcon;
        evidenceNameText.text = evidence.clueName;
        evidenceDescriptionText.text = evidence.clueDescription;
        evidencePopup.SetActive(true);
        LockCamera(true);
    }

    public void HideEvidencePopup()
    {
        if (!inventoryMenu.activeSelf && InventoryManager.Instance.GetCollectedEvidence().Count > 1)
        {
            ToggleInventoryMenu(true);
        }

        evidencePopup.SetActive(false);
        LockCamera(inventoryMenu.activeSelf);
    }

    private void UpdateEvidenceProgress(int collected, int total)
    {
        evidenceProgressText.text = $"{collected}/{total}";
    }

    #endregion

    #region Teleportal UI

    private void ShowTeleportalLoadUI(bool isActive, GameObject teleportalCanvas)
    {
        teleportalCanvas.SetActive(isActive);
    }

    #endregion
}
