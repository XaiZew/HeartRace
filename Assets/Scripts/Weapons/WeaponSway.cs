using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    GameManager gameManager;

    float sinTime;

    [Header("Sway Settings")]
    public float speed;
    public float mag;

    Vector3 startPos;

    private void Start() {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        startPos = transform.localPosition;
    }

    private void Update() {
        if (!gameManager.isActive) return;
        Vector3 inputVector = new Vector3(Input.GetAxisRaw("Vertical"), 0, Input.GetAxisRaw("Horizontal"));

        if (inputVector.magnitude > 0) { // If moving or trying to move
            transform.localPosition += FootStepMotion();
        }

        if (transform.localPosition != startPos) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, 1 * Time.deltaTime);
        }
    }

    Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * speed) * mag / 100f;
        pos.x += Mathf.Cos(Time.time * speed / 2) * mag / 50f;
        return pos;
    }
}
