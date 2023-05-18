using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeartRate : MonoBehaviour
{
    GameManager gameManager;
    DeathManager deathManager;
    TextMeshProUGUI heartRateText;
    Rigidbody rb;

    float currentSpeed;

    [SerializeField] AudioSource heartbeatSource;
    [SerializeField] AudioSource lowHealthSource;

    [Header("Heart Rate Settings")]
    public float minSpeed;
    public float speedThreshold;
    public float heartRateVelMultiplier;
    public float heartSpeedChangeTime;

    float speedTimer = 0;
    public float speedTimerThreshold;
    
    float targetRate;
    float startRate;

    Image deathFade;
    float newOpacity = 0;

    private void Start() {
        deathManager = GameObject.FindObjectOfType<DeathManager>();
        gameManager = deathManager.GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        heartRateText = GameObject.Find("MainCanvas").transform.Find("ConstantCanvas").transform.Find("HUD").transform.Find("HeartRate").GetComponentInChildren<TextMeshProUGUI>();
        deathFade = GameObject.Find("MainCanvas").transform.Find("ScaleCanvas").transform.Find("DeathFade").GetComponent<Image>();
        heartbeatSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.isActive) return;
        currentSpeed = rb.velocity.magnitude;

        if (currentSpeed > speedThreshold) {
            // If Speed reaches speed threshold
            heartbeatSource.pitch = 1.5f;
            targetRate = 80;
            newOpacity = Mathf.Lerp(newOpacity, 0, heartSpeedChangeTime * Time.deltaTime);
            heartRateText.text = Mathf.Lerp(int.Parse(heartRateText.text), targetRate, heartSpeedChangeTime * Time.deltaTime).ToString("0");
        }
        else if (currentSpeed < minSpeed) {
            targetRate = 0;

            if (!lowHealthSource.isPlaying && Mathf.Lerp(60, targetRate, (speedTimer / speedTimerThreshold)) < 40)
                lowHealthSource.Play();

            heartbeatSource.pitch = Mathf.Lerp(1, 0, (speedTimer / speedTimerThreshold));
            heartRateText.text = Mathf.Lerp(60, targetRate, (speedTimer / speedTimerThreshold)).ToString("0");
            newOpacity = Mathf.Lerp(0, 255, speedTimer / speedTimerThreshold);
            // If Speed is at a low enough level
            speedTimer += 1 * Time.deltaTime;
            if (speedTimer >= speedTimerThreshold) {
                heartRateText.text = "0";
                deathManager.StartCoroutine(deathManager.Death());
            }
        }
        else {
            // If Speed is normal
            heartbeatSource.pitch = 1;
            newOpacity = Mathf.Lerp(newOpacity, 0, heartSpeedChangeTime * Time.deltaTime);
            heartRateText.text = Mathf.Lerp(int.Parse(heartRateText.text), targetRate, heartSpeedChangeTime * Time.deltaTime).ToString("0");
            speedTimer = 0;
            targetRate = 60;
        }

        // heartRateText.text = (rb.velocity.magnitude * heartRateVelMultiplier).ToString("0");

        // Fading screen when standing stil
        deathFade.color = new Color32(0, 0, 0, (byte)newOpacity);
    }
}
