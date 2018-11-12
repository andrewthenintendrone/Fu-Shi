using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Diagnostics;

#if UNITY_EDITOR

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
            SaveLoad.deleteSave();
            this.Close();
        }
        if (GUILayout.Button("No"))
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
            var processInfo = new ProcessStartInfo("C:/Program Files (x86)/Google/Chrome/Application/chrome.exe", "https://youtu.be/6n3pFFPSlW4");
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

#endif