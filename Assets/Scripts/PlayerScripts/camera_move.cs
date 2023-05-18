using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_move : MonoBehaviour
{
    [SerializeField] Transform cameraPos;

    void Update() {
        transform.position = cameraPos.position;
    }
}
