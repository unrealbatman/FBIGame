using System;
using System.Collections;
using UnityEngine;

public class Teleportal : MonoBehaviour, IExaminable
{
    [Header("Teleport Settings")]
    [SerializeField] private float teleportDelay = 10f;

    private bool isTeleporting = false;

    public bool IsTeleporting { get => isTeleporting; set => isTeleporting = value; }

    public GameObject teleportalCanvas;

    public static event Action<bool, GameObject> OnTeleportalEnabled;

    public void Interact()
    {
        if (IsTeleporting) return;

        Debug.Log("Teleporting player...");
        StartCoroutine(TeleportPlayer());
    }

    private IEnumerator TeleportPlayer()
    {
        IsTeleporting = true;

        yield return new WaitForSeconds(teleportDelay);

        Transform playerTransform = FindObjectOfType<PlayerController>()?.transform;

        if (playerTransform != null)
        {
            playerTransform.position = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z);
            playerTransform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 180f, transform.rotation.eulerAngles.z);

            Debug.Log("Player teleported successfully.");
        }
        else
        {
            Debug.LogWarning("PlayerController not found. Teleportation failed.");
        }

        IsTeleporting = false;
    }

    public void EnableTeleportal(bool isActive, GameObject canvas)
    {
        OnTeleportalEnabled?.Invoke(isActive, canvas);
    }
}
