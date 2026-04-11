using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Zuweisungen")]
    public CharacterController controller;
    public Transform cam;

    [Header("Movement Einstellungen")]
    public float speed = 6f;
    public float gravity = -25f;
    public float jumpHeight = 2.5f;

    [Header("Boden & Plattform Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    public bool isGrounded;
    private MovingPlatform activePlatform; // Speichert die aktuelle Plattform

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
        velocity.y = -2f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Boden-Check
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Eingabe & Kamera-relative Richtung
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 moveDir = Vector3.zero;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        // 3. Bewegungs-Ausführung (Normaler Walk)
        controller.Move(moveDir.normalized * speed * Time.deltaTime);

        // 4. PLATTFORM-LOGIK: Hier ziehen wir die Bewegung der Plattform mit ein
        CheckForPlatform();
        if (activePlatform != null)
        {
            // Wir bewegen den Controller zusätzlich mit der Geschwindigkeit der Plattform
            controller.Move(activePlatform.GetVelocity() * Time.deltaTime);
        }

        // 5. Springen
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 6. Schwerkraft
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void CheckForPlatform()
    {
        RaycastHit hit;
        // Schießt einen Strahl nach unten, um die Plattform zu finden
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundDistance + 0.2f))
        {
            MovingPlatform platform = hit.collider.GetComponent<MovingPlatform>();
            if (platform != null)
            {
                activePlatform = platform;
            }
            else
            {
                activePlatform = null;
            }
        }
        else
        {
            activePlatform = null;
        }
    }
}