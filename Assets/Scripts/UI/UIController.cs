using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject endScreen;
    [SerializeField]
    private GameObject pauseScreen;

    private bool pauseAxisHeld = false;

    private void Start()
    {
        endScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }


    void Update ()
    {
        if (Utils.endstate)
        {
            showEndstate();
        }
        SetPauseScreen();

	}

    private void SetPauseScreen()
    {
        
        if ((int)Input.GetAxisRaw("Pause") != 0)
        {
            if (!pauseAxisHeld)
            {
                Utils.gamePaused = !Utils.gamePaused;
                pauseAxisHeld = true;
            }          
        }
        else
        {
            pauseAxisHeld = false;
        }


        if (Utils.gamePaused)
        {
            pauseScreen.SetActive(true);
        }
        else
        {
            pauseScreen.SetActive(false);
        }

    }

    private void showEndstate()
    {
        endScreen.SetActive(true);
    }

    //private void OnTriggerEnter2D(Collider2D coll)
    //{
    //    Abilityactivator ability = coll.gameObject.GetComponent<Abilityactivator>();
    //    if (ability != null)
    //    {
    //        if (ability.hasInkAbility && ability.hasTimeAbility && Utils.endstate != true)
    //        {
    //            Utils.endstate = true;
    //        }
    //    }
    //}

}
