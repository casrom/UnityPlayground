using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{

    public float sensitivity = 100f;

    public Transform playerBody;
    float xRotation;
    private Vector2 m_Look;
    public Vector2 m_Rotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    public void Update()
    {
        //float mouseX = m_Look.x * sensitivity * Time.deltaTime;
        //float mouseY = m_Look.y * sensitivity * Time.deltaTime;

        //playerBody.transform.Rotate(Vector3.up * mouseX);

        //xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        Look(m_Look);
    }
    public void OnLook(InputAction.CallbackContext context) {
        m_Look = context.ReadValue<Vector2>();
    }


    public void Look(Vector2 rotate) {

        if (rotate.sqrMagnitude < 0.01)
            return;
        var scaledRotateSpeed = sensitivity * Time.deltaTime;
        //m_Rotation.y += rotate.x * scaledRotateSpeed;

        playerBody.transform.Rotate(Vector3.up * rotate.x * scaledRotateSpeed);

        m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89, 89);
        transform.localEulerAngles = m_Rotation;
    }

}
