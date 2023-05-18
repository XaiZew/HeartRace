using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    GameManager gameManager;

    public Vector3 lastCheckPoint;
    public int checkpointSceneIndex;

    [SerializeField] AudioSource deathSource;

    [HideInInspector] public string lastCheckPointX = "LCPX";
    [HideInInspector] public string lastCheckPointY = "LCPY";
    [HideInInspector] public string lastCheckPointZ = "LCPZ";

    private void Awake() {
        gameManager = GetComponent<GameManager>();
        lastCheckPoint = new Vector3(PlayerPrefs.GetFloat(lastCheckPointX), PlayerPrefs.GetFloat(lastCheckPointY), PlayerPrefs.GetFloat(lastCheckPointZ));
        checkpointSceneIndex = PlayerPrefs.GetInt("CheckPointSceneBuildIndex");
    }

    void RetryButton() {
        SceneManager.LoadScene(0);
    }
    
    public float deathDuration;
    public IEnumerator Death() {
        gameManager.isActive = true;
        deathSource.Play();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        Debug.Log("Dead");
        yield return null;
    }
}
