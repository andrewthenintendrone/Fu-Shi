﻿using System.Collections;
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
        public bool hasInkAbility;
        public bool hasTimeAbility;
        public List<bool> collectables;

        public SaveData()
        {
            currentPosition = Vector3.zero;
            hasInkAbility = false;
            hasTimeAbility = false;
            collectables = new List<bool>();
        }
    }

    public static SaveData saveData = new SaveData();

    public static bool Save()
    {
        // store position
        saveData.currentPosition = Utils.resetPos;

        // store which abilities 
        saveData.hasInkAbility = GameObject.FindGameObjectWithTag("Player").GetComponent<Abilityactivator>().hasInkAbility;
        saveData.hasTimeAbility = GameObject.FindGameObjectWithTag("Player").GetComponent<Abilityactivator>().hasTimeAbility;

        // find which collectables are active
        saveData.collectables.Clear();
        foreach(Transform currentCollectable in GameObject.Find("collectables").GetComponentInChildren<Transform>())
        {
            saveData.collectables.Add(currentCollectable.gameObject.activeSelf);
        }

        // serialize data to save file
        BinaryFormatter bf = new BinaryFormatter();

        // use surrogate selector (otherwise Vectors won't serialize)
        SurrogateSelector surrogateSelector = new SurrogateSelector();
        Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
        bf.SurrogateSelector = surrogateSelector;

        FileStream file = File.Create(Application.persistentDataPath + "/saveGame.fox");
        bf.Serialize(file, saveData);
        file.Close();

        return true;
    }

    public static bool Load()
    {
        // deserialize data from save file
        if (File.Exists(Application.persistentDataPath + "/saveGame.fox"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            // use surrogate selector (otherwise Vectors won't serialize)
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);
            bf.SurrogateSelector = surrogateSelector;

            FileStream file = File.Open(Application.persistentDataPath + "/saveGame.fox", FileMode.Open);
            saveData = (SaveData)bf.Deserialize(file);
            file.Close();

            // place player
            Utils.resetPos = saveData.currentPosition;
            Utils.resetPlayer();

            // set if abilities have been obtained
            GameObject.FindGameObjectWithTag("Player").GetComponent<Abilityactivator>().hasInkAbility = saveData.hasInkAbility;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Abilityactivator>().hasTimeAbility = saveData.hasTimeAbility;

            // destroy ability givers
            if(saveData.hasInkAbility)
            {
                GameObject.Destroy(GameObject.Find("inkGiver"));
            }
            if (saveData.hasTimeAbility)
            {
                GameObject.Destroy(GameObject.Find("timeGiver"));
            }

            // disable already obtained collectables
            Transform[] collectables = GameObject.Find("collectables").GetComponentsInChildren<Transform>();
            if(collectables.Length != saveData.collectables.Count)
            {
                Debug.Log("Collectables: " + collectables.Length);
                Debug.Log("Save Data bools: " + saveData.collectables.Count);
            }
            else
            {
                // activate / deactivate collectables
                for(int i = 0; i < collectables.Length; i++)
                {
                    collectables[i].gameObject.SetActive(saveData.collectables[i]);
                }
            }


            return true;
        }

        return false;
    }
}
