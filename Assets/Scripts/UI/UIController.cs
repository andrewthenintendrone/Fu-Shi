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
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject confirmationScreen;
    [SerializeField]
    private GameObject controlsPanel;

    private GameObject autoSaveIcon;

    // are we currently showing a notification
    private bool showingNotification = false;

    private GameObject notification = null;

    [SerializeField]
    private GameObject[] abilityPanels;

    [SerializeField]
    private GameObject notificationPrefab;

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

    [SerializeField]
    private AudioClip selectSound;

    [SerializeField]
    private AudioClip confirmSound;

    private void Start()
    {
        pauseScreen.SetActive(false);
        confirmationScreen.SetActive(false);
        controlsPanel.SetActive(false);
        autoSaveIcon = GameObject.Find("FakeSaveIcon");
        autoSaveIcon.SetActive(false);
    }

    void Update ()
    {
        SetPauseScreen();
	}

    private void SetPauseScreen()
    {
        if ((int)Input.GetAxisRaw("Pause") != 0 && !showingNotification)
        {
            if (!pauseAxisHeld)
            {
                Utils.gamePaused = !Utils.gamePaused;

                pauseScreen.SetActive(Utils.gamePaused);

                if(Utils.gamePaused)
                {
                    eventSystem.SetSelectedGameObject(GameObject.Find("resume button"));
                    GameObject.Find("resume button").GetComponent<Button>().OnSelect(null);

                    if(SaveLoad.saveData != null)
                    {
                        abilityPanels[0].SetActive(GameObject.FindObjectOfType<Abilityactivator>().hasInkAbility);
                        abilityPanels[1].SetActive(GameObject.FindObjectOfType<Abilityactivator>().hasTimeAbility);
                        abilityPanels[2].SetActive(Utils.maxHealth == 6);
                    }
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

        if (showingNotification && Input.GetAxisRaw("Back") == 1)
        {
            exitNotification();
        }
    }

    void ClosePanels()
    {
        controlsPanel.SetActive(false);
        confirmationScreen.SetActive(false);
    }

    public void Resume()
    {
        PlayConfirmSound();

        Utils.gamePaused = false;

        ClosePanels();

        pauseScreen.SetActive(false);
    }

    public void showControls()
    {
        PlayConfirmSound();
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
        PlayConfirmSound();
        currentExitType = EXITTYPE.DESKTOP;
        confirmationScreen.SetActive(true);
        GameObject.Find("Yes").GetComponent<Button>().Select();
    }

    public void QuitToMenu()
    {
        PlayConfirmSound();
        currentExitType = EXITTYPE.MENU;
        confirmationScreen.SetActive(true);
        GameObject.Find("Yes").GetComponent<Button>().Select();
    }

    public void ConfirmExit()
    {
        PlayConfirmSound();
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
        PlayConfirmSound();
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

    public void showNotification(string messageText, string confirmText)
    {
        Utils.gamePaused = true;
        showingNotification = true;

        notification = Instantiate(notificationPrefab, transform);

        notification.GetComponentsInChildren<Text>()[0].text = confirmText;
        notification.GetComponentsInChildren<Text>()[1].text = messageText;
    }

    public void exitNotification()
    {
        PlayConfirmSound();
        Utils.gamePaused = false;
        showingNotification = false;

        Destroy(notification);
    }

    public void PlaySelectSound()
    {
        GetComponent<AudioSource>().PlayOneShot(selectSound);
    }

    public void PlayConfirmSound()
    {
        GetComponent<AudioSource>().PlayOneShot(confirmSound);
    }
}
