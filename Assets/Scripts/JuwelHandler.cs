using UnityEngine;
using UnityEngine.Audio;

public class JuwelHandler : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 90f;
    [SerializeField]
    private AudioClip collectSound;
    [SerializeField]
    private AudioMixerGroup sfxGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var collectSoundGO = new GameObject("CollectSOund");
        collectSoundGO.transform.position = transform.position;
        var audioSource = collectSoundGO.AddComponent<AudioSource>();
        audioSource.clip = collectSound;
        audioSource.outputAudioMixerGroup = sfxGroup;
        audioSource.Play();
        Destroy(collectSoundGO, collectSound.length);

        UIManager.Instance.victroy();
        Destroy(this.gameObject);
        
    }
}
