using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoint : MonoBehaviour
{
    Transform cam = GameObject.Find("CameraHolder").transform.Find("Main Camera");

    private void Update() {
        transform.LookAt(cam.forward);
    }
}
