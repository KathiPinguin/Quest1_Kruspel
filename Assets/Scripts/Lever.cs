using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    private bool on = false;
    private bool interpolating = false;
    private float currentInterpolationTime = 0.0f;
    public bool playerInRange;

    [Header("Einstellungen")]
    [SerializeField] private float switchTime = 0.5f;
    [SerializeField] private LayerMask layerMask;

    [Header("Rotationseinstellungen (in Grad)")]
    [SerializeField] private Vector3 rotationOn = new Vector3(0, 90, 0);
    [SerializeField] private Vector3 rotationOff = new Vector3(0, -90, 0);

    [Header("Zuweisungen")]
    [SerializeField] private GameObject leverHandle;

    [Header("Events")]
    [SerializeField] private UnityEvent onLeverActivated;
    [SerializeField] private UnityEvent onLeverDeactivated;

    void Start()
    {
        // Start Methode bleibt leer, da Tastenabfrage in Update() stattfindet
    }

    IEnumerator InterpolateLeverCoroutine()
    {
        this.interpolating = true;
        Quaternion startRotation = leverHandle.transform.localRotation;

        // Berechne die Ziel-Rotation aus den Vektoren
        Quaternion targetRotation = Quaternion.Euler(this.on ? this.rotationOn : this.rotationOff);

        this.currentInterpolationTime = 0.0f;
        while (this.currentInterpolationTime < this.switchTime)
        {
            float percentage = this.currentInterpolationTime / this.switchTime;

            // Weiche Drehung mittels Slerp
            var currentRotation = Quaternion.Slerp(startRotation, targetRotation, percentage);
            this.leverHandle.transform.localRotation = currentRotation;

            yield return null;
            this.currentInterpolationTime += Time.deltaTime;
        }

        this.leverHandle.transform.localRotation = targetRotation;
        this.interpolating = false;
        DoStuff();
    }

    private void DoStuff()
    {
        if (this.on)
            onLeverActivated.Invoke();
        else
            onLeverDeactivated.Invoke();
    }

    void ToggleLever()
    {
        if (!playerInRange)
        { return; }

        this.on = !this.on;
        this.StartCoroutine(this.InterpolateLeverCoroutine());
    }

    void Update()
    {
        // Wir nutzen "Fire1" (Standard: Linke Maustaste oder Gamepad A-Knopf) um keine Tasten im Code zu hardcodieren.
        if (Input.GetButtonDown("Fire1") && playerInRange && !interpolating)
        {
            ToggleLever();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            playerInRange = false;
        }
    }
}