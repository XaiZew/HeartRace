using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    DeathManager deathManager;

    private void Start() {
        deathManager = FindObjectOfType<DeathManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Deathbox")) {
            deathManager.StartCoroutine(deathManager.Death());
        }
    }
}
