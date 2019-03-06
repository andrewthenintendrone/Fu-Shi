using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject confirmPanel;

    private EventSystem eventSystem;

    [SerializeField]
    private AudioClip selectSound;

    [SerializeField]
    private AudioClip confirmSound;

    private void Start()
    {
        eventSystem = GameObject.FindObjectOfType<EventSystem>();
        confirmPanel.SetActive(false);
        InitDiscord();
    }

    // starts discord rich presence
    void InitDiscord()
    {
        if(Utils.startTime == 0)
        {
            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            Utils.startTime = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        }

        DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();
        DiscordRpc.Initialize("529846377602351112", ref handlers, true, "");

        Utils.UpdateDiscordPresence();
    }

    public void NewGame()
    {
        PlayConfirmSound();
        SaveLoad.deleteSave();
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        PlayConfirmSound();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        DiscordRpc.Shutdown();
        Utils.UpdateDiscordPresence();
        PlayConfirmSound();
        confirmPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(GameObject.Find("Yes"));
    }

    public void ConfirmQuit()
    {
        PlayConfirmSound();
        Application.Quit();
    }

    public void CancelQuit()
    {
        PlayConfirmSound();
        confirmPanel.SetActive(false);
        eventSystem.SetSelectedGameObject(GameObject.Find("NewGameButton"));
    }

    public void PlaySelectSound()
    {
        FindObjectOfType<AudioSource>().PlayOneShot(selectSound);
    }

    public void PlayConfirmSound()
    {
        FindObjectOfType<AudioSource>().PlayOneShot(confirmSound);
    }
}
