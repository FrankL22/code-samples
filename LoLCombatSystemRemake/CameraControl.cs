using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private static CameraControl Instance;
    public static CameraControl Get
    {
        get { return Instance; }
    }

    public float sensitivity;
    public float cameraMoveSpeed;
    public float borderFactor;

    private Vector3 moveVertical;
    private Vector3 moveHorizontal;

    private void Start()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Transform cameraTransform = transform.GetChild(0);
        Quaternion cameraRotation = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0);
        moveVertical = cameraRotation * Vector3.back;
        moveHorizontal = cameraRotation * Vector3.left;
    }

    private void Update()
    {
        // move camera view when mouse near border
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        if (mouseX < Screen.width * borderFactor)
        {
            transform.Translate(moveHorizontal * sensitivity * cameraMoveSpeed * Time.deltaTime);
        }
        else if (mouseX > Screen.width * (1 - borderFactor))
        {
            transform.Translate(-moveHorizontal * sensitivity * cameraMoveSpeed * Time.deltaTime);
        }

        if (mouseY < Screen.height * borderFactor)
        {
            transform.Translate(moveVertical * sensitivity * cameraMoveSpeed * Time.deltaTime);
        }
        else if (mouseY > Screen.height * (1 - borderFactor))
        {
            transform.Translate(-moveVertical * sensitivity * cameraMoveSpeed * Time.deltaTime);
        }
    }
}
