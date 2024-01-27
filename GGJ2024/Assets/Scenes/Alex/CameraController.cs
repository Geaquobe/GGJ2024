using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private float xSensitivity = 100f;
    [SerializeField] private float ySensitivity = 100f;

    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform orientation;


    private float xRotation = 0f;
    private float yRotation = 0f;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        yRotation += mouseX;

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
