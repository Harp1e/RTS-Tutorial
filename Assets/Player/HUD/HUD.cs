﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class HUD : MonoBehaviour {

    //TODO Convert to Canvas from GUI coding?

    public GUISkin resourceSkin, ordersSkin, selectBoxSkin, mouseCursorSkin, playerDetailsSkin;
    public Texture2D activeCursor;
    public Texture2D selectCursor, leftCursor, rightCursor, upCursor, downCursor;
    public Texture2D buttonHover, buttonClick;
    public Texture2D buildFrame, buildMask;
    public Texture2D smallButtonHover, smallButtonClick;
    public Texture2D rallyPointCursor;
    public Texture2D healthy, damaged, critical; 
    public Texture2D[] moveCursors, attackCursors, harvestCursors;
    public Texture2D[] resources;
    public Texture2D[] resourceHealthBars;

    Dictionary<ResourceType, int> resourceValues, resourceLimits;
    Dictionary<ResourceType, Texture2D> resourceImages;

    const int ORDERS_BAR_WIDTH = 150, RESOURCE_BAR_HEIGHT = 40;
    const int SELECTION_NAME_HEIGHT = 15;
    const int ICON_WIDTH = 32, ICON_HEIGHT = 32, TEXT_WIDTH = 128, TEXT_HEIGHT = 32;
    const int BUILD_IMAGE_WIDTH = 64, BUILD_IMAGE_HEIGHT = 64;
    const int BUTTON_SPACING = 7;
    const int SCROLL_BAR_WIDTH = 22;
    const int BUILD_IMAGE_PADDING = 8;

    Player player;
    CursorState activeCursorState;
    CursorState previousCursorState;
    WorldObject lastSelection;

    float sliderValue;
    int currentFrame = 0;
    int buildAreaHeight = 0;

	void Start () {
        player = transform.root.GetComponent<Player> ();
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
        Dictionary<ResourceType, Texture2D> resourceHealthBarTextures = new Dictionary<ResourceType, Texture2D> ();
        for (int i = 0; i < resourceHealthBars.Length; i++)
        {
            switch (resourceHealthBars[i].name)
            {
                case "ore":
                    resourceHealthBarTextures.Add (ResourceType.Ore, resourceHealthBars[i]);
                    break;
                default:
                    break;
            }
        }
        ResourceManager.SetResourceHealthBarTextures (resourceHealthBarTextures);
        buildAreaHeight = Screen.height - RESOURCE_BAR_HEIGHT - SELECTION_NAME_HEIGHT - 2 * BUTTON_SPACING;
        ResourceManager.StoreSelectBoxItems (selectBoxSkin, healthy, damaged, critical);
        SetCursorState (CursorState.Select);
    }

    void OnGUI ()
    {
        if (player.human)
        {
            DrawPlayerDetails ();
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
        if (activeCursorState != newState) previousCursorState = activeCursorState;
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
            case CursorState.RallyPoint:
                activeCursor = rallyPointCursor;
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

    public CursorState GetPreviousCursorState ()
    {
        return previousCursorState;
    }

    public CursorState GetCursorState ()
    {
        return activeCursorState;
    }

    void DrawPlayerDetails ()
    {
        GUI.skin = playerDetailsSkin;
        GUI.BeginGroup (new Rect (0, 0, Screen.width, Screen.height));
        float height = ResourceManager.TextHeight;
        float leftPos = ResourceManager.Padding;
        float topPos = Screen.height - height - ResourceManager.Padding;
        Texture2D avatar = PlayerManager.GetPlayerAvatar ();
        if (avatar)
        {
            GUI.DrawTexture (new Rect (leftPos, topPos, height, height), avatar);
            leftPos += height + ResourceManager.Padding;
        }
        float minWidth = 0, maxWidth = 0;
        string playerName = PlayerManager.GetPlayerName ();
        playerDetailsSkin.GetStyle ("label").CalcMinMaxWidth (new GUIContent (playerName), out minWidth, out maxWidth);
        GUI.Label (new Rect (leftPos, topPos, maxWidth, height), playerName);
        GUI.EndGroup ();
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

        int padding = 7;
        int buttonWidth = ORDERS_BAR_WIDTH - 2 * padding - SCROLL_BAR_WIDTH;
        int buttonHeight = RESOURCE_BAR_HEIGHT - 2 * padding;
        int leftPos = Screen.width - ORDERS_BAR_WIDTH / 2 - buttonWidth / 2 + SCROLL_BAR_WIDTH / 2;
        Rect menuButtonPosition = new Rect (leftPos, padding, buttonWidth, buttonHeight);

        if(GUI.Button(menuButtonPosition, "Menu"))
        {
            Time.timeScale = 0f;
            PauseMenu pauseMenu = GetComponent<PauseMenu> ();
            if (pauseMenu) pauseMenu.enabled = true;
            UserInput userInput = player.GetComponent<UserInput> ();
            if (userInput) userInput.enabled = false;
            Cursor.visible = true;
            ResourceManager.MenuOpen = true;
        }
        GUI.EndGroup ();
    }

    private void DrawOrdersBar ()
    {
        GUI.skin = ordersSkin;
        GUI.BeginGroup (new Rect (Screen.width - ORDERS_BAR_WIDTH - BUILD_IMAGE_WIDTH, RESOURCE_BAR_HEIGHT, ORDERS_BAR_WIDTH + BUILD_IMAGE_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT));
        GUI.Box (new Rect (BUILD_IMAGE_WIDTH + SCROLL_BAR_WIDTH, 0, ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT), "");

        string selectionName = "";
        if (player.SelectedObject)
        {
            selectionName = player.SelectedObject.objectName;
            if (player.SelectedObject.IsOwnedBy (player))
            {
                if (lastSelection && lastSelection != player.SelectedObject)
                {
                    sliderValue = 0f;
                }
                DrawActions (player.SelectedObject.GetActions ());
                lastSelection = player.SelectedObject;

                Building selectedBuilding = lastSelection.GetComponent<Building> ();
                if (selectedBuilding)
                {
                    DrawBuildQueue (selectedBuilding.getBuildQueueValues (), selectedBuilding.getBuildPercentage ());
                    DrawStandardBuildingOptions (selectedBuilding);
                }
            }
        }
        if (!selectionName.Equals (""))
        {
            int leftPos = BUILD_IMAGE_WIDTH + SCROLL_BAR_WIDTH / 2;
            int topPos = buildAreaHeight + BUTTON_SPACING;
            GUI.Label (new Rect (leftPos, topPos, ORDERS_BAR_WIDTH, SELECTION_NAME_HEIGHT), selectionName);
        }
        GUI.EndGroup ();
    }

    void DrawStandardBuildingOptions (Building building)
    {
        GUIStyle buttons = new GUIStyle ();
        buttons.hover.background = smallButtonHover;
        buttons.active.background = smallButtonClick;
        GUI.skin.button = buttons;
        int leftPos = BUILD_IMAGE_WIDTH + SCROLL_BAR_WIDTH + BUTTON_SPACING;
        int topPos = buildAreaHeight - BUILD_IMAGE_HEIGHT / 2;
        int width = BUILD_IMAGE_WIDTH / 2;
        int height = BUILD_IMAGE_HEIGHT / 2;
        if (GUI.Button (new Rect (leftPos, topPos, width, height), building.sellImage))
        {
            building.Sell ();
        }
        if (building.hasSpawnPoint())
        {
            leftPos += width + BUTTON_SPACING;
            if (GUI.Button (new Rect (leftPos, topPos, width, height), building.rallyPointImage))
            {
                if (activeCursorState != CursorState.RallyPoint && previousCursorState != CursorState.RallyPoint) SetCursorState (CursorState.RallyPoint);
                else
                {
                    SetCursorState (CursorState.PanRight);
                    SetCursorState (CursorState.Select);
                }
            }
        }
    }

    void DrawBuildQueue (string[] buildQueue, float buildPercentage)
    {
        for (int i = 0; i < buildQueue.Length; i++)
        {
            float topPos = i * BUILD_IMAGE_HEIGHT - (i + 1) * BUILD_IMAGE_PADDING;
            Rect buildPos = new Rect (BUILD_IMAGE_PADDING, topPos, BUILD_IMAGE_WIDTH, BUILD_IMAGE_HEIGHT);
            GUI.DrawTexture (buildPos, ResourceManager.GetBuildImage (buildQueue[i]));
            GUI.DrawTexture (buildPos, buildFrame);
            topPos += BUILD_IMAGE_PADDING;
            float width = BUILD_IMAGE_WIDTH - 2 * BUILD_IMAGE_PADDING;
            float height = BUILD_IMAGE_HEIGHT - 2 * BUILD_IMAGE_PADDING;
            if (i==0)
            {
                topPos += height * buildPercentage;
                height *= (1 - buildPercentage);
            }
            GUI.DrawTexture (new Rect (2 * BUILD_IMAGE_PADDING, topPos, width, height), buildMask);
        }
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
        if (mouseOverHud || ResourceManager.MenuOpen)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
            if (!player.IsFindingBuildingLocation ())
            {
                GUI.skin = mouseCursorSkin;
                GUI.BeginGroup (new Rect (0, 0, Screen.width, Screen.height));
                UpdateCursorAnimation ();
                Rect cursorPosition = GetCursorDrawPosition ();
                GUI.Label (cursorPosition, activeCursor);
                GUI.EndGroup ();
            }
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
        else if (activeCursorState == CursorState.RallyPoint) topPos -= activeCursor.height;
        return new Rect (leftPos, topPos, activeCursor.width, activeCursor.height);
    }

    void DrawActions (string[] actions)
    {
        GUIStyle buttons = new GUIStyle ();
        buttons.hover.background = buttonHover;
        buttons.active.background = buttonClick;
        GUI.skin.button = buttons;
        int numActions = actions.Length;

        GUI.BeginGroup (new Rect (BUILD_IMAGE_WIDTH, 0, ORDERS_BAR_WIDTH, buildAreaHeight));
        if (numActions >= MaxNumRows (buildAreaHeight)) DrawSlider (buildAreaHeight, numActions / 2f);
        for (int i = 0; i < numActions; i++)
        {
            int column = i % 2;
            int row = i / 2;
            Rect pos = GetButtonPos (row, column);
            Texture2D action = ResourceManager.GetBuildImage (actions[i]);
            if (action)
            {
                if (GUI.Button(pos, action))
                {
                    if (player.SelectedObject)
                    {
                        player.SelectedObject.PerformAction (actions[i]);
                    }
                }
            }
        }
        GUI.EndGroup ();
    }

    int MaxNumRows (int areaHeight)
    {
        return areaHeight / BUILD_IMAGE_HEIGHT;
    }

    Rect GetButtonPos (int row, int column)
    {
        int left = SCROLL_BAR_WIDTH + column * BUILD_IMAGE_WIDTH;
        float top = row * BUILD_IMAGE_HEIGHT - sliderValue * BUILD_IMAGE_HEIGHT;
        return new Rect (left, top, BUILD_IMAGE_WIDTH, BUILD_IMAGE_HEIGHT);
    }

    void DrawSlider(int groupHeight, float numRows)
    {
        sliderValue = GUI.VerticalSlider (GetScrollPos (groupHeight), sliderValue, 0f, numRows - MaxNumRows (groupHeight));
    }

    Rect GetScrollPos (int groupHeight)
    {
        return new Rect (BUTTON_SPACING, BUTTON_SPACING, SCROLL_BAR_WIDTH, groupHeight - 2 * BUTTON_SPACING);
    }
}
