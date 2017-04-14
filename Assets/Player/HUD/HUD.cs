using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

    //TODO Convert to Canvas from GUI coding

    public GUISkin resourceSkin, ordersSkin;

    const int ORDERS_BAR_WIDTH = 150, RESOURCE_BAR_HEIGHT = 40;
    const int SELECTION_NAME_HEIGHT = 15;

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

    public bool MouseInBounds()
    {
        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width - ORDERS_BAR_WIDTH;
        bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height - RESOURCE_BAR_HEIGHT;
        return insideWidth && insideHeight;
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
        string selectionName = "";
        if (player.SelectedObject)
        {
            selectionName = player.SelectedObject.objectName;
        }
        if (!selectionName.Equals (""))
        {
            GUI.Label (new Rect (0, 10, ORDERS_BAR_WIDTH, SELECTION_NAME_HEIGHT), selectionName);
        }
        GUI.EndGroup ();
    }
}
