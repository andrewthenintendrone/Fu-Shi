using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Diagnostics;

public class MainMenu : MonoBehaviour
{
    // child buttons
    public Button[] buttons;

    public int selectedButton = 0;

    public int currentVertical;
    public int previousVertical;

    private void Start()
    {
        buttons = GetComponentsInChildren<Button>();
    }

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical");

        if(Mathf.Abs(vertical) > 0.5f)
        {
            currentVertical = (int)Mathf.Sign(vertical);
        }
        else
        {
            currentVertical = 0;
        }

        if (currentVertical != previousVertical)
        {
            selectedButton -= currentVertical;

            previousVertical = currentVertical;

            // wrap
            if (selectedButton < 0)
            {
                selectedButton = buttons.Length - 1;
            }
            else if (selectedButton >= buttons.Length)
            {
                selectedButton = 0;
            }
        }

        // select buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == selectedButton)
            {
                buttons[selectedButton].Select();
            }
        }

        // jump button confirms
        if (Input.GetAxisRaw("Jump") > 0)
        {
            switch (selectedButton)
            {
                case 0:
                    SaveLoad.deleteSave();
                    SceneManager.LoadScene(1);
                    break;
                case 1:
                    SceneManager.LoadScene(1);
                    break;
                case 2:
                    Application.Quit();
                    break;
                case 3:
                    var processInfo = new ProcessStartInfo("C:/Program Files (x86)/Google/Chrome/Application/chrome.exe", "https://www.youtube.com/watch?v=oHg5SJYRHA0");
                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;

                    var process = Process.Start(processInfo);

                    process.WaitForExit();
                    process.Close();
                    break;
                default:
                    break;
            }
        }
    }
}
