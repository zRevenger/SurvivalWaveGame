using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{

    public GameObject playerModel;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerModel.SetActive(false);
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "Map1")
        {
            if (!playerModel.activeSelf)
            {
                SetPosition();
                playerModel.SetActive(true);
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
        transform.position = new Vector3(Random.Range(-5, 5), 0.8f, Random.Range(-15, 7));
    }

    #region MovementVars
    [SerializeField] private float baseMovementSpeed = 10;
    [SerializeField] private float sprintSpeed = 20;
    [SerializeField] private float jumpForce = 5;
    private Rigidbody rb;
    #endregion
}
