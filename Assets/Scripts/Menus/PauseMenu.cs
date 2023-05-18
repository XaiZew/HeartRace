using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Scene pauseScene;
    GameManager gameManager;

    private void Start() {
        gameManager = GetComponent<GameManager>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.isActive) {
            if (SceneManager.GetSceneByName("PauseMenu").buildIndex == -1)
                SceneManager.LoadScene(1, LoadSceneMode.Additive);
        }
    }
}
