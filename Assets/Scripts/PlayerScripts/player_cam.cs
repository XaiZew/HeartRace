using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_cam : MonoBehaviour
{
    GameManager gameManager;

    public float sensX;
    public float sensY;

    float mouseX;
    float mouseY;

    float xRotation;
    float yRotation;

    [SerializeField] Transform orientation;
    [SerializeField] Transform camHolder;

    private void Start() {
        gameManager = GameManager.FindObjectOfType<GameManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yRotation = transform.rotation.eulerAngles.y;
    }

    private void Update() {
        if (!gameManager.isActive) return;
        mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        camHolder.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
