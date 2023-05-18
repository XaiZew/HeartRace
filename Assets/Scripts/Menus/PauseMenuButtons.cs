using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    public Button playBtn, resumeBtn, continueBtn, optionsBtn, menuBtn, quitButton;

    [SerializeField] AudioSource buttonSource;

    private void Start() {
        playBtn?.onClick.AddListener(() => Play());
        continueBtn?.onClick.AddListener(() => Continue());
        resumeBtn?.onClick.AddListener(() => StartCoroutine(Resume()));
        optionsBtn?.onClick.AddListener(() => Options());
        menuBtn?.onClick.AddListener(() => Menu());
        quitButton?.onClick.AddListener(() => Quit());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StartCoroutine(Resume());
        }
    }

    void Play() {
        buttonSource?.Play();
        PlayerPrefs.SetInt("SpawnAtCheckpoint", 0);
        SceneManager.LoadScene(3);
    }

    void Continue() {
        buttonSource?.Play();
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentScene"));
    }

    IEnumerator Resume() { // Unloads Pause Menu
        buttonSource?.Play();
        if (SceneManager.GetSceneByName("PauseMenu").buildIndex != -1) { // if pausemenu is loaded
            SpritePlayer spritePlayer = transform.parent.Find("PauseClose").GetComponent<SpritePlayer>();
            spritePlayer.reverse = false;
            spritePlayer.StartCoroutine(spritePlayer.PlayAnim());
            // yield return new WaitForSeconds(.45f);

            while (spritePlayer.playing)
                yield return null;

            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("PauseMenu").buildIndex);
        }
        yield return null;
    }

    void Options() { // Loads Options menu scene and unloads pause menu scene
        // SceneManager.LoadScene(3, LoadSceneMode.Additive);
        // for (int i = 0; i < SceneManager.sceneCount; i++)
        // Resume(); // Unload Pause Menu
    }

    void Menu() { // Loads Main Menu and unloads every other scene
        buttonSource?.Play();
        SceneManager.LoadScene(0);
    }

    void Quit() { // Quits the game
        buttonSource?.Play();
        Application.Quit();
    }
}
