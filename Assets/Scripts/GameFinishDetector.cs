using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Abilityactivator>() != null)
        {
            Abilityactivator instance = collider.gameObject.GetComponent<Abilityactivator>();

            if (instance.hasTimeAbility && instance.hasInkAbility)
            {
                Utils.loadScene("Ending");
            }
        }
    }
}
