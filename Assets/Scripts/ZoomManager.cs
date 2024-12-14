using System.Collections;
using UnityEngine;

public class ZoomManager : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f; // Speed of the zoom transition
    [SerializeField] private float maxZoomDistance = 2f; // Maximum zoom distance from the hit point

    private Coroutine zoomCoroutine;
    public static bool isZooming { get; private set; } // Indicates if the zoom process is active

    /// <summary>
    /// Initiates the zoom and examination process.
    /// </summary>
    /// <param name="hit">The RaycastHit containing the hit information.</param>
    /// <param name="examinable">The object to be examined.</param>
    public void StartZoomAndExamine(RaycastHit hit, IExaminable examinable)
    {
        if (isZooming || examinable == null) return;

        zoomCoroutine = StartCoroutine(HandleZoomAndExamination(hit, examinable));
    }

    /// <summary>
    /// Coroutine to handle the zoom-in, examination, and zoom-out process.
    /// </summary>
    private IEnumerator HandleZoomAndExamination(RaycastHit hit, IExaminable examinable)
    {
        isZooming = true;
        Camera camera = Camera.main;

        if (camera == null)
        {
            Debug.LogError("Main camera not found.");
            yield break;
        }

        // Calculate the target zoom position
        Vector3 zoomTargetPosition = hit.point - (hit.point - camera.transform.position).normalized * maxZoomDistance;
        Vector3 originalCameraPosition = camera.transform.position;

        // Zoom in
        yield return MoveCameraToTarget(camera.transform, originalCameraPosition, zoomTargetPosition);

        // Perform examination
        examinable.Interact();

        // Zoom out
        yield return MoveCameraToTarget(camera.transform, camera.transform.position, originalCameraPosition);

        isZooming = false;
    }

    /// <summary>
    /// Smoothly moves the camera between two positions.
    /// </summary>
    /// <param name="cameraTransform">Transform of the camera.</param>
    /// <param name="startPosition">Starting position of the camera.</param>
    /// <param name="targetPosition">Target position of the camera.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator MoveCameraToTarget(Transform cameraTransform, Vector3 startPosition, Vector3 targetPosition)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * zoomSpeed;
            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        // Snap to the target position to avoid precision issues
        cameraTransform.position = targetPosition;
    }
}
