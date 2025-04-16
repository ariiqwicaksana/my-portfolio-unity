using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SpectatorCamera : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float moveSpeed = 10f;
    public CinemachineVirtualCamera virtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))  // Check if the right mouse button is pressed
        {
            HandleCameraRotation();
            HandleCameraMovement();
        }
    }

    void HandleCameraRotation()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
        float vertical = Input.GetAxis("Mouse Y") * rotationSpeed;

        if (virtualCamera != null)
        {
            Transform camTransform = virtualCamera.transform;
            camTransform.Rotate(Vector3.up, horizontal, Space.World);
            camTransform.Rotate(Vector3.right, -vertical, Space.Self);
        }
    }

    void HandleCameraMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        Vector3 move = new Vector3(moveX, 0, moveZ);

        if (virtualCamera != null)
        {
            virtualCamera.transform.position += virtualCamera.transform.TransformDirection(move);
        }
    }
}
