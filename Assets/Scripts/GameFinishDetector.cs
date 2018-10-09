using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishDetector : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D colider)
    {
        
        if (colider.gameObject.GetComponent<Abilityactivator>() != null )
        {
            Abilityactivator instance = colider.gameObject.GetComponent<Abilityactivator>();

            if (instance.hasTimeAbility && instance.hasInkAbility)
            {
                Utils.endstate = true;
            }
        }
    }

}
