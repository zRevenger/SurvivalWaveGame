using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private float lookSensitivity;

    public Camera camRef;

    private float rotX;

    private void Start()
    {
        camRef.gameObject.SetActive(false);
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

            //look up and down
            rotX -= mouseY;
            rotX = Mathf.Clamp(rotX, -85, 85);
            camRef.transform.localEulerAngles = new Vector3(rotX, 0, 0);

            //rotate
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}