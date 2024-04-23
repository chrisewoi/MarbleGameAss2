using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;
    public Transform cameraPivot;
    public Vector3 offset;
    public Transform player;
    Vector2 angles;
    public float sensitivity;
    public float yRotationLimit;

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Subscribe_MouseInput(MouseInputHandler);
    }

    // Update is called once per frame
    void Update()
    {
        cameraPivot.position = player.position;
        mainCamera.transform.localPosition = offset;
    }

    void MouseInputHandler(Vector2 mouseScreenPos, Vector3 mouseWorldPos, Vector2 mouseDelta)
    {
        angles.x += mouseDelta.x * sensitivity;
        angles.y += mouseDelta.y * sensitivity;
        angles.y = Mathf.Clamp(angles.y, -yRotationLimit, yRotationLimit);
        Quaternion rotation = Quaternion.AngleAxis(angles.x, Vector3.up) * Quaternion.AngleAxis(angles.y, Vector3.left);
        cameraPivot.localRotation = rotation;
    }
}
