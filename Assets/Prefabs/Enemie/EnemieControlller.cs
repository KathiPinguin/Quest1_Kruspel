
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Audio;

public class EnemieControlller : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float waitTimeAtWaypoint = 2f;
    [SerializeField] private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private bool isSquashing;
    public Vector3 squashFactor = new Vector3(1.5f, 0.2f, 1.5f);
    public float squashDuration = 0.1f;
    public float headHeight = 0.5f;
    [Header("Audio")]
    [SerializeField] private AudioClip squashSound;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        if (waypoints.Count == 0) return;

        Vector3 targetPos = waypoints[currentWaypointIndex].position;
        Vector3 moveDirection = (targetPos - transform.position);
        moveDirection.y = 0;

        TurnTowardsWaypoint(moveDirection);

        if (isWaiting) return;

        MoveTowardsWaypoint(targetPos, moveDirection);

    }

    void TurnTowardsWaypoint(Vector3 moveDirection)
    {
      
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void MoveTowardsWaypoint(Vector3 targetPos, Vector3 moveDirection)
    {
       
        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
        
        transform.position = newPos;

        // Prüfen, ob Wegpunkt erreicht
        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }
    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
       
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;

        yield return new WaitForSeconds(waitTimeAtWaypoint);

 
        isWaiting = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSquashing)
        {
            Debug.Log("I wana squash other:" + (other.transform.position.y - transform.position.y));
            // Prüfen ob der Spieler von oben kommt (einfacher Check)
            if ((other.transform.position.y - transform.position.y) > headHeight)
            {
                StartCoroutine(SquashSequence());
            }
        }
    }

    IEnumerator SquashSequence()
    {
        isWaiting = true;
        isSquashing = true;
        // Soundeffekt abspielen, wenn vorhanden
        if (squashSound != null)
        {
            var go = new GameObject("SquashSoundObject");
            go.transform.position = transform.position;
            var source = go.AddComponent<AudioSource>();
            source.clip = squashSound;

            if (sfxMixerGroup != null)
            {
                source.outputAudioMixerGroup = sfxMixerGroup;
            }

            source.Play();
            Destroy(go, squashSound.length);
        }

        Vector3 targetScale = Vector3.Scale(transform.localScale, squashFactor);
        yield return StartCoroutine(ScaleOverTime(transform.localScale, targetScale, squashDuration));
        yield return new WaitForSecondsRealtime(0.5f); // Kurze Pause bevor der Gegner verschwindet
        //Delete oder Deaktivieren des Gegners hier
        Destroy(this.gameObject);

     
    }

    IEnumerator ScaleOverTime(Vector3 start, Vector3 end, float time)
    {
        float elapsed = 0;
        while (elapsed < time)
        {
            transform.localScale = Vector3.Lerp(start, end, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = end;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
