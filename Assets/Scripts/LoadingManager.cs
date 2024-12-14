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
    [SerializeField] private float loadingDuration = 2f; // Time required to complete loading

    private Coroutine activeLoadingCoroutine; // Tracks the current loading coroutine
    public static bool isLoading { get; private set; } = false; // Tracks whether loading is active

    /// <summary>
    /// Starts the loading process for a target object.
    /// </summary>
    /// <param name="hit">The RaycastHit with target information.</param>
    /// <param name="onComplete">Callback to execute on successful completion.</param>
    public void StartLoadingProcess(RaycastHit hit, Action onComplete)
    {
        if (isLoading) return; // Avoid starting a new process if one is already active

        activeLoadingCoroutine = StartCoroutine(ExecuteLoadingRoutine(hit, onComplete));
    }

    /// <summary>
    /// Cancels the current loading process.
    /// </summary>
    public void CancelLoadingProcess()
    {
        if (activeLoadingCoroutine != null)
        {
            StopCoroutine(activeLoadingCoroutine);
            activeLoadingCoroutine = null;
        }

        isLoading = false;
        OnLoadingCancelled?.Invoke(); // Notify listeners about the cancellation
    }

    /// <summary>
    /// Manages the loading process through a coroutine.
    /// </summary>
    /// <param name="hit">Initial RaycastHit information.</param>
    /// <param name="onComplete">Callback to execute on successful completion.</param>
    private IEnumerator ExecuteLoadingRoutine(RaycastHit hit, Action onComplete)
    {
        isLoading = true;
        float progress = 0f;

        while (progress < 1f)
        {
            // Verify the crosshair remains on the initial target
            if (!IsCrosshairStillOnTarget(hit))
            {
                CancelLoadingProcess();
                yield break;
            }

            progress += Time.deltaTime / loadingDuration; // Increment progress
            OnLoadingProgress?.Invoke(progress); // Notify listeners of progress
            yield return null;
        }

        // Completion logic
        isLoading = false;
        OnLoadingComplete?.Invoke(); // Notify listeners about completion
        onComplete?.Invoke(); // Trigger the provided callback
    }

    /// <summary>
    /// Checks if the crosshair is still aimed at the original target.
    /// </summary>
    /// <param name="initialHit">The initial RaycastHit.</param>
    /// <returns>True if the crosshair is still on target; otherwise, false.</returns>
    private bool IsCrosshairStillOnTarget(RaycastHit initialHit)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        return Physics.Raycast(ray, out RaycastHit hit) && hit.collider == initialHit.collider;
    }
}
