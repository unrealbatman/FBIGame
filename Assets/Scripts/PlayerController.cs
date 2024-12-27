using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Transform playerHead;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform crosshair;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 50f;

    [Header("Mouse Sensitivity Settings")]
    [SerializeField] private float mouseSensitivityX = 2f;
    [SerializeField] private float mouseSensitivityY = 2f;
    [SerializeField] private Vector2 rotationLimitX = new Vector2(-90f, 90f);
    [SerializeField] private Vector2 rotationLimitY = new Vector2(-90f, 90f);

    private float rotationX = 0f;
    private float rotationY = 0f;

    private LoadingManager loadingManager;
    private ZoomManager zoomManager;

    public static bool isCameraLocked { get; set; } = false;

    public static event Action<bool> onShowInventory;

    private bool isInventoryActive = false;
    private GameObject activeTeleportal;

    private void Start()
    {
        LockCursor();
        InitializeManagers();
    }

    private void Update()
    {
        if (!ZoomManager.isZooming && !isCameraLocked)
        {
            HandlePlayerRotation();
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

 
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    private void InitializeManagers()
    {
        loadingManager = GetComponent<LoadingManager>();
        zoomManager = GetComponent<ZoomManager>();

        if (loadingManager == null || zoomManager == null)
        {
            Debug.LogError("Required managers are not attached to the PlayerController.");
        }
    }

 
    private void HandlePlayerRotation()
    {
        rotationX = Mathf.Clamp(rotationX + Input.GetAxis("Mouse X") * mouseSensitivityX, rotationLimitX.x, rotationLimitX.y);
        rotationY = Mathf.Clamp(rotationY - Input.GetAxis("Mouse Y") * mouseSensitivityY, rotationLimitY.x, rotationLimitY.y);

        playerHead.localEulerAngles = new Vector3(rotationY, rotationX, 0f);
    }

    /// <summary>
    /// Handles automatic interaction by processing raycast hits.
    /// </summary>
    private void HandleAutomaticInteraction()
    {
        if (LoadingManager.isLoading || ZoomManager.isZooming) return;

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
            ProcessRaycastHit(hit);
        else
            ResetInteractionState();
    }


    private void ProcessRaycastHit(RaycastHit hit)
    {
        // Handle Teleportal interactions
        if (hit.collider.TryGetComponent(out Teleportal teleportal))
        {
            InteractWithTeleportal(teleportal);
        }
        else
        {
            // Reset teleportal state if no valid teleportal is detected
            ResetTeleportalState();
        }

        // Handle examinable objects (e.g., Evidence)
        if (hit.collider.TryGetComponent(out IExaminable examinable))
        {
            InteractWithExaminable(hit);
        }
    }

   
    private void InteractWithTeleportal(Teleportal teleportal)
    {
        // Skip teleportal interaction if it's already teleporting
        if (teleportal.IsTeleporting) return;
        teleportal.EnableTeleportal(true, teleportal.teleportalCanvas);
        activeTeleportal = teleportal.gameObject;
    }

  
    private void InteractWithExaminable(RaycastHit hit)
    {
        // Check if there's an active teleportal and ensure it's not teleporting
        if (activeTeleportal != null && activeTeleportal.GetComponent<Teleportal>().IsTeleporting) return; 
            
        
        // Proceed with examination logic if no teleportal is blocking interaction
        loadingManager.StartLoadingProcess(hit, () =>
        {
            activeTeleportal?.GetComponent<Teleportal>()?.Interact();
            // Interact with the examinable object (e.g., evidence)
            if (hit.collider.TryGetComponent(out Evidence evidenceItem))
            {
                zoomManager.StartZoomAndExamine(hit, evidenceItem);
            }
        });
    }

    /// <summary>
    /// Resets the teleportal state when no valid teleportal is detected.
    /// </summary>
    private void ResetTeleportalState()
    {
        if (activeTeleportal == null) return;

        var teleportal = activeTeleportal.GetComponent<Teleportal>();
        teleportal?.EnableTeleportal(false, teleportal.teleportalCanvas);
        activeTeleportal = null;
    }

    /// <summary>
    /// Resets interaction state when no valid object is hit.
    /// </summary>
    private void ResetInteractionState()
    {
        loadingManager.CancelLoadingProcess();
        ResetTeleportalState();
    }
    /// <summary>
    /// Toggles the inventory display state.
    /// </summary>
    private void ToggleInventory()
    {
        isInventoryActive = !isInventoryActive;
        onShowInventory?.Invoke(isInventoryActive);
    }
}
