using System;
using System.Collections;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    // Events to notify UI and other systems about loading states
    public static event Action<float> OnLoadingProgress; // Fired as loading progresses
    public static event Action OnLoadingComplete; // Fired when loading completes
    public static event Action OnLoadingCancelled; // Fired when loading is cancelled

    [Header("Loading Settings")]
    [SerializeField] private float loadingDuration = 2f; // Duration required to complete loading

    private Coroutine activeLoadingCoroutine; // Tracks the currently running loading coroutine
    public static bool isLoading = false; // Indicates whether loading is active

    /// <summary>
    /// Starts the loading process for a given target.
    /// </summary>
    /// <param name="hit">The RaycastHit containing the target information.</param>
    /// <param name="onComplete">Callback to execute when loading completes.</param>
    public void StartLoadingProcess(RaycastHit hit, Action onComplete)
    {
        // Prevent starting a new loading process if one is already active
        if (isLoading) return;

        activeLoadingCoroutine = StartCoroutine(ExecuteLoadingRoutine(hit, onComplete));
    }

    /// <summary>
    /// Cancels the currently active loading process.
    /// </summary>
    public void CancelLoadingProcess()
    {
        if (activeLoadingCoroutine != null)
        {
            StopCoroutine(activeLoadingCoroutine);
            activeLoadingCoroutine = null;
        }

        isLoading = false;
        OnLoadingCancelled?.Invoke(); // Notify listeners that loading was cancelled
    }

    /// <summary>
    /// Coroutine to manage the loading process.
    /// </summary>
    private IEnumerator ExecuteLoadingRoutine(RaycastHit hit, Action onComplete)
    {
        isLoading = true;
        float progress = 0f;

        while (progress < 1f)
        {
            // Check if the crosshair is still on the target
            if (!IsCrosshairStillOnTarget(hit))
            {
                CancelLoadingProcess();
                yield break; // Exit the coroutine if the target is lost
            }

            // Increment progress over time
            progress += Time.deltaTime / loadingDuration;

            // Notify listeners of the current loading progress
            OnLoadingProgress?.Invoke(progress);
            yield return null;
        }

        // Loading is complete
        isLoading = false;
        OnLoadingComplete?.Invoke(); // Notify listeners
        onComplete?.Invoke(); // Trigger the provided callback
    }

    /// <summary>
    /// Checks whether the crosshair is still aimed at the initial target.
    /// </summary>
    /// <param name="initialHit">The initial RaycastHit.</param>
    /// <returns>True if the crosshair is still on the target; otherwise, false.</returns>
    private bool IsCrosshairStillOnTarget(RaycastHit initialHit)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        return Physics.Raycast(ray, out RaycastHit hit) && hit.collider == initialHit.collider;
    }
}
