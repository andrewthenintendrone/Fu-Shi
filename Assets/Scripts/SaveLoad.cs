using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;

public static class SaveLoad
{
    [System.Serializable]
    public class SaveData
    {
        public Vector3 currentPosition;

        public int currentLevel;

        public SaveData()
        {
            currentPosition = Vector3.zero;
            currentLevel = 0;
        }
    }

    public static SaveData saveData = new SaveData();

    public static bool Save()
    {
        Debug.Log("attempting to save: C:/Users/s170837/Desktop/saveGame.fox");

        // store level name and position
        saveData.currentLevel = SceneManager.GetActiveScene().buildIndex;
        saveData.currentPosition = Utils.resetPos;

        // serialize data to save file
        BinaryFormatter bf = new BinaryFormatter();

        // use surrogate selector (otherwise Vectors won't serialize)
        SurrogateSelector surrogateSelector = new SurrogateSelector();
        Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
        bf.SurrogateSelector = surrogateSelector;

        FileStream file = File.Create("C:/Users/s170837/Desktop/saveGame.fox");
        bf.Serialize(file, saveData);
        file.Close();

        return true;
    }

    public static bool Load()
    {
        Debug.Log("attempting to load: C:/Users/s170837/Desktop/saveGame.fox");

        // deserialize data from save file
        if (File.Exists("C:/Users/s170837/Desktop/saveGame.fox"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            // use surrogate selector (otherwise Vectors won't serialize)
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            bf.SurrogateSelector = surrogateSelector;

            FileStream file = File.Open("C:/Users/s170837/Desktop/saveGame.fox", FileMode.Open);
            saveData = (SaveData)bf.Deserialize(file);
            file.Close();

            Debug.Log("loaded");

            // load scene and place player
            //SceneManager.LoadScene(saveData.currentLevel);
            Utils.resetPos = saveData.currentPosition;
            Utils.resetPlayer();
            return true;
        }

        Debug.Log("failed to load a save game");
        return false;
    }
}
