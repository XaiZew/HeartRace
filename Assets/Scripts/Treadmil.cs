using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmil : MonoBehaviour
{
    [SerializeField] Vector3[] allPos;
    Vector3 targetPos;
    int currPosCount;

    public float duration;

    private void OnTriggerEnter(Collider other) {
        currPosCount = 0;
        if (other.CompareTag("Player"))
            StartCoroutine(TreadmilRun(other));
    }
    
    IEnumerator TreadmilRun(Collider other) {
        float timer = 0;
        targetPos = allPos[currPosCount];
        Vector3 startPos = transform.position;

        while (timer < duration) {
            other.transform.position = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
            transform.position = Vector3.Lerp(startPos, targetPos, timer / duration);
            timer += 1 * Time.deltaTime;
            yield return null;
        }
        currPosCount++;
        if (currPosCount < allPos.Length) {
            StartCoroutine(TreadmilRun(other));
        }
    }
}
