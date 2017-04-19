using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;
using Newtonsoft.Json;

public class Harvester : Unit
{
    public Building resourceStore;

    public float capacity;
    public float collectionAmount, depositAmount;

    ResourceType harvestType;
    Resource resourceDeposit;

    bool harvesting = false, emptying = false;
    float currentLoad = 0f;
    float currentDeposit = 0f;

    protected override void Start ()
    {
        base.Start ();
        harvestType = ResourceType.Unknown;
    }

    protected override void Update ()
    {
        base.Update ();
        if (!rotating && !moving)
        {
            if (harvesting || emptying)
            {
                Arms[] arms = GetComponentsInChildren<Arms> ();
                foreach (Arms arm in arms) arm.GetComponent<Renderer>().enabled = true;
                if (harvesting)
                {
                    Collect ();
                    if (currentLoad >= capacity || resourceDeposit.isEmpty())
                    {
                        currentLoad = Mathf.Floor (currentLoad);
                        harvesting = false;
                        emptying = true;
                        foreach (Arms arm in arms) arm.GetComponent<Renderer>().enabled = false;
                        StartMove (resourceStore.transform.position, resourceStore.gameObject);
                    }
                }
                else
                {
                    Deposit ();
                    if (currentLoad <= 0)
                    {
                        emptying = false;
                        foreach (Arms arm in arms) arm.GetComponent<Renderer>().enabled = false;
                        if (!resourceDeposit.isEmpty())
                        {
                            harvesting = true;
                            StartMove (resourceDeposit.transform.position, resourceDeposit.gameObject);
                        }
                    }
                }
            }
        }
    }

    public override void SetHoverState (GameObject hoverObject)
    {
        base.SetHoverState (hoverObject);
        if (player && player.human && currentlySelected)
        {
            if (hoverObject.name != "Ground")
            {
                Resource resource = hoverObject.transform.parent.GetComponent<Resource> ();
                if (resource && !resource.isEmpty ()) player.hud.SetCursorState (CursorState.Harvest);
            }
        }
    }

    public override void MouseClick (GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        base.MouseClick (hitObject, hitPoint, controller);
        if (player && player.human)
        {
            if (hitObject.name != "Ground")
            {
                Resource resource = hitObject.transform.parent.GetComponent<Resource> ();
                if (resource && !resource.isEmpty ())
                {
                    if (player.SelectedObject) player.SelectedObject.SetSelection (false, playingArea);
                    SetSelection (true, playingArea);
                    player.SelectedObject = this;
                    StartHarvest (resource);
                }
            }
            else StopHarvest ();
        }
    }

    protected override void DrawSelectionBox (Rect selectBox)
    {
        base.DrawSelectionBox (selectBox);
        float percentFull = currentLoad / capacity;
        float maxHeight = selectBox.height - 4;
        float height = maxHeight * percentFull;
        float leftPos = selectBox.x + selectBox.width - 7;
        float topPos = selectBox.y + 2 + (maxHeight - height);
        float width = 5f;
        Texture2D resourceBar = ResourceManager.GetResourceHealthBar (harvestType);
        if (resourceBar) GUI.DrawTexture (new Rect (leftPos, topPos, width, height), resourceBar);
    }

    public override void SetBuilding (Building store)
    {
        base.SetBuilding (store);
        resourceStore = store;
    }

    void StartHarvest (Resource resource)
    {
        resourceDeposit = resource;
        StartMove (resource.transform.position, resource.gameObject);
        if (harvestType == ResourceType.Unknown || harvestType != resource.GetResourceType())
        {
            harvestType = resource.GetResourceType ();
            currentLoad = 0f;
        }
        harvesting = true;
        emptying = false;
    }

    void StopHarvest ()
    {

    }

    void Collect ()
    {
        float collect = collectionAmount * Time.deltaTime;
        if (currentLoad + collect > capacity) collect = capacity - currentLoad;
        resourceDeposit.Remove (collect);
        currentLoad += collect;
    }

    void Deposit ()
    {
        currentDeposit += depositAmount * Time.deltaTime;
        int deposit = Mathf.FloorToInt (currentDeposit);
        if (deposit >= 1)
        {
            if (deposit > currentLoad) deposit = Mathf.FloorToInt (currentLoad);
            currentDeposit -= deposit;
            currentLoad -= deposit;
            ResourceType depositType = harvestType;
            if (harvestType == ResourceType.Ore) depositType = ResourceType.Money;
            player.AddResource (depositType, deposit);
        }
    }

    public override void SaveDetails (JsonWriter writer)
    {
        base.SaveDetails (writer);
        SaveManager.WriteBoolean (writer, "Harvesting", harvesting);
        SaveManager.WriteBoolean (writer, "Emptying", emptying);
        SaveManager.WriteFloat (writer, "CurrentLoad", currentLoad);
        SaveManager.WriteFloat (writer, "CurrentDeposit", currentDeposit);
        SaveManager.WriteString (writer, "HarvestType", harvestType.ToString());
        if (resourceDeposit) SaveManager.WriteInt (writer, "ResourceDepositId", resourceDeposit.ObjectId);
        if (resourceStore) SaveManager.WriteInt (writer, "ResourceStoreId", resourceStore.ObjectId);
    }
}
