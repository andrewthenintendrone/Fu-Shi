using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // start new game
    public void newGame()
    {
        Debug.Log("new game");
        SceneManager.LoadScene(1);
    }

    public void loadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void exitToDesktop()
    {
        Application.Quit();
    }
}
