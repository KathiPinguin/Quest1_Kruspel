using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float platformSpeed = 2f;
    [SerializeField] private Vector3 start;
    [SerializeField] private Vector3 end;

    public bool isActive = false; // Steuert, ob die Plattform fährt

    private Vector3 lastPosition;
    private Vector3 velocity;

    void Start()
    {
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Wenn der Hebel nicht umgelegt wurde, berechne keine Bewegung
        if (!isActive)
        {
            velocity = Vector3.zero;
            return;
        }
        float pingPong = Mathf.PingPong(Time.time * platformSpeed, 1f);
        Vector3 nextPosition = Vector3.Lerp(start, end, pingPong);

        transform.localPosition = nextPosition;

        velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }

    public Vector3 GetVelocity() => velocity;
    public void setActive(bool state)
    {
        this.isActive = state;
    }
}