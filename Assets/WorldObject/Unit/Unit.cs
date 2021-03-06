﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;
using Newtonsoft.Json;

public class Unit : WorldObject
{
    public float moveSpeed, rotateSpeed;

    protected bool moving, rotating;

    Vector3 destination;
    Quaternion targetRotation;
    GameObject destinationTarget;

    int loadedDestinationTargetId = -1;

    protected override void Awake ()
    {
        base.Awake ();
    }
    protected override void Start ()
    {
        base.Start ();
        if (player && loadedSavedValues && loadedDestinationTargetId >= 0)
        {
            destinationTarget = player.GetObjectForId (loadedDestinationTargetId).gameObject;
        }
    }

    protected override void Update ()
    {
        base.Update ();
        if (rotating) TurnToTarget ();
        else if (moving) MakeMove ();
    }

    protected override void OnGUI ()
    {
        base.OnGUI ();
    }

    public override void MouseClick (GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        base.MouseClick (hitObject, hitPoint, controller);
        if (player && player.human && currentlySelected)
        {
            bool clickedOnEmptyResource = false;
            if (hitObject.transform.parent)
            {
                Resource resource = hitObject.transform.parent.GetComponent<Resource> ();
                if (resource && resource.isEmpty ()) clickedOnEmptyResource = true;
            }
            if ((WorkManager.ObjectIsGround (hitObject) || clickedOnEmptyResource) && hitPoint != ResourceManager.InvalidPosition)
            {
                float x = hitPoint.x;
                float y = hitPoint.y + player.SelectedObject.transform.position.y;
                float z = hitPoint.z;
                Vector3 destination = new Vector3 (x, y, z);
                StartMove (destination);
            }
        }
    }

    public override void SetHoverState (GameObject hoverObject)
    {
        base.SetHoverState (hoverObject);
        if (player && player.human && currentlySelected)
        {
            bool moveHover = false;
            if (WorkManager.ObjectIsGround (hoverObject))
            {
                moveHover = true;
            }
            else
            {
                Resource resource = hoverObject.transform.parent.GetComponent<Resource> ();
                if (resource && resource.isEmpty ()) moveHover = true;
            }
            if (moveHover) player.hud.SetCursorState (CursorState.Move);
        }
    }

    public virtual void SetBuilding (Building building)
    {
        // specific intialisation for a unit can be specified here
    }

    public virtual void StartMove (Vector3 destination)
    {
        this.destination = destination;
        destinationTarget = null;
        targetRotation = Quaternion.LookRotation (destination - transform.position);
        rotating = true;
        moving = false;
    }

    public void StartMove (Vector3 destination, GameObject destinationTarget)
    {
        StartMove (destination);
        this.destinationTarget = destinationTarget;
    }

    void TurnToTarget ()
    {
        transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, rotateSpeed);
        CalculateBounds ();
        Quaternion inverseTargetRotation = new Quaternion (-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
        if (transform.rotation == targetRotation || transform.rotation == inverseTargetRotation)
        {
            rotating = false;
            moving = true;
        }
        if (destinationTarget) CalculateTargetDestination ();
    }

    void MakeMove ()
    {
        transform.position = Vector3.MoveTowards (transform.position, destination, Time.deltaTime * moveSpeed);
        if (transform.position == destination)
        {
            moving = false;
            movingIntoPosition = false;
        }
        CalculateBounds ();
    }

    void CalculateTargetDestination ()
    {
        Vector3 originalExtents = selectionBounds.extents;
        Vector3 normalExtents = originalExtents;
        normalExtents.Normalize ();
        float numberOfExtents = originalExtents.x / normalExtents.x;
        int unitShift = Mathf.FloorToInt (numberOfExtents);

        WorldObject worldObject = destinationTarget.GetComponent<WorldObject> ();
        if (worldObject) originalExtents = worldObject.GetSelectionBounds ().extents;
        else originalExtents = new Vector3 (0f, 0f, 0f);
        normalExtents = originalExtents;
        normalExtents.Normalize ();
        numberOfExtents = originalExtents.x / normalExtents.x;
        int targetShift = Mathf.FloorToInt (numberOfExtents);

        int shiftAmount = targetShift + unitShift;

        Vector3 origin = transform.position;
        Vector3 direction = new Vector3 (destination.x - origin.x, 0f, destination.z - origin.z);
        direction.Normalize ();

        for (int i = 0; i < shiftAmount; i++)
        {
            destination -= direction;
        }
        destination.y = destinationTarget.transform.position.y;
        destinationTarget = null;
    }

    public override void SaveDetails (JsonWriter writer)
    {
        base.SaveDetails (writer);
        SaveManager.WriteBoolean (writer, "Moving", moving);
        SaveManager.WriteBoolean (writer, "Rotating", rotating);
        SaveManager.WriteVector (writer, "Destination", destination);
        SaveManager.WriteQuaternion (writer, "TargetRotation", targetRotation);
        if (destinationTarget)
        {
            WorldObject destinationObject = destinationTarget.GetComponent<WorldObject> ();
            if (destinationObject) SaveManager.WriteInt (writer, "DestinationTargetId", destinationObject.ObjectId);
        }
    }

    protected override void HandleLoadedProperty (JsonTextReader reader, string propertyName, object readValue)
    {
        base.HandleLoadedProperty (reader, propertyName, readValue);
        switch (propertyName)
        {
            case "Moving": moving = (bool)readValue; break;
            case "Rotating": rotating = (bool)readValue; break;
            case "Destination": destination = LoadManager.LoadVector (reader); break;
            case "TargetRotation": targetRotation = LoadManager.LoadQuaternion (reader); break;
            case "DestinationTargetId": loadedDestinationTargetId = (int)(System.Int64)readValue; break;
            default: break;
        }
    }
}
