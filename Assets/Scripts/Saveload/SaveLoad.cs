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
        public float xPosition, yPosition;
        public bool hasInkAbility;
        public bool hasTimeAbility;
        public List<bool> collectables;
        public List<bool> wisps;
        public bool hasExtraHealth;

        public SaveData()
        {
            xPosition = yPosition = 0;
            hasInkAbility = hasTimeAbility = false;
            collectables = new List<bool>();
            wisps = new List<bool>();
            hasExtraHealth = false;
        }
    }

    public static SaveData saveData = new SaveData();

    public static bool Save()
    {
        // restore the health of all enemies
        Enemy[] enemys = GameObject.FindObjectsOfType<Enemy>();

        foreach (Enemy currentEnemy in enemys)
        {
            currentEnemy.enabled = true;
            currentEnemy.gameObject.GetComponent<Renderer>().enabled = true;
            currentEnemy.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            if(currentEnemy.GetComponent<patrolmove>() != null)
            {
                currentEnemy.GetComponent<patrolmove>().enabled = true;
            }

            currentEnemy.health = currentEnemy.GetComponent<Enemy>().maxHealth;

            if(currentEnemy.shoot)
            {
                currentEnemy.CancelInvoke("checkPlayerDist");
                currentEnemy.InvokeRepeating("checkPlayerDist", currentEnemy.shootInterval, currentEnemy.shootInterval);
            }
        }

        // store position
        saveData.xPosition = Utils.resetPos.x;
        saveData.yPosition = Utils.resetPos.y;

        // store which abilities 
        saveData.hasInkAbility = GameObject.FindGameObjectWithTag("Player").GetComponent<Abilityactivator>().hasInkAbility;
        saveData.hasTimeAbility = GameObject.FindGameObjectWithTag("Player").GetComponent<Abilityactivator>().hasTimeAbility;

        // find which collectables are active
        saveData.collectables.Clear();

        if(GameObject.Find("collectables") != null)
        {
            foreach (Transform currentCollectable in GameObject.Find("collectables").GetComponentsInChildren<Transform>(true))
            {
                saveData.collectables.Add(currentCollectable.gameObject.activeSelf);
            }
        }

        // find which wisps are active
        saveData.wisps.Clear();
        if (GameObject.Find("WispManager") != null)
        {
            foreach (WispController currentWisp in GameObject.Find("WispManager").GetComponentsInChildren<WispController>(true))
            {
                saveData.wisps.Add(currentWisp.gameObject.activeSelf);
            }
        }

        // determine if the player has unlocked extra health
        saveData.hasExtraHealth = Utils.maxHealth > 3;

        // serialize data to save file
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/saveGame.fox");
        bf.Serialize(file, saveData);
        file.Close();

        // show fake autosave icon
        GameObject.FindObjectOfType<UIController>().showFakeSave();

        return true;
    }

    public static bool Load()
    {
        // deserialize data from save file
        if (File.Exists(Application.persistentDataPath + "/saveGame.fox"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/saveGame.fox", FileMode.Open);
            saveData = (SaveData)bf.Deserialize(file);
            file.Close();

            // place player
            Utils.resetPos = new Vector3(saveData.xPosition, saveData.yPosition, 0);
            Utils.ResetPlayer();

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
            if(GameObject.Find("collectables") != null)
            {
                Transform[] collectables;
                collectables = GameObject.Find("collectables").GetComponentsInChildren<Transform>(true);

                if(collectables.Length != saveData.collectables.Count)
                {
                    Debug.Log("loaded the state of " + saveData.collectables.Count.ToString() + " collectables but there are " + collectables.Length.ToString() + " collectables in the scene");
                }
                else
                {
                    for(int i = 0; i < collectables.Length; i++)
                    {
                        collectables[i].gameObject.SetActive(saveData.collectables[i]);

                        if(!saveData.collectables[i])
                        {
                            Utils.numberOfCollectables++;
                        }
                    }
                }

                Utils.updateCollectableText();
            }

            // disable already seen wisps
            if (GameObject.Find("WispManager") != null)
            {
                WispController[] wisps;
                wisps = GameObject.Find("WispManager").GetComponentsInChildren<WispController>(true);

                if (wisps.Length != saveData.wisps.Count)
                {
                    Debug.Log("loaded the state of " + saveData.wisps.Count.ToString() + " wisps but there are " + wisps.Length.ToString() + " wisps in the scene");
                }
                else
                {
                    for (int i = 0; i < wisps.Length; i++)
                    {
                        wisps[i].gameObject.SetActive(saveData.wisps[i]);
                    }
                }
            }

            // apply max health
            Utils.maxHealth = saveData.hasExtraHealth ? 6 : 3;

            // remove the extra health object
            if(saveData.hasExtraHealth)
            {
                if(GameObject.Find("extraHealth") != null)
                {
                    GameObject.Destroy(GameObject.Find("extraHealth"));
                }
            }

            return true;
        }

        return false;
    }

    // deletes the save file
    public static void deleteSave()
    {
        if(File.Exists(Application.persistentDataPath + "/saveGame.fox"))
        {
            File.Delete(Application.persistentDataPath + "/saveGame.fox");
        }
    }
}
