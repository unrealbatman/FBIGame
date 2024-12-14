using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Transform playerHead; // Reference to the player's head for rotation
    [SerializeField] private Camera mainCamera; // Main camera for the player
    [SerializeField] private Transform crosshair; // Crosshair transform for aiming

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 50f; // Maximum interaction range

    [Header("Mouse Sensitivity Settings")]
    [SerializeField] private float mouseSensitivityX = 2f; // Mouse sensitivity for X-axis
    [SerializeField] private float mouseSensitivityY = 2f; // Mouse sensitivity for Y-axis
    [SerializeField] private float rotationLimitXMin = -90f; // Minimum X rotation limit
    [SerializeField] private float rotationLimitXMax = 90f; // Maximum X rotation limit
    [SerializeField] private float rotationLimitYMin = -90f; // Minimum Y rotation limit
    [SerializeField] private float rotationLimitYMax = 90f; // Maximum Y rotation limit

    private float rotationX = 0f;
    private float rotationY = 0f;

    private LoadingManager loadingManager;
    private ZoomManager zoomManager;

    public static bool isCameraLocked { get; set; } = false; // Static flag to lock/unlock camera movement

    public static event Action<bool> onShowInventory;

    private bool isInventoryActive = false;

    private void Start()
    {
        // Lock the cursor to the game screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get references to the LoadingManager and ZoomManager
        loadingManager = GetComponent<LoadingManager>();
        zoomManager = GetComponent<ZoomManager>();
    }

    private void Update()
    {
        if (!ZoomManager.isZooming && !isCameraLocked)
        {
            HandleMouseLook();
        }

        if (!isCameraLocked)
        {
            HandleAutomaticInteraction();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    /// <summary>
    /// Rotates the player's head based on mouse input.
    /// </summary>
    private void HandleMouseLook()
    {
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivityX;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivityY;

        // Clamp rotation values to prevent excessive rotation
        rotationX = Mathf.Clamp(rotationX, rotationLimitXMin, rotationLimitXMax);
        rotationY = Mathf.Clamp(rotationY, rotationLimitYMin, rotationLimitYMax);

        // Apply the rotation to the player's head
        playerHead.localEulerAngles = new Vector3(rotationY, rotationX, 0f);
    }

    /// <summary>
    /// Automatically detects objects in the player's view and interacts with them if possible.
    /// </summary>
    private void HandleAutomaticInteraction()
    {
        if (ZoomManager.isZooming || LoadingManager.isLoading || IsTeleporting())
            return;

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange) &&
            hit.collider.TryGetComponent<IExaminable>(out IExaminable examinable))
        {
            loadingManager.StartLoadingProcess(hit, () =>
            {
                if (hit.collider.TryGetComponent<Teleportal>(out Teleportal teleItem))
                {
                    teleItem.Interact();
                }
                else if (hit.collider.TryGetComponent<Evidence>(out Evidence evidenceItem))
                {
                    zoomManager.StartZoomAndExamine(hit, evidenceItem);
                }
            });
        }
        else
        {
            loadingManager.CancelLoadingProcess();
        }
    }

    /// <summary>
    /// Toggles the inventory display state.
    /// </summary>
    private void ToggleInventory()
    {
        isInventoryActive = !isInventoryActive;
        onShowInventory?.Invoke(isInventoryActive);
    }

    /// <summary>
    /// Helper method to check if teleportation is in progress.
    /// </summary>
    private bool IsTeleporting()
    {
        var teleportal = FindObjectOfType<Teleportal>();
        return teleportal != null && teleportal.isTeleporting;
    }
}
