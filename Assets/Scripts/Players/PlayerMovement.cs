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

            transform.position += (Input.GetButtonDown("Sprint") ? baseMovementSpeed : sprintSpeed) * transform.forward * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += baseMovementSpeed * transform.right * Input.GetAxis("Horizontal") * Time.deltaTime;

            if (Input.GetButtonDown("Jump"))
                rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        }
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5, 5), .8f, Random.Range(-15, 7));
    }

    #region MovementVars
    [SerializeField] private float baseMovementSpeed = 10;
    [SerializeField] private float sprintSpeed = 20;
    [SerializeField] private float jumpForce = 5;
    private Rigidbody rb;
    #endregion
}
