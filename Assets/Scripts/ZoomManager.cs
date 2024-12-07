using System.Collections;
using UnityEngine;

public class ZoomManager : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float maxZoomDistance = 2f;

    private Coroutine zoomCoroutine;
    public static bool isZooming = false; // Indicates if the zoom process is active

    /// <summary>
    /// Initiates the zoom and examination process.
    /// </summary>
    /// <param name="hit">The RaycastHit containing the hit information.</param>
    /// <param name="examinable">The object to be examined.</param>
    public void StartZoomAndExamine(RaycastHit hit, IExaminable examinable)
    {
        if (isZooming) return;

        zoomCoroutine = StartCoroutine(HandleZoomAndExamination(hit, examinable));
    }

    /// <summary>
    /// Coroutine to handle the zoom-in, examination, and zoom-out process.
    /// </summary>
    private IEnumerator HandleZoomAndExamination(RaycastHit hit, IExaminable examinable )
    {
        isZooming = true;
        Camera camera = Camera.main;

        // Calculate the target zoom position
        Vector3 zoomTargetPosition = hit.point - (hit.point - camera.transform.position).normalized * maxZoomDistance;

        // Save the current camera position before zooming in
        Vector3 currentCameraPosition = camera.transform.position;

       
        // Zoom in
        yield return MoveCameraToTarget(camera.transform, currentCameraPosition, zoomTargetPosition);

        // Perform examination
        examinable.Examine();

        // Zoom out
        yield return MoveCameraToTarget(camera.transform, camera.transform.position, currentCameraPosition);

        isZooming = false;
    }

    /// <summary>
    /// Moves the camera between two positions smoothly.
    /// </summary>
    private IEnumerator MoveCameraToTarget(Transform cameraTransform, Vector3 startPosition, Vector3 targetPosition)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * zoomSpeed;
            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        // Ensure the camera snaps precisely to the target position
        cameraTransform.position = targetPosition;
    }
}
