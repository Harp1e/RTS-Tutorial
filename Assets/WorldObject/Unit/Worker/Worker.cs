﻿using System.Collections;
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

    protected override void Start ()
    {
        base.Start ();
        actions = new string[] { "Refinery", "WarFactory" };
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
        if (player && player.human && currentlySelected && hitObject && hitObject.name != "Ground")
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
}
