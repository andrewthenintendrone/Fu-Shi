using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.EventSystems;

public class EndingMenu : MonoBehaviour
{
    #region variables

    [SerializeField]
    private GameObject confirmPanel;

    [SerializeField]
    private AudioClip selectSound;

    [SerializeField]
    private AudioClip confirmSound;

    private EventSystem eventSystem;

    #endregion

    #region monobehavior

    private void Start()
    {
        eventSystem = GameObject.FindObjectOfType<EventSystem>();
        confirmPanel.SetActive(false);
        Utils.UpdateDiscordPresence();
    }

    public void BackToMenu()
    {
        PlayConfirmSound();
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        PlayConfirmSound();
        confirmPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(GameObject.Find("Yes"));
    }

    public void CancelQuit()
    {
        PlayConfirmSound();
        confirmPanel.SetActive(false);
        eventSystem.SetSelectedGameObject(GameObject.Find("BackToMenuButton"));
    }

    public void ConfirmQuit()
    {
        PlayConfirmSound();
        Application.Quit();
    }

    public void PlaySelectSound()
    {
        GetComponent<AudioSource>().PlayOneShot(selectSound);
    }

    public void PlayConfirmSound()
    {
        GetComponent<AudioSource>().PlayOneShot(confirmSound);
    }

    #endregion
}
