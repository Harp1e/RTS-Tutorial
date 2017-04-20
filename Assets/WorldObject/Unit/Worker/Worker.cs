using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using RTS;

public class Worker : Unit
{
    public int buildSpeed;

    Building currentProject;
    bool building = false;
    float amountBuilt = 0f;
    int loadedProjectId = -1;

    protected override void Start ()
    {
        base.Start ();
        actions = new string[] { "Refinery", "WarFactory" };
        if (player && loadedSavedValues && loadedProjectId >= 0)
        {
            WorldObject obj = player.GetObjectForId (loadedProjectId);
            if (obj.GetType ().IsSubclassOf (typeof (Building))) currentProject = (Building)obj;
        }
    }

    protected override void Update ()
    {
        base.Update ();
        if (!moving && !rotating)
        {
            if (building && currentProject && currentProject.UnderConstruction ())
            {
                amountBuilt += buildSpeed * Time.deltaTime;
                int amount = Mathf.FloorToInt (amountBuilt);
                if (amount > 0)
                {
                    amountBuilt -= amount;
                    currentProject.Construct (amount);
                    if (!currentProject.UnderConstruction ()) building = false;
                }
            }
        }
    }

    public override void MouseClick (GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        bool doBase = true;
        if (player && player.human && currentlySelected && hitObject && !WorkManager.ObjectIsGround (hitObject))
        {
            Building building = hitObject.transform.parent.GetComponent<Building> ();
            if (building)
            {
                if (building.UnderConstruction())
                {
                    SetBuilding (building);
                    doBase = false;
                }
            }
        }
        if (doBase) base.MouseClick (hitObject, hitPoint, controller);
    }

    public override void SetBuilding (Building project)
    {
        base.SetBuilding (project);
        currentProject = project;
        StartMove (currentProject.transform.position, currentProject.gameObject);
        building = true;
    }

    public override void PerformAction (string actionToPerform)
    {
        base.PerformAction (actionToPerform);
        CreateBuilding (actionToPerform);
    }

    public override void StartMove (Vector3 destination)
    {
        base.StartMove (destination);
        amountBuilt = 0f;
        building = false;
    }

    private void CreateBuilding (string buildingName)
    {
        Vector3 buildPoint = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 10);
        if (player) player.CreateBuilding (buildingName, buildPoint, this, playingArea);
    }

    public override void SaveDetails (JsonWriter writer)
    {
        base.SaveDetails (writer);
        SaveManager.WriteBoolean (writer, "Building", building);
        SaveManager.WriteFloat (writer, "AmountBuilt", amountBuilt);
        if (currentProject) SaveManager.WriteInt (writer, "CurrentProjectId", currentProject.ObjectId);
    }

    protected override void HandleLoadedProperty (JsonTextReader reader, string propertyName, object readValue)
    {
        base.HandleLoadedProperty (reader, propertyName, readValue);
        switch (propertyName)
        {
            case "Building": building = (bool)readValue; break;
            case "AmountBuilt": amountBuilt = (float)(double)readValue; break;
            case "CurrentProjectId": loadedProjectId = (int)(System.Int64)readValue; break;
            default: break;
        }
    }
}
