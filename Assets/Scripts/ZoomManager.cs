using System.Collections;
using UnityEngine;

public class ZoomManager : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float maxZoomDistance = 2f;

    private Coroutine zoomCoroutine;
    public static bool isZooming { get; private set; }

    public void StartZoomAndExamine(RaycastHit hit, IExaminable examinable)
    {
        if (isZooming || examinable == null) return;

        zoomCoroutine = StartCoroutine(HandleZoomAndExamination(hit, examinable));
    }

    private IEnumerator HandleZoomAndExamination(RaycastHit hit, IExaminable examinable)
    {
        isZooming = true;
        Camera camera = Camera.main;

        if (camera == null)
        {
            Debug.LogError("Main camera not found.");
            yield break;
        }

        Vector3 zoomTargetPosition = hit.point - (hit.point - camera.transform.position).normalized * maxZoomDistance;
        Vector3 originalCameraPosition = camera.transform.position;

        yield return MoveCameraToTarget(camera.transform, originalCameraPosition, zoomTargetPosition);

        examinable.Interact();

        yield return MoveCameraToTarget(camera.transform, camera.transform.position, originalCameraPosition);

        isZooming = false;
    }

    private IEnumerator MoveCameraToTarget(Transform cameraTransform, Vector3 startPosition, Vector3 targetPosition)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * zoomSpeed;
            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        cameraTransform.position = targetPosition;
    }
}
