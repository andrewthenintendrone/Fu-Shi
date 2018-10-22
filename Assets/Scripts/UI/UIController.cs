using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private GameObject endScreen;
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject confirmationScreen;
    [SerializeField]
    private GameObject controlsPanel;

    private GameObject autoSaveIcon;

    [SerializeField]
    [Tooltip("how long to show the fake autosave icon for")]
    private float autosaveTime;

    private bool pauseAxisHeld = false;

    enum EXITTYPE
    {
        MENU,
        DESKTOP
    }

    EXITTYPE currentExitType = EXITTYPE.MENU;

    private void Start()
    {
        endScreen.SetActive(false);
        pauseScreen.SetActive(false);
        confirmationScreen.SetActive(false);
        controlsPanel.SetActive(false);
        autoSaveIcon = GameObject.Find("FakeSaveIcon");
        autoSaveIcon.SetActive(false);
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

                pauseScreen.SetActive(Utils.gamePaused);

                if(Utils.gamePaused)
                {
                    eventSystem.SetSelectedGameObject(GameObject.Find("resume button"));
                    GameObject.Find("resume button").GetComponent<Button>().OnSelect(null);
                }
                else
                {
                    ClosePanels();
                }

                pauseAxisHeld = true;
            }          
        }
        else
        {
            pauseAxisHeld = false;
        }
    }

    void ClosePanels()
    {
        controlsPanel.SetActive(false);
        confirmationScreen.SetActive(false);
    }

    private void showEndstate()
    {
        endScreen.SetActive(true);
    }

    public void Resume()
    {
        Utils.gamePaused = false;

        ClosePanels();

        pauseScreen.SetActive(false);
    }

    public void showControls()
    {
        controlsPanel.SetActive(true);
        GameObject.Find("controls ok").GetComponent<Button>().Select();
    }

    public void hideControls()
    {
        controlsPanel.SetActive(false);
        GameObject.Find("resume button").GetComponent<Button>().Select();
    }

    public void QuitToDesktop()
    {
        currentExitType = EXITTYPE.DESKTOP;
        confirmationScreen.SetActive(true);
        GameObject.Find("Yes").GetComponent<Button>().Select();
    }

    public void QuitToMenu()
    {
        currentExitType = EXITTYPE.MENU;
        confirmationScreen.SetActive(true);
        GameObject.Find("Yes").GetComponent<Button>().Select();
    }

    public void ConfirmExit()
    {
        switch (currentExitType)
        {
            case EXITTYPE.MENU:
                Utils.gamePaused = false;
                Utils.loadScene("MainMenu");
                break;
            case EXITTYPE.DESKTOP:
                Utils.gamePaused = false;
                Utils.Exit();
                break;
            default:
                break;
        }
    }

    public void cancelExit()
    {
        GameObject.Find("resume button").GetComponent<Button>().Select();
        confirmationScreen.SetActive(false);
    }

    public void showFakeSave()
    {
        autoSaveIcon.SetActive(true);
        Invoke("hideFakeSaveIcon", autosaveTime);
    }

    private void hideFakeSaveIcon()
    {
        autoSaveIcon.SetActive(false);
    }
}
