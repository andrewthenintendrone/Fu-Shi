﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public class SaveDeleter : EditorWindow
{
    [MenuItem("Tools/Delete Save Data")]
    static void Init()
    {
        SaveDeleter window = ScriptableObject.CreateInstance<SaveDeleter>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        window.ShowPopup();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("This will delete your save file. Are you sure?", EditorStyles.wordWrappedLabel);
        GUILayout.Space(70);
        if (GUILayout.Button("Yes"))
        {
            if (File.Exists(Application.persistentDataPath + "/saveGame.fox"))
            {
                File.Delete(Application.persistentDataPath + "./saveGame.fox");
            }
            this.Close();
        }
        if(GUILayout.Button("No"))
        {
            this.Close();
        }
    }
}

public class EverythingDeleter : EditorWindow
{
    [MenuItem("Tools/Delete Entire Project")]
    static void Init()
    {
        EverythingDeleter window = ScriptableObject.CreateInstance<EverythingDeleter>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        window.ShowPopup();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("This will delete the entire project and all backups. Are you sure?", EditorStyles.wordWrappedLabel);
        GUILayout.Space(70);
        if (GUILayout.Button("Yes"))
        {
            var processInfo = new ProcessStartInfo("C:/Program Files (x86)/Google/Chrome/Application/chrome.exe", "https://www.youtube.com/watch?v=oHg5SJYRHA0");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;

            var process = Process.Start(processInfo);

            process.WaitForExit();
            process.Close();

            this.Close();
        }
        if (GUILayout.Button("No"))
        {
            this.Close();
        }
    }
}