using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishDetector : MonoBehaviour
{

    private void OnTriggerEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.GetComponent<Abilityactivator>() != null )
        {
            Abilityactivator instance = collision.gameObject.GetComponent<Abilityactivator>();
            if (instance.hasTimeAbility && instance.hasInkAbility)
            {
                Utils.endstate = true;
            }
            
        }
    }

}
