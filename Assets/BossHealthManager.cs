using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossHealthManager : MonoBehaviour
{

    //Health currently Displayed
    public float HealthUI = 1f;
    //Health that was previously displayed so i can transition between
    public float OldHealthUI = 1f;
    // Maximum amount of health
    float MaxHealth = 100f;
    // Current amount of health
    float CurrentHealth = 0f;

    float healthDuration = 1f;

    public Slider healthSlider; 

    public GameObject bossCanvas;

    public GameObject bossMain;

    public GameObject bossdefeated; 

    HeartRate heartRate;

    BossManager bossmanager;

    BehaviourManager behaviourmanager;




    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        HealthUI = CurrentHealth / MaxHealth;
        OldHealthUI = HealthUI;

        heartRate = GameObject.FindObjectOfType<HeartRate>();
        bossmanager = GameObject.FindObjectOfType<BossManager>();
        behaviourmanager = GameObject.FindObjectOfType<BehaviourManager>();

    }

    void Update()
    {

    }

    // Called once 
    public void BossLoseHealth()
    {
        CurrentHealth = CurrentHealth - 25f;
        HealthUIUpdater();
        FormChecker();
    }

    void HealthUIUpdater()
    {
        HealthUI = CurrentHealth / MaxHealth;
        StartCoroutine(ChangeHealthUI(HealthUI,healthDuration));
        
    }

    public IEnumerator ChangeHealthUI(float healthUI, float duration) {
        float timer = 0;


    
        while (timer < duration) {
            healthSlider.value = Mathf.Lerp(OldHealthUI, healthUI, timer / duration);
            timer += 1 * Time.deltaTime;
            yield return null;
        }
        OldHealthUI = HealthUI;

        DeathChecker(); 
        
        yield return null;
        
    }

    void FormChecker()
    {
        if(CurrentHealth >= 50)
        {
            behaviourmanager.ChangePhase();
        }
    }

    void DeathChecker()
    {
        if (CurrentHealth <= 0 )
        {
            Die();
        }
    }

    void Die()
    {
        heartRate.enabled = false;
        bossdefeated.SetActive(true);
        Destroy(bossMain);
        Destroy(bossCanvas);
        
    }

}
