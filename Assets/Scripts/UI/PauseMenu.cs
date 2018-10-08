using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {


	void Start ()
    {
		
	}
	

	void Update ()
    {
		
	}



    public void Unpause()
    {
        Utils.gamePaused = false;
    }
 

    public void QuitToMenu()
    {
        //TODO create a confirmation box
    }

    public void quitToDesktop()
    {

    }
}
