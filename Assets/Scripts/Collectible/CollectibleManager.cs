using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class CollectibleManager : MonoBehaviour
{
    
    public TextMeshProUGUI CollectibleUI;
    string collectiblecounterawesome = "Carrots";
    int CollectibleAmount = 0;

    void Start()
    {
        CollectibleAmount = PlayerPrefs.GetInt(collectiblecounterawesome);
        if (CollectibleUI)
            CollectibleUI.text = CollectibleAmount.ToString();
    }


    
    public void GrabCollectible()
    {
        CollectibleAmount++;
        if (CollectibleUI)
            CollectibleUI.text = CollectibleAmount.ToString();
        PlayerPrefs.SetInt(collectiblecounterawesome, CollectibleAmount);
        PlayerPrefs.Save();
    }
}
