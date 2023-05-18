using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltCamera : MonoBehaviour
{
    Camera cam;

    private void Start() {
        cam = GetComponent<Camera>();
    }

    public IEnumerator RotateCameraZ(float targetRot, float duration) {
        float t = 0;
        float startRot = transform.eulerAngles.z;
        float currRot;

        if (startRot > 180) {
            startRot = 360 - startRot;
        }

        while (t < duration) {
            currRot = Mathf.Lerp(startRot, targetRot, t / duration);
            cam.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, cam.transform.rotation.eulerAngles.y, currRot);
            t += 1 * Time.deltaTime;
            yield return null;
        }

        cam.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, cam.transform.rotation.eulerAngles.y, targetRot);
        yield return null;
    }
}