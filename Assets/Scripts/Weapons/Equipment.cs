using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [SerializeField] Transform cam;

    [Header("Equipment Prefabs")]
    [SerializeField] GameObject phoneBomb;

    public float throwForce;

    int[] currentEquipment;

    private void Update() {
        my_input();
    }

    void my_input() {
        if (Input.GetKeyDown(KeyCode.G)) {
            GameObject pB = Instantiate(phoneBomb, cam.position, Quaternion.identity, transform);
            pB.transform.localPosition = new Vector3(pB.transform.localPosition.x, pB.transform.localPosition.y, cam.localPosition.z + 1);
            pB.GetComponent<Rigidbody>().AddForce(cam.forward * throwForce, ForceMode.Impulse);
            pB.transform.parent = null;
            pB.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
