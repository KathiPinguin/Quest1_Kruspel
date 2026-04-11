using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    // Das Feld für den sicheren Punkt im Inspector
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        // Wir prüfen: Hat das Objekt den Tag "Player"?
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null)
            {
                // Wir rufen die Logik auf
                ExecuteRespawn(controller);
            }
        }
    }

    private void ExecuteRespawn(CharacterController controller)
    {
        // 1. Controller kurz ausschalten
        controller.enabled = false;

        // 2. Teleport zum sicheren Punkt
        if (respawnPoint != null)
        {
            controller.transform.position = respawnPoint.position;
            Debug.Log("Mage wurde zu " + respawnPoint.name + " teleportiert!");
        }
        else
        {
            Debug.LogError("Kein RespawnPoint im Inspector zugewiesen!");
        }

        // 3. Controller wieder einschalten
        controller.enabled = true;
    }
}