using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuCanvas : MonoBehaviour, IPointerEnterHandler
{
    RectTransform cursor;

    PauseMenuCanvas[] allButtons;

    private void Start() {
        allButtons = FindObjectsOfType<PauseMenuCanvas>();
        cursor = transform.parent.Find("Cursor").GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
     {
        cursor.localPosition = new Vector3(cursor.localPosition.x, transform.GetComponent<RectTransform>().localPosition.y, cursor.localPosition.z);

        // foreach(PauseMenuCanvas script in allButtons) {
        //     script.StopAllCoroutines();
        // }

        // StartCoroutine(CursorMove(transform.GetComponent<RectTransform>().localPosition.y));
     }

     IEnumerator CursorMove(float targetPos) {
        float timer = 0;
        float startPos = cursor.localPosition.y;
        while (timer < .4f) {
            cursor.localPosition = new Vector3(cursor.localPosition.x, Mathf.Lerp(startPos, targetPos, timer / .4f), cursor.localPosition.z);
            timer += 1 * Time.deltaTime;
            yield return null;
        }
        cursor.localPosition = new Vector3(cursor.localPosition.x, targetPos, cursor.localPosition.z);
        yield return null;
     }
}
