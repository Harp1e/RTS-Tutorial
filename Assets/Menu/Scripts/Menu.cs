﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Menu : MonoBehaviour {

    public GUISkin mySkin;
    public Texture2D header;

    protected string[] buttons;


	protected virtual void Start () {
        SetButtons ();
	}

    protected virtual void OnGUI ()
    {
        DrawMenu ();
    }

    protected virtual void DrawMenu ()
    {
        GUI.skin = mySkin;
        float menuHeight = GetMenuHeight ();

        float groupLeft = Screen.width / 2 - ResourceManager.MenuWidth / 2;
        float groupTop = Screen.height / 2 - menuHeight / 2;
        GUI.BeginGroup (new Rect (groupLeft, groupTop, ResourceManager.MenuWidth, menuHeight));

        GUI.Box (new Rect (0, 0, ResourceManager.MenuWidth, menuHeight), "");
        GUI.DrawTexture (new Rect (ResourceManager.Padding, ResourceManager.Padding, ResourceManager.HeaderWidth, ResourceManager.HeaderHeight), header);

        float leftPos = ResourceManager.Padding;
        float topPos = 2 * ResourceManager.Padding + header.height;
        GUI.Label (new Rect (leftPos, topPos, ResourceManager.MenuWidth - 2 * ResourceManager.Padding, ResourceManager.TextHeight), "Welcome " + PlayerManager.GetPlayerName ());

        if (buttons != null)
        {
            leftPos = ResourceManager.MenuWidth / 2 - ResourceManager.ButtonWidth / 2;
            topPos += ResourceManager.TextHeight + ResourceManager.Padding;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i > 0)
                {
                    topPos += ResourceManager.ButtonHeight + ResourceManager.Padding;
                }
                if (GUI.Button(new Rect(leftPos,topPos,ResourceManager.ButtonWidth,ResourceManager.ButtonHeight),buttons[i]))
                {
                    HandleButton (buttons[i]);
                }
            }
        }
        GUI.EndGroup ();
    }

    protected virtual void SetButtons ()
    {
        // child class sets this
    }

    protected virtual void HandleButton (string text)
    {
        // handled by child class
    }

    protected virtual float GetMenuHeight ()
    {
        float buttonHeight = 0;
        if (buttons != null) buttonHeight = buttons.Length * ResourceManager.ButtonHeight;
        float paddingHeight = 2 * ResourceManager.Padding;
        if (buttons != null) paddingHeight += buttons.Length * ResourceManager.Padding;
        float messageHeight = ResourceManager.TextHeight + ResourceManager.Padding;
        return ResourceManager.HeaderHeight + buttonHeight + paddingHeight + messageHeight;           
    }

    protected virtual void HideCurrentMenu ()
    {
        // child class needs to set this to hide itself
    }

    protected void ExitGame ()
    {
        Application.Quit();
    }

    protected void LoadGame ()
    {
        HideCurrentMenu ();
        LoadMenu loadMenu = GetComponent<LoadMenu> ();
        if (loadMenu)
        {
            loadMenu.enabled = true;
            loadMenu.Activate ();
        }
    }
}
