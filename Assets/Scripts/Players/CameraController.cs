using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private float lookSensitivity;

    public Camera camRef;
    public float fovIncreaseOnSprint;

    private float rotX;

    private float defaultFov;

    private void Start()
    {
        camRef.gameObject.SetActive(false);
        defaultFov = camRef.fieldOfView;
    }

    private bool hasInitialized;

    private void Update()
    {
        if(GetComponent<PlayerObjectController>().IsInGameScene())
        {
            if (!hasInitialized && GetComponent<PlayerObjectController>().playerModel.activeSelf && hasAuthority)
            {
                camRef.gameObject.SetActive(true);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                hasInitialized = true;
            }

            //Mouse Values
            float mouseX = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;

            //set fov when sprinting
            if (Input.GetButton("Sprint") && GetComponent<PlayerMovement>().isGrounded)
            {
                camRef.fieldOfView = Mathf.Lerp(camRef.fieldOfView, defaultFov + fovIncreaseOnSprint, 5f * Time.deltaTime);
            }
            else
            {
                if (camRef.fieldOfView != defaultFov)
                    camRef.fieldOfView = Mathf.Lerp(camRef.fieldOfView, defaultFov, 5f * Time.deltaTime);
            }

            //look up and down
            rotX -= mouseY;
            rotX = Mathf.Clamp(rotX, -85, 85);
            camRef.transform.localEulerAngles = new Vector3(rotX, 0, 0);

            //rotate
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}