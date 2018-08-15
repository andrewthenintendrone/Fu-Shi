using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    private Sprite[] healthImages;

    private void Start()
    {
        setHealth(3);
    }

    public void setHealth(int health)
    {
        if(health >= 0 && health < healthImages.Length)
        {
            GetComponent<Image>().sprite = healthImages[health];
        }
        else
        {
            Debug.Log("Tried to set health to " + health.ToString() + ". Was this a mistake?");
        }
    }
}
