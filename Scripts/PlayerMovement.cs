using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [SerializeField] private Transform orientation;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody rb;
    private float moveSpeed = 60.0f;
    private float maxSpeed = 7.0f;
    private float stonePrimingMaxSpeed = 2.5f;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private bool grounded;
    private float groundedDrag = 5.0f;

    private float jumpForce = 8.0f;
    private float jumpCooldown = 0.25f;
    private float airMultiplier = 0.3f;
    private bool readyToJump = true;

    private void Start()
    {
        instance = this;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        //CHECK IF PLAYER IS TOUCHING THE GROUND
        grounded = Physics.Raycast(transform.position + new Vector3(0.0f, 0.2f, 0.0f), Vector3.down, 0.4f, groundMask);

        GetInput();
        SpeedControl();

        rb.linearDamping = grounded ? groundedDrag : 0.0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetInput()
    {
        //WASD
        horizontalInput = Player.instance.IsEnabled() ? Input.GetAxisRaw("Horizontal") : 0.0f;
        verticalInput = Player.instance.IsEnabled() ? Input.GetAxisRaw("Vertical") : 0.0f;
        Player.instance.UpdateMovementVector(horizontalInput, verticalInput);

        //JUMPING
        if (Input.GetKey(KeyCode.Space) && grounded && readyToJump && Player.instance.IsEnabled())
        {
            readyToJump = false;
            Jump();
            Invoke("ResetJump", jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = (orientation.forward * verticalInput) + (orientation.right * horizontalInput);

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0.0f, rb.linearVelocity.z);

        float cappedSpeed = StoneHandling.instance.IsPrimed() ? stonePrimingMaxSpeed : maxSpeed;

        if (flatVel.magnitude > cappedSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * cappedSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0.0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
