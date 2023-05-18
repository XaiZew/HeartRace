using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class CollectibleScript : MonoBehaviour
{

    CollectibleManager collectibleManager;

    bool collectRange = false;
    
    void Start()
    {
        collectibleManager = GameObject.FindObjectOfType<CollectibleManager>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Yo yo yo this guy entere a collider ");

            collectRange = true;

        }
    
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Yo yo yo this guy left the collider");

            collectRange = false;

        }
    }
    
    

    void Update()
    {
         if(Input.GetKeyDown("e") && collectRange)
        {
            Debug.Log("This guy got a whoiile ass collectible yo");
            collectibleManager.GrabCollectible();
            Destroy(gameObject);
        }
    }
}
