using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Player : MonoBehaviour {

    public HUD hud;
    public WorldObject SelectedObject { get; set; }

    public string username;
    public bool human;
    public int startMoney, startMoneyLimit, startPower, startPowerLimit;

    Dictionary<ResourceType, int> resources, resourceLimits;

    private void Awake ()
    {
        resources = InitResourceList ();
        resourceLimits = InitResourceList ();
    }

    void Start () {
        hud = GetComponentInChildren<HUD> ();
        AddStartResourceLimits ();
        AddStartResources ();
	}
	
	void Update () {
        if (human)
        {
            hud.SetResourceValues (resources, resourceLimits);
        }
	}

    private Dictionary<ResourceType, int> InitResourceList ()
    {
        Dictionary<ResourceType, int> list = new Dictionary<ResourceType, int> ();
        list.Add (ResourceType.Money, 0);
        list.Add (ResourceType.Power, 0);
        return list;
    }

    void AddStartResourceLimits ()
    {
        IncrementResourceLimit (ResourceType.Money, startMoneyLimit);
        IncrementResourceLimit (ResourceType.Power, startPowerLimit);
    }

    void AddStartResources()
    {
        AddResource (ResourceType.Money, startMoney);
        AddResource (ResourceType.Power, startPower);
    }

    public void AddResource (ResourceType type, int amount)
    {
        resources[type] += amount;
    }

    public void IncrementResourceLimit (ResourceType type, int amount)
    {
        resourceLimits[type] += amount;
    }
    
    public void AddUnit (string unitName, Vector3 spawnPoint, Vector3 rallyPoint, Quaternion rotation)
    {
        Units units = GetComponentInChildren<Units> ();
        GameObject newUnit = (GameObject)Instantiate (ResourceManager.GetUnit (unitName), spawnPoint, rotation);
        newUnit.transform.parent = units.transform;
        Unit unitObject = newUnit.GetComponent<Unit> ();
        if (unitObject && spawnPoint != rallyPoint) unitObject.StartMove (rallyPoint);
    }
}
