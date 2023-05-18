using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // public float magni;
    // public float shakeTime;

    public IEnumerator ShakeCamera(float magnitude, float length) {
        float mag = magnitude;
        float t = 0;

        while (t < length) {
            transform.localPosition = new Vector3(Random.Range(-1f, 1f) * mag, Random.Range(-1f, 1f) * mag, 0);
            mag = Mathf.Lerp(magnitude, 0, t / length);
            t += 1 * Time.deltaTime;
            yield return null;
        }
        transform.localPosition = Vector3.zero;
    }
}
