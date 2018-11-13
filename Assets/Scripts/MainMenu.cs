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
        PlayConfirmSound();
        confirmPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(GameObject.Find("Yes"));
    }

    public void ConfirmQuit()
    {
        PlayConfirmSound();
        Application.Quit();
    }

    public void OpenOptions()
    {
        PlayConfirmSound();

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
