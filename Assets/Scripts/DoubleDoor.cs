using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoor : MonoBehaviour
{
    [SerializeField] AudioSource doorSource;

    [SerializeField] Transform rDoor;
    [SerializeField] Transform lDoor;

    private void OnTriggerEnter(Collider other) {
        StopAllCoroutines();
        doorSource.pitch = 1f;
        doorSource.Play();
        StartCoroutine(LerpAxis(rDoor, -2.75f, .4f));
        StartCoroutine(LerpAxis(lDoor, 2.75f, .4f));
    }

    private void OnTriggerExit(Collider other) {
        StopAllCoroutines();
        doorSource.pitch = -1f;
        doorSource.Play();
        StartCoroutine(LerpAxis(rDoor, 0, .4f));
        StartCoroutine(LerpAxis(lDoor, 0, .4f));
    }

    IEnumerator LerpAxis(Transform door, float targetPos, float duration) {
        float timer = 0;
        float startPos = door.localPosition.x;

        while (timer < duration) {
            door.localPosition = new Vector3(Mathf.Lerp(startPos, targetPos, timer / duration), 0, 0);
            timer += 1 * Time.deltaTime;
            yield return null;
        }
        door.localPosition = new Vector3(targetPos, 0, 0);
        yield return null;
    }
}
