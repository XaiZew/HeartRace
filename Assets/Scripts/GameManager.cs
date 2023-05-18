using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // When we want things to run
    public bool isActive = true;

    public Image sceneChangeImg;
    public float sceneFadeLength;

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Additive) {
            isActive = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Physics.gravity = new Vector3(0, 0, 0);
        }
        else {
            Debug.Log(PlayerPrefs.GetInt("CurrentScene"));
            if (SceneManager.GetActiveScene().buildIndex > PlayerPrefs.GetInt("CurrentScene")) {
                PlayerPrefs.SetInt("CurrentScene", SceneManager.GetActiveScene().buildIndex);
            }

            isActive = true;
            Physics.gravity = new Vector3(0, -9.81f, 0);
            sceneChangeImg.color = new Color32(0, 0, 0, 0);
            StartCoroutine(FadeInOut(false));
        }
    }

    void OnSceneUnloaded(Scene scene) {
        isActive = true;
        Physics.gravity = new Vector3(0, -9.81f, 0);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start() {
        if (SceneManager.GetActiveScene().buildIndex > PlayerPrefs.GetInt("CurrentScene")) {
            PlayerPrefs.SetInt("CurrentScene", SceneManager.GetActiveScene().buildIndex);
        }
    }

    public IEnumerator FadeInOut(bool bo) {
        float t = 0;
        int newOpacity = 0;
        while (t < sceneFadeLength) {
            if (bo)
                newOpacity = (int)Mathf.Lerp(0, 255, t / sceneFadeLength);
            else
                newOpacity = (int)Mathf.Lerp(255, 0, t / sceneFadeLength);
            
            sceneChangeImg.color = new Color32(0, 0, 0, (byte)newOpacity);
            t += 1 * Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
