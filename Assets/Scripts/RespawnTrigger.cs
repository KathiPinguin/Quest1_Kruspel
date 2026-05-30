using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                //ExecuteRespawn(controller);
                playerMovement.kill();
            }
        }
    }

    private void ExecuteRespawn(CharacterController controller)
    {
        // 1. Sichere Position zuweisen
        if (respawnPoint != null)
        {
            // CharacterController kurz ausschalten, um Ruckler und Fehler zu vermeiden
            controller.enabled = false;
            controller.transform.position = respawnPoint.position;
            controller.enabled = true;
            Debug.Log("Respawn erfolgreich!");
        }
        else
        {
            Debug.LogError("Kein Respawn-Punkt zugewiesen!");
        }
    }
}