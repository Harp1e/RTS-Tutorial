using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    static int nextObjectId = 0;
    static bool created = false;
    bool initialised = false;

    private void Awake ()
    {
        if (!created)
        {
            DontDestroyOnLoad (transform.gameObject);
            created = true;
            initialised = true;
        }
        else
        {
            Destroy (this.gameObject);
        }
    }

    private void OnEnable ()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    } 

    void OnLevelLoaded (Scene scene, LoadSceneMode mode)
    {
        if (initialised)
        {
            if (ResourceManager.LevelName != null && ResourceManager.LevelName != "")
            {
                LoadManager.LoadGame (ResourceManager.LevelName);
            }
            else
            {
                WorldObject[] worldObjects = GameObject.FindObjectsOfType (typeof (WorldObject)) as WorldObject[];
                foreach (WorldObject worldObject in worldObjects)
                {
                    worldObject.ObjectId = nextObjectId++;
                    if (nextObjectId >= int.MaxValue) nextObjectId = 0;
                }
            }
        }
    }

    public int GetNewObjectId ()
    {
        nextObjectId++;
        if (nextObjectId >= int.MaxValue) nextObjectId = 0;
        return nextObjectId;
    }
}
