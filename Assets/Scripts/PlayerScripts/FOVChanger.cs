using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVChanger : MonoBehaviour
{
    Camera cam;

    private void Start() {
        cam = GetComponent<Camera>();
    }

    public IEnumerator ChangeFOV(float targetFOV, float duration) {
        float timer = 0;
        float startFOV = cam.fieldOfView;

        while (timer < duration) {
            cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, timer / duration);
            timer += 1 * Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = targetFOV;
        yield return null;
    }
}
