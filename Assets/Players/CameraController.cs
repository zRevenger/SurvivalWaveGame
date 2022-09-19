using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float lookSensitivity;

    private float rotX;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Mouse Values
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;

        //look up and down
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -85, 85);
        transform.localEulerAngles = new Vector3(rotX, 0, 0);

        //rotate
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}