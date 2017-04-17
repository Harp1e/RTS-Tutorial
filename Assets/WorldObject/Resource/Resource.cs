﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Resource : WorldObject
{
    public float capacity;

    protected float amountLeft;
    protected ResourceType resourceType;

    protected override void Start ()
    {
        base.Start ();
        amountLeft = capacity;
        resourceType = ResourceType.Unknown;
    }

    protected override void CalculateCurrentHealth ()
    {
        healthPercentage = amountLeft / capacity;
        healthStyle.normal.background = ResourceManager.GetResourceHealthBar (resourceType);
    }

    public void Remove (float amount)
    {
        amountLeft -= amount;
        if (amountLeft < 0)
        {
            amountLeft = 0;
        }
    }

    public bool isEmpty ()
    {
        return amountLeft <= 0;
    }

    public ResourceType GetResourceType ()
    {
        return resourceType;
    }
}