using JetBrains.Annotations;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class CharacterFootsteps : MonoBehaviour
{
    [Header("Audio Einstellungen")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    private PlayerMovement player;
    private AudioSource audioSource;
    private InputAction jumpAction; // Referenz auf die Sprung-Aktion

    private bool wasGrounded = true;
    private bool hasJumped = false; // Merker, ob aktiv gesprungen 
    private float stepCooldown = 0.4f;
    private float nextStepTime;

    void Start()
    {
        player = GetComponent<PlayerMovement>();

        // Audio Source Komponente sicherstellen
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.outputAudioMixerGroup = sfxMixerGroup;
        audioSource.loop = false;

        // Sprung-Aktion aus dem Input-System laden (wie in deinem Movement-Skript)
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        // 1. Footsteps Sound (Schleife oder zeitversetzte Abspielung bei Bewegung)
        Vector3 actVelocity = player.controller.velocity;
        actVelocity.y = 0; // Vertikale Geschwindigkeit ignorieren für die Fußschritte
        
        if (player.controller.isGrounded && actVelocity.sqrMagnitude > 0.1f)
        {
            if (Time.time >= nextStepTime)
            {
                if (footstepClip != null)
                {
                    UnityEngine.Debug.Log("Playing footstep sound" + player.velocity.sqrMagnitude);
                    audioSource.PlayOneShot(footstepClip);
                }
                nextStepTime = Time.time + stepCooldown;
            }
        }

        // 2. Aktiven Sprung erkennen
        if (player.controller.isGrounded && jumpAction.WasPressedThisFrame())
        {
            hasJumped = true;
            if (jumpClip != null)
            {
                audioSource.PlayOneShot(jumpClip);
            }
        }

        // Abgleich des Grounded-Status
        if (!player.controller.isGrounded)
        {
            wasGrounded = false;
        }

        // 3. Lande-Sound (nur abspielen wenn Boden berührt wird und gesprungen wurde
        if (player.controller.isGrounded && !wasGrounded)
        {
            if (hasJumped) 
            {
                if (landClip != null)
                {
                    audioSource.PlayOneShot(landClip);
                }
                hasJumped = false; // Zurücksetzen für den nächsten Sprung
            }
            wasGrounded = true;
        }
    }

    public void stopAll()
    {
        UnityEngine.Debug.Log("Stopping all sounds");
        audioSource.Stop();
        
    }
}