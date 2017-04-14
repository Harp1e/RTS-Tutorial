﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

    //TODO Convert to Canvas from GUI coding

    public GUISkin resourceSkin, ordersSkin;

    const int ORDERS_BAR_WIDTH = 150, RESOURCE_BAR_HEIGHT = 40;

    Player player;

	void Start () {
        player = transform.root.GetComponent<Player> ();
	}

    void OnGUI ()
    {
        if (player && player.human)
        {
            DrawOrdersBar ();
            DrawResourceBar ();
        }
    }

    private void DrawResourceBar ()
    {
        GUI.skin = resourceSkin;
        GUI.BeginGroup (new Rect (0, 0, Screen.width, RESOURCE_BAR_HEIGHT));
        GUI.Box (new Rect (0, 0, Screen.width, RESOURCE_BAR_HEIGHT), "");
        GUI.EndGroup ();
    }

    private void DrawOrdersBar ()
    {
        GUI.skin = ordersSkin;
        GUI.BeginGroup (new Rect (Screen.width - ORDERS_BAR_WIDTH, RESOURCE_BAR_HEIGHT, ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT));
        GUI.Box (new Rect (0, 0, ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT), "");
        GUI.EndGroup ();
    }
}