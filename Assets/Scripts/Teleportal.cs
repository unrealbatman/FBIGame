using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportal : MonoBehaviour, IExaminable
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform teleportTarget; // Target location
    [SerializeField] private float teleportDelay = 2f;

    public void Examine()
    {
        Debug.Log($"Teleporting player...");
        StartCoroutine(TeleportPlayer());
    }


    private IEnumerator TeleportPlayer()
    {
        yield return new WaitForSeconds(teleportDelay);

        // Teleport player
        Transform playerTransform = FindObjectOfType<PlayerController>().transform;
        if (teleportTarget != null && playerTransform != null)
        {
            playerTransform.position = teleportTarget.position;
            playerTransform.rotation = teleportTarget.rotation;
            Debug.Log("Player teleported successfully.");
        }
        else
        {
            Debug.LogWarning("Teleport target or player transform is missing.");
        }
    }
}
