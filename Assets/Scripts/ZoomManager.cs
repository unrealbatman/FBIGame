using System.Collections;
using UnityEngine;

public class ZoomManager : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f; // Speed of the zoom transition.
    [SerializeField] private float maxZoomDistance = 2f; // Maximum zoom distance from the hit point.

    private Coroutine zoomCoroutine;
    public static bool isZooming { get; private set; } // Indicates if the zoom process is currently active.

    /// <summary>
    /// Starts the zoom and examination process when the player interacts with an examinable object.
    /// </summary>
    /// <param name="hit">The RaycastHit containing information about where the ray hit.</param>
    /// <param name="examinable">The object to be examined after zooming.</param>
    public void StartZoomAndExamine(RaycastHit hit, IExaminable examinable)
    {
        // If already zooming or the examinable is null, return early.
        if (isZooming || examinable == null) return;

        // Start the coroutine to handle the zoom and examination process.
        zoomCoroutine = StartCoroutine(HandleZoomAndExamination(hit, examinable));
    }

    /// <summary>
    /// Coroutine that handles the zoom-in, examination, and zoom-out process.
    /// </summary>
    private IEnumerator HandleZoomAndExamination(RaycastHit hit, IExaminable examinable)
    {
        isZooming = true; // Mark that zoom is active.
        Camera camera = Camera.main;

        // Check if the main camera is available.
        if (camera == null)
        {
            Debug.LogError("Main camera not found.");
            yield break;
        }

        // Calculate the target zoom position based on the hit point and max zoom distance.
        Vector3 zoomTargetPosition = hit.point - (hit.point - camera.transform.position).normalized * maxZoomDistance;
        Vector3 originalCameraPosition = camera.transform.position;

        // Zoom in smoothly.
        yield return MoveCameraToTarget(camera.transform, originalCameraPosition, zoomTargetPosition);

        // Perform the examination of the object.
        examinable.Interact();

        // Zoom out smoothly.
        yield return MoveCameraToTarget(camera.transform, camera.transform.position, originalCameraPosition);

        isZooming = false; // Mark that zooming is complete.
    }

    /// <summary>
    /// Smoothly moves the camera from the start position to the target position.
    /// </summary>
    /// <param name="cameraTransform">The transform of the camera to move.</param>
    /// <param name="startPosition">The starting position of the camera.</param>
    /// <param name="targetPosition">The target position for the camera.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator MoveCameraToTarget(Transform cameraTransform, Vector3 startPosition, Vector3 targetPosition)
    {
        float progress = 0f;

        // Smoothly interpolate the camera position until it reaches the target.
        while (progress < 1f)
        {
            progress += Time.deltaTime * zoomSpeed;
            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        // Snap to the exact target position to avoid any precision issues at the end of the transition.
        cameraTransform.position = targetPosition;
    }
}
