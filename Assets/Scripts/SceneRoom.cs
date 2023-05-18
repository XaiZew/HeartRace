using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRoom : MonoBehaviour
{
    GameManager gm;
    public int targetScene;

    private void Start() {
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            StartCoroutine(ChangeRoom(targetScene));
        }
    }

    IEnumerator ChangeRoom(int i) {
        gm.StartCoroutine(gm.FadeInOut(true));
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(i);
    }
}
