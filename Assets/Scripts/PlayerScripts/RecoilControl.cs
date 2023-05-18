using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilControl : MonoBehaviour
{
    public float recoilBackSpeed;

    private void Update() {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, recoilBackSpeed / 100);
    }
}
