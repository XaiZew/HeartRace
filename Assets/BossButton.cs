using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossButton : MonoBehaviour
{

    bool buttonPressed = false;

    bool buttonRange = false;

    BossHealthManager healthmanager;

    // Start is called before the first frame update
    void Start()
    {
        healthmanager = GameObject.FindObjectOfType<BossHealthManager>();
    }

    void Update()
    {
         if(Input.GetKeyDown("e") && buttonRange && buttonPressed == false)
        {
            PressButton();
        }
    }

    void PressButton()
    {
        healthmanager.BossLoseHealth();
        buttonPressed = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            buttonRange = true;

        }
    
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {

            buttonRange = false;

        }
    }
}
