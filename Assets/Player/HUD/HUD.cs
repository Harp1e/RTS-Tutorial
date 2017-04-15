using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class HUD : MonoBehaviour {

    //TODO Convert to Canvas from GUI coding?

    public GUISkin resourceSkin, ordersSkin, selectBoxSkin, mouseCursorSkin;
    public Texture2D activeCursor;
    public Texture2D selectCursor, leftCursor, rightCursor, upCursor, downCursor;
    public Texture2D[] moveCursors, attackCursors, harvestCursors;
    public Texture2D[] resources;

    Dictionary<ResourceType, int> resourceValues, resourceLimits;
    Dictionary<ResourceType, Texture2D> resourceImages;

    const int ORDERS_BAR_WIDTH = 150, RESOURCE_BAR_HEIGHT = 40;
    const int SELECTION_NAME_HEIGHT = 15;
    const int ICON_WIDTH = 32, ICON_HEIGHT = 32, TEXT_WIDTH = 128, TEXT_HEIGHT = 32;

    Player player;
    CursorState activeCursorState;

    int currentFrame = 0;

	void Start () {
        player = transform.root.GetComponent<Player> ();
        ResourceManager.StoreSelectBoxItems (selectBoxSkin);
        SetCursorState (CursorState.Select);
        resourceValues = new Dictionary<ResourceType, int> ();
        resourceLimits = new Dictionary<ResourceType, int> ();
        resourceImages = new Dictionary<ResourceType, Texture2D> ();
        for (int i = 0; i < resources.Length; i++)
        {
            switch (resources[i].name)
            {
                case "Money":
                    resourceImages.Add (ResourceType.Money, resources[i]);
                    resourceValues.Add (ResourceType.Money, 0);
                    resourceLimits.Add (ResourceType.Money, 0);
                    break;
                case "Power":
                    resourceImages.Add (ResourceType.Power, resources[i]);
                    resourceValues.Add (ResourceType.Power, 0);
                    resourceLimits.Add (ResourceType.Power, 0);
                    break;
                default:
                    break;
            }
        }
	}

    void OnGUI ()
    {
        if (player && player.human)
        {
            DrawOrdersBar ();
            DrawResourceBar ();
            DrawMouseCursor ();
        }
    }

    public bool MouseInBounds()
    {
        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width - ORDERS_BAR_WIDTH;
        bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height - RESOURCE_BAR_HEIGHT;
        return insideWidth && insideHeight;
    }

    public Rect GetPlayingArea()
    {
        return new Rect (0, RESOURCE_BAR_HEIGHT, Screen.width - ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT);
    }

    public void SetCursorState (CursorState newState)
    {
        activeCursorState = newState;
        switch (newState)
        {
            case CursorState.Select:
                activeCursor = selectCursor;
                break;
            case CursorState.Move:
                currentFrame = (int)Time.time % moveCursors.Length;
                activeCursor = moveCursors[currentFrame];
                break;
            case CursorState.Attack:
                currentFrame = (int)Time.time % attackCursors.Length;
                activeCursor = attackCursors[currentFrame];
                break;
            case CursorState.PanLeft:
                activeCursor = leftCursor;
                break;
            case CursorState.PanRight:
                activeCursor = rightCursor;
                break;
            case CursorState.PanUp:
                activeCursor = upCursor;
                break;
            case CursorState.PanDown:
                activeCursor = downCursor;
                break;
            case CursorState.Harvest:
                currentFrame = (int)Time.time % harvestCursors.Length;
                activeCursor = harvestCursors[currentFrame];
                break;
            default:
                break;
        }
    }

    public void SetResourceValues (Dictionary<ResourceType, int> resourceValues, Dictionary<ResourceType, int> resourceLimits)
    {
        this.resourceValues = resourceValues;
        this.resourceLimits = resourceLimits;
    }

    private void DrawResourceBar ()
    {
        GUI.skin = resourceSkin;
        GUI.BeginGroup (new Rect (0, 0, Screen.width, RESOURCE_BAR_HEIGHT));
        GUI.Box (new Rect (0, 0, Screen.width, RESOURCE_BAR_HEIGHT), "");
        int topPos = 4, iconLeft = 4, textLeft = 20;
        DrawResourceIcon (ResourceType.Money, iconLeft, textLeft, topPos);
        iconLeft += TEXT_WIDTH;
        textLeft += TEXT_WIDTH;
        DrawResourceIcon (ResourceType.Power, iconLeft, textLeft, topPos);
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

    void DrawResourceIcon (ResourceType type, int iconLeft, int textLeft, int topPos)
    {
        Texture2D icon = resourceImages[type];
        string text = resourceValues[type].ToString () + "/" + resourceLimits[type].ToString ();
        GUI.DrawTexture (new Rect (iconLeft, topPos, ICON_WIDTH, ICON_HEIGHT), icon);
        GUI.Label (new Rect (textLeft, topPos, TEXT_WIDTH, TEXT_HEIGHT), text);
    }

    void DrawMouseCursor ()
    {
        bool mouseOverHud = !MouseInBounds() && activeCursorState != CursorState.PanRight && activeCursorState != CursorState.PanUp;
        if (mouseOverHud)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
            GUI.skin = mouseCursorSkin;
            GUI.BeginGroup (new Rect (0, 0, Screen.width, Screen.height));
            UpdateCursorAnimation ();
            Rect cursorPosition = GetCursorDrawPosition ();
            GUI.Label (cursorPosition, activeCursor);
            GUI.EndGroup ();
        }
    }

    void UpdateCursorAnimation ()
    {
        if (activeCursorState == CursorState.Move )
        {
            currentFrame = (int)Time.time % moveCursors.Length;
            activeCursor = moveCursors [currentFrame];
        }
        else if (activeCursorState == CursorState.Attack)
        {
            currentFrame = (int)Time.time % attackCursors.Length;
            activeCursor = attackCursors [currentFrame];
        }
        else if (activeCursorState == CursorState.Harvest)
        {
            currentFrame = (int)Time.time % harvestCursors.Length;
            activeCursor = harvestCursors[currentFrame];
        }
    }

    Rect GetCursorDrawPosition ()
    {
        float leftPos = Input.mousePosition.x;
        float topPos = Screen.height - Input.mousePosition.y;

        if (activeCursorState == CursorState.PanRight) leftPos = Screen.width - activeCursor.width;
        else if (activeCursorState == CursorState.PanDown) topPos = Screen.height - activeCursor.height;
        else if (activeCursorState == CursorState.Move || activeCursorState == CursorState.Select || activeCursorState == CursorState.Harvest) 
        {
            topPos -= activeCursor.height / 2;
            leftPos -= activeCursor.width / 2;
        }
        return new Rect (leftPos, topPos, activeCursor.width, activeCursor.height);
    }
}
