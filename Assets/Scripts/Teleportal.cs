using System;
using System.Collections;
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
            // Set the player's position and rotation to match the teleportal's.
            playerTransform.SetPositionAndRotation(transform.position, transform.rotation);
            Debug.Log("Player teleported successfully.");
        }
        else
        {
            // Log a warning if the PlayerController could not be found.
            Debug.LogWarning("PlayerController not found. Teleportation failed.");
        }

        IsTeleporting = false; // Reset teleportation status.
    }
}
