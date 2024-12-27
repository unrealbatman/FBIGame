using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Represents a teleportal object that allows the player to teleport after interacting with it.
/// </summary>
public class Teleportal : MonoBehaviour, IExaminable
{
    [Header("Teleport Settings")]
    [SerializeField] private float teleportDelay = 10f; // Delay before the teleportation occurs.

    private bool isTeleporting = false; // Tracks whether the teleportation process is ongoing.

    public bool IsTeleporting { get => isTeleporting; set => isTeleporting = value; }

    public GameObject teleportalCanvas;

    public static event Action<bool, GameObject> OnTeleportalEnabled;



    /// <summary>
    /// Interact with the teleportal to initiate teleportation.
    /// Prevents multiple simultaneous teleport attempts.
    /// </summary>
    public void Interact()
    {
        if (IsTeleporting) return; // Exit if a teleportation is already in progress.

        Debug.Log("Teleporting player...");
        StartCoroutine(TeleportPlayer()); // Start the teleportation process.
    }

    /// <summary>
    /// Handles the teleportation process with a delay.
    /// Finds the player's transform and moves it to the teleportal's position and rotation.
    /// </summary>
    /// <returns>Coroutine IEnumerator for delayed execution.</returns>
    private IEnumerator TeleportPlayer()
    {
        IsTeleporting = true; // Mark teleportation as in progress.

        yield return new WaitForSeconds(teleportDelay); // Wait for the specified delay.

        // Attempt to locate the PlayerController's transform.
        Transform playerTransform = FindObjectOfType<PlayerController>()?.transform;

        if (playerTransform != null)
        {
            
            // Teleport the player to the transform's position
            playerTransform.position = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z);

            // Set the player's rotation to the transform's rotation, then rotate by 180 degrees on the Y-axis
            playerTransform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 180f, transform.rotation.eulerAngles.z);

            Debug.Log("Player teleported successfully.");
        }
        else
        {
            // Log a warning if the PlayerController could not be found.
            Debug.LogWarning("PlayerController not found. Teleportation failed.");
        }

        IsTeleporting = false; // Reset teleportation status.
    }



    public void EnableTeleportal(bool isActive, GameObject canvas)
    {
        OnTeleportalEnabled?.Invoke(isActive, canvas);

    }
}