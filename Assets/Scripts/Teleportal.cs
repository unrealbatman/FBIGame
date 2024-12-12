using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportal : MonoBehaviour, IExaminable
{
    [Header("Teleport Settings")]
    [SerializeField] private float teleportDelay = 10f;
    public bool isTeleporting = false;

    public void Interact()
    {
        Debug.Log($"Teleporting player...");
        if (isTeleporting) return;

        StartCoroutine(TeleportPlayer());
    }


    private IEnumerator TeleportPlayer()
    {
        isTeleporting = true;

        yield return new WaitForSeconds(teleportDelay);

        // Teleport player
        Transform playerTransform = FindObjectOfType<PlayerController>().transform;
        if (playerTransform != null)
        {
            playerTransform.SetPositionAndRotation(this.gameObject.transform.position, this.gameObject.transform.rotation);

            Debug.Log("Player teleported successfully.");
        }
        else
        {
            Debug.LogWarning("Teleport target or player transform is missing.");
        }

        isTeleporting=false;
       
    }
}
