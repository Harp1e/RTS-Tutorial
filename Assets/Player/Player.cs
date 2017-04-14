using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public HUD hud;
    public WorldObject SelectedObject { get; set; }
    public string username;
    public bool human;

	void Start () {
        hud = GetComponentInChildren<HUD> ();
	}
	
	void Update () {
		
	}
}
