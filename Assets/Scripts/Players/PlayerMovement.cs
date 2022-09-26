using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private bool hasInitialized;

    private bool startJump;

    private void Update()
    {
        if(GetComponent<PlayerObjectController>().IsInGameScene())
        {
            if (!hasInitialized && GetComponent<PlayerObjectController>().playerModel.activeSelf)
            {
                SetPosition();
                rb.useGravity = true;
                hasInitialized = true;
            }

            if (!hasAuthority) return;

            if (startJump && !isGrounded)
                startJump = false;

            if (inputAxis.magnitude == 0 && !startJump && isGrounded)
                rb.isKinematic = true;
            else
                rb.isKinematic = false;

            inputAxis.x = Input.GetAxisRaw("Horizontal");
            inputAxis.y = Input.GetAxisRaw("Vertical");

            moveDir = transform.forward * inputAxis.y + transform.right * inputAxis.x;
            shouldSprint = Input.GetButton("Sprint");

            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.35f);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                startJump = true;
                rb.isKinematic = false;
                rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
            }

            slopeDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);

            HandleDrag();
            SpeedControl();
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.25f + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
                return true;
        }
        return false;
    }

    private void HandleDrag()
    {
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    public void FixedUpdate()
    {
        if (GetComponent<PlayerObjectController>().IsInGameScene())
        {
            if (!hasAuthority) return;

            float movementSpeed = shouldSprint && isGrounded ? sprintSpeed : baseMovementSpeed;
            float airControl = isGrounded ? 1 : .4f;
            Vector3 direction = OnSlope() && isGrounded ? slopeDir.normalized : moveDir.normalized;

            rb.AddForce(direction * movementSpeed * 10f * airControl, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        float movementSpeed = shouldSprint && isGrounded ? sprintSpeed : baseMovementSpeed;

        if (flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVelocity.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5, 5), 0, Random.Range(-15, 7));
    }

    #region MovementVars
    [SerializeField] private float baseMovementSpeed = 10;
    [SerializeField] private float sprintSpeed = 20;
    [SerializeField] private float jumpForce = 5;
    private Rigidbody rb;

    [SerializeField] private float groundDrag = 6f;

    private bool shouldSprint;
    internal bool isGrounded;

    private Vector3 moveDir;
    private RaycastHit slopeHit;
    private Vector3 slopeDir;
    private Vector2 inputAxis;
    #endregion
}
