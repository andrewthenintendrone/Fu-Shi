﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.EventSystems;

public class EndingMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject confirmPanel;

    private EventSystem eventSystem;


    private void Start()
    {
        eventSystem = GameObject.FindObjectOfType<EventSystem>();
        confirmPanel.SetActive(false);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        confirmPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(GameObject.Find("Yes"));
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }

    public void CancelQuit()
    {
        confirmPanel.SetActive(false);
        eventSystem.SetSelectedGameObject(GameObject.Find("BackToMenuButton"));
    }
}