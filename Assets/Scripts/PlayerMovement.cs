using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Zuweisungen")]
    public CharacterController controller;
    public Transform cam;

    [Header("Movement Einstellungen")]
    public float speed = 6f;
    public float gravity = -25f;
    public float jumpHeight = 2.5f;
    public float dampening = 0.1f;

    [Header("Boden & Plattform Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask platformMask;
    [Header("Staub Partikel")]
    public ParticleSystem dustParticlesLeft;  // NEU: Für links
    public ParticleSystem dustParticlesRight; // NEU: Für rechts
    private ParticleSystem.EmissionModule dustEmissionLeft;
    private ParticleSystem.EmissionModule dustEmissionRight;

    private Vector3 characterMovement;
    private Vector3 platformVelocity;
    private Vector3 characterGravity;
    private Vector3 velocity;

    public bool isGrounded;
    // Variablen für den Input
    private float horizontalInput;
    private float verticalInput;
    public bool jumpPressed;
    //private MovingPlatform activePlatform; // Speichert die aktuelle Plattform

    private InputAction movement;
    private InputAction jump;
    private Animator animator;
    public float characterMovementSpeed;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
        //velocity.y = -2f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        this.movement = InputSystem.actions.FindAction("Move");
        this.jump = InputSystem.actions.FindAction("Jump");
            animator = GetComponent<Animator>();
        if (dustParticlesLeft != null)
        {
            dustEmissionLeft = dustParticlesLeft.emission;
            dustEmissionLeft.enabled = true; // Zum Testen erst einmal auf true
        }
        if (dustParticlesRight != null)
        {
            dustEmissionRight = dustParticlesRight.emission;
            dustEmissionRight.enabled = true;
        }
    }

    
    void Update()
    {
        /*
        // 1. Eingaben in Update abfragen (Verhindert das Verschlucken von Sprüngen)
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
        */

        //jumpPressed = jump.WasPressedThisFrame();
        if (jump.WasPressedThisFrame())
        {
            jumpPressed = true;
        }
        horizontalInput = movement.ReadValue<Vector2>().x;
        verticalInput = movement.ReadValue<Vector2>().y;
    }
    
    
    void FixedUpdate()
    {
        // 2. Bewegungsrichtung ausrichten
        Vector3 inputRightDirection = cam.right;
        Vector3 inputForwardDirection = cam.forward;
        inputRightDirection.y = 0.0f;
        inputForwardDirection.y = 0.0f;
        inputRightDirection.Normalize();
        inputForwardDirection.Normalize();



        Vector3 moveDir = Vector3.zero;
        moveDir += inputRightDirection * horizontalInput;
        moveDir += inputForwardDirection * verticalInput;
        //moveDir.Normalize();

        // 3. Rotation des Charakters
        if (moveDir.sqrMagnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }

        // 4. Schwerkraft & Bodenabfrage
        if (controller.isGrounded)
        {
            this.characterGravity = Vector3.zero;
            this.velocity.y = -2f;
        }
        else
        {
            this.velocity.y += gravity * Time.fixedDeltaTime;
        }

        // 5. Bewegungsberechnung
        this.characterMovement = Vector3.zero;
        this.characterMovement += moveDir * this.speed * Time.fixedDeltaTime;
        this.characterMovement *= (1 - this.dampening);


        // 6. Springen
       
        if (jumpPressed && controller.isGrounded)
        {
            this.velocity.y = Mathf.Sqrt(this.jumpHeight * -2f * this.gravity);
            jumpPressed = false; // Zurücksetzen nach dem Sprung
        }
        isGrounded = controller.isGrounded;
        
        //jumpPressed = false; // Absichern für den Fall, dass der Input nicht sofort gelesen wird

        // 7. Plattformgeschwindigkeit
        this.GetPlatformVelocity();

        // 8. Ausführen der Bewegung (Ruckelfrei)
        var combinedMovement = this.characterMovement + this.platformVelocity * Time.fixedDeltaTime;
        combinedMovement += velocity * Time.fixedDeltaTime;

        controller.Move(combinedMovement);

        // 9. Animationen
        if (animator != null)
        {
            Vector2 movement = new Vector2(characterMovement.x, characterMovement.z);
            characterMovementSpeed = movement.magnitude / Time.fixedDeltaTime;
            animator.SetFloat("RunningSpeed", characterMovementSpeed);
            animator.SetBool("isJumping", !controller.isGrounded);
            animator.SetBool("isGrounded", controller.isGrounded);
        }
        // 10. Staub-Partikel aktivieren, wenn sich der Spieler bewegt und auf dem Boden ist
        // Partikel-Steuerung für beide Füße
        if (dustParticlesLeft != null && dustParticlesRight != null)
        {
            // Wenn der Charakter auf dem Boden ist und die Bewegung größer als Null ist
            bool isMoving = controller.isGrounded && moveDir.sqrMagnitude > 0.1f;

            dustEmissionLeft.enabled = isMoving;
            dustEmissionRight.enabled = isMoving;
        }
    }

    private void GetPlatformVelocity()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundDistance + 0.2f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Platforms"))
            {
                MovingPlatform platform = hit.collider.GetComponent<MovingPlatform>();
                if (platform != null)
                {
                    this.platformVelocity = platform.GetVelocity();
                    return;
                }
            }
        }
        this.platformVelocity = Vector3.zero;
    }

}