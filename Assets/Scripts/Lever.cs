using UnityEngine;

public class Lever : MonoBehaviour
{
    public MovingPlatform targetPlatform;
    public Transform leverHandle;

    public bool isActivated = false;
    private bool playerInRange = false;

    // Wir definieren die Winkel fest
    [SerializeField] private Vector3 rotationOn = new Vector3(0, 90f, 0);
    [SerializeField] private Vector3 rotationOff = new Vector3(0, -90f, 0);

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleLever();
        }
    }

    void ToggleLever()
    {
        isActivated = !isActivated; // Das wechselt true/false bei jedem Klick

        if (targetPlatform != null)
        {
            targetPlatform.isActive = isActivated;
        }

        if (leverHandle != null)
        {
            // Wir setzen die Z-Rotation direkt
            float zAngle = isActivated ? 50f : -50f;
            leverHandle.localEulerAngles = new Vector3(0, 0, zAngle);
        }

        Debug.Log("Hebel Zustand: " + isActivated);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}