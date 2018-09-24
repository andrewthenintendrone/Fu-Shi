using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Endstate : MonoBehaviour
{
    [SerializeField]
    private GameObject endScreen;
    

    private void Start()
    {
        endScreen.SetActive(false);
    }


    void Update ()
    {
        if (Utils.endstate)
        {
            showEndstate();
        }

	}


   private void showEndstate()
    {
        endScreen.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        Abilityactivator ability = coll.gameObject.GetComponent<Abilityactivator>();
        if (ability != null)
        {
            if (ability.hasInkAbility && ability.hasTimeAbility)
            {
                Utils.endstate = true;
            }
        }
    }

}
