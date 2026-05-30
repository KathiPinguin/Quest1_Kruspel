using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(AudioSource))]
public class SawHandler : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damagePerSecond = 10;

    [Header("Rotation Settings")]
    [SerializeField]
    private float rotationSpeed = 400f;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioMixerGroup sfxMixerGroup;
    [SerializeField]
    private AudioClip idleSound;
    [SerializeField]
    private AudioClip cuttingSound;

    [Header("Particles")]
    [SerializeField]
    private ParticleSystem cuttingParticles;

    private bool isCutting;
    private bool playerInRange = false;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = sfxMixerGroup;
        audioSource.loop = true;
        audioSource.playOnAwake = true;

        isCutting = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.clip = idleSound;
        audioSource.Play();
        isCutting = false;
        if (cuttingParticles != null)
        {
            cuttingParticles.Stop();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        if ( !playerInRange && isCutting)
        {
            SetState(false);
        }

        playerInRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetState(false);
        }
    }
    //OnTriggerStay() is called every physics update - we use Time.fixedDeltaTime!
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var character = other.GetComponentInChildren<PlayerMovement>();
            character.InflictDamage(this.damagePerSecond * Time.fixedDeltaTime);
            playerInRange = true;
        }
    }

    private void SetState(bool cutting)
    {
        if (isCutting == cutting)
            return;
        if (cutting)
        {
            isCutting = true;
            audioSource.clip = cuttingSound;
            if (cuttingParticles != null)
            {
                cuttingParticles.Play();
            }
        }
        else
        {
            isCutting = false;
            audioSource.clip = idleSound;
            if (cuttingParticles != null)
            {
                cuttingParticles.Stop();
            }
        }
        audioSource.Play();
    }
}
