using System;
using System.Collections;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    // Events to notify UI and other systems about loading states
    public static event Action<float> OnLoadingProgress; // Triggered as loading progresses
    public static event Action OnLoadingComplete; // Triggered when loading completes
    public static event Action OnLoadingCancelled; // Triggered when loading is cancelled

    [Header("Loading Settings")]
    [SerializeField] private float loadingDuration = 2f; // Duration to complete loading (in seconds)

    private Coroutine activeLoadingCoroutine; // Tracks the current active loading coroutine
    public static bool isLoading { get; private set; } = false; // Tracks whether a loading process is ongoing

    /// <summary>
    /// Starts the loading process for a target object.
    /// </summary>
    /// <param name="hit">The RaycastHit with target information.</param>
    /// <param name="onComplete">Callback to execute when the loading completes successfully.</param>
    public void StartLoadingProcess(RaycastHit hit, Action onComplete)
    {
        // Ensure a loading process isn't already active
        if (isLoading) return;

        // Start a new loading process
        activeLoadingCoroutine = StartCoroutine(ExecuteLoadingRoutine(hit, onComplete));
    }

    /// <summary>
    /// Cancels the current loading process if active.
    /// </summary>
    public void CancelLoadingProcess()
    {
        // Stop the active loading coroutine if it's running
        if (activeLoadingCoroutine != null)
        {
            StopCoroutine(activeLoadingCoroutine);
            activeLoadingCoroutine = null;
        }

        isLoading = false; // Reset loading status
        OnLoadingCancelled?.Invoke(); // Notify listeners that loading was canceled
    }

    /// <summary>
    /// Executes the loading process in a coroutine.
    /// </summary>
    /// <param name="hit">Initial RaycastHit information.</param>
    /// <param name="onComplete">Callback to execute on successful completion.</param>
    private IEnumerator ExecuteLoadingRoutine(RaycastHit hit, Action onComplete)
    {
        isLoading = true; // Mark loading as in progress
        float progress = 0f; // Progress bar for loading

        // Loading loop until progress reaches 100%
        while (progress < 1f)
        {
            // Check if the crosshair is still over the same target
            if (!IsCrosshairStillOnTarget(hit))
            {
                CancelLoadingProcess(); // Cancel if the target is no longer in focus
                yield break;
            }

            progress += Time.deltaTime / loadingDuration; // Increment progress
            OnLoadingProgress?.Invoke(progress); // Notify listeners about the progress
            yield return null; // Wait for the next frame
        }

        // Finalizing loading process
        isLoading = false; // Mark loading as complete
        OnLoadingComplete?.Invoke(); // Notify listeners about loading completion
        onComplete?.Invoke(); // Trigger the provided completion callback
    }

    /// <summary>
    /// Checks if the crosshair is still aimed at the original target.
    /// </summary>
    /// <param name="initialHit">The initial RaycastHit data.</param>
    /// <returns>True if the crosshair is still on the same target; otherwise, false.</returns>
    private bool IsCrosshairStillOnTarget(RaycastHit initialHit)
    {
        // Cast a ray from the screen's center (where the crosshair is)
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        // Check if the ray still hits the same object as the initial target
        return Physics.Raycast(ray, out RaycastHit hit) && hit.collider == initialHit.collider;
    }
}
