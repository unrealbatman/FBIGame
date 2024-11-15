using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static event Action<float> OnLoadingProgress;
    public static event Action OnLoadingComplete;
    public static event Action OnLoadingCancelled;

    [Header("Player Settings")]
    [SerializeField] private Transform playerHead;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform crosshair;

    [Header("Zoom and Interaction Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float maxZoomDistance = 2f;
    [SerializeField] private float loadingTime = 2f;
    [SerializeField] private float interactionRange = 50f;

    [Header("Mouse Sensitivity")]
    public float sensitivityX = 2f;
    public float minX = -90f;
    public float maxX = 90f;
    public float sensitivityY = 2f;
    public float minY = -90f;
    public float maxY = 90f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector3 initialCamPosition;
    private bool isZooming = false;
    private bool isLoading = false;

    private Coroutine zoomCoroutine;
    private Coroutine loadingCoroutine;
    private IExaminable targetExaminable;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        initialCamPosition = cam.transform.position;
    }

    private void Update()
    {
        RotateHeadWithMouseInput();
        HandleInteraction();
    }

    private void RotateHeadWithMouseInput()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationY += Input.GetAxis("Mouse Y") * -sensitivityY;

        rotationX = Mathf.Clamp(rotationX, minX, maxX);
        rotationY = Mathf.Clamp(rotationY, minY, maxY);

        playerHead.localEulerAngles = new Vector3(rotationY, rotationX, playerHead.localEulerAngles.z);
    }

    private void HandleInteraction()
    {

        //This is to prevent further raycasting if already zooming or loading
        if (isZooming || isLoading) return;

        Ray ray = cam.ScreenPointToRay(crosshair.position);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange) &&
            hit.collider.gameObject.TryGetComponent<IExaminable>(out IExaminable examinable))
        {
            targetExaminable = examinable;
            StartLoadingCoroutine(hit);
        }
    }


    private void StartLoadingCoroutine(RaycastHit hit)
    {

        if (loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
        }

        loadingCoroutine = StartCoroutine(LoadingRoutine(hit));
    }

    private void StartZoomCoroutine(RaycastHit hit, IExaminable examinable)
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomAndExamine(hit, examinable));
    }

    private IEnumerator LoadingRoutine(RaycastHit hit)
    {

        isLoading = true;


        float progress = 0f;

        while (progress < 1f)
        {
            //This is where we determine loading bar logic. 


            if (!IsCrosshairOnTarget())
            {
                CancelLoading();
                yield break;
            }


            progress += Time.deltaTime / loadingTime;

            OnLoadingProgress?.Invoke(progress);
            yield return null;

        }

        isLoading = false;
        OnLoadingComplete?.Invoke();
        StartZoomCoroutine(hit, targetExaminable);
    }


    private bool IsCrosshairOnTarget()
    {
        Ray ray = cam.ScreenPointToRay(crosshair.position);
        return Physics.Raycast(ray, out RaycastHit hit, interactionRange) &&
               hit.collider.gameObject.TryGetComponent(out IExaminable examinable) &&
               examinable == targetExaminable;
    }


    private IEnumerator ZoomAndExamine(RaycastHit hit, IExaminable examinable)
    {
        isZooming = true;
        Vector3 zoomTargetPosition = hit.point - (hit.point - initialCamPosition).normalized * maxZoomDistance;

        // Move camera to zoom position
        yield return MoveCamera(cam.transform.position, zoomTargetPosition);

        // Perform examination
        examinable.Examine();

        // Move camera back to initial position
        yield return MoveCamera(zoomTargetPosition, initialCamPosition);

        isZooming = false;
    }



    private void CancelLoading()
    {


        if(loadingCoroutine != null) StopCoroutine(loadingCoroutine);
        OnLoadingCancelled?.Invoke();
        isLoading = false;  
        targetExaminable = null;
    }



    private IEnumerator MoveCamera(Vector3 fromPosition, Vector3 toPosition)
    {
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * zoomSpeed;
            cam.transform.position = Vector3.Lerp(fromPosition, toPosition, progress);
            yield return null;



        }

    }

}