using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class WorldObject : MonoBehaviour {

    public string objectName;
    public Texture2D buildImage;
    public int cost, sellValue, hitPoints, maxHitPoints;

    protected Player player;
    protected Bounds selectionBounds;
    protected Rect playingArea = new Rect (0f, 0f, 0f, 0f);
    protected GUIStyle healthStyle = new GUIStyle ();

    protected string[] actions = { };
    protected float healthPercentage = 1f;
    protected bool currentlySelected = false;

    protected virtual void Awake ()
    {
        selectionBounds = ResourceManager.InvalidBounds;
        CalculateBounds ();
    }
	protected virtual void Start ()
    {
        player = transform.root.GetComponentInChildren<Player> ();
	}
	
	protected virtual void Update ()
    {
		
	}

    protected virtual void OnGUI ()
    {
        if (currentlySelected) DrawSelection ();
    }

    public virtual void SetSelection (bool selected, Rect playingArea)
    {
        currentlySelected = selected;
        if (selected) this.playingArea = playingArea;
    }

    public string[] GetActions ()
    {
        return actions;
    }

    public virtual void PerformAction (string actionToPerform)
    {
        // actions defined by children
    }

    public virtual void MouseClick (GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        if (currentlySelected && hitObject && hitObject.name != "Ground")
        {
            WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject> ();
            if (worldObject)
            {
                Resource resource = hitObject.transform.parent.GetComponent<Resource> ();
                if (resource && resource.isEmpty ()) return;
                ChangeSelection (worldObject, controller);
            }
        }
    }

    public virtual void SetHoverState (GameObject hoverObject)
    {
        if (player && player.human && currentlySelected)
        {
            if (hoverObject.name == "Ground") player.hud.SetCursorState (CursorState.Move);
        }
    }

    public void CalculateBounds ()
    {
        selectionBounds = new Bounds (transform.position, Vector3.zero);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            selectionBounds.Encapsulate (r.bounds);
        }
    }

    public bool IsOwnedBy (Player owner)
    {
        if (player && player.Equals(owner))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Bounds GetSelectionBounds ()
    {
        return selectionBounds;
    }

    void ChangeSelection (WorldObject worldObject, Player controller)
    {
        SetSelection (false, playingArea);
        if (controller.SelectedObject) controller.SelectedObject.SetSelection (false, playingArea);
        controller.SelectedObject = worldObject;
        worldObject.SetSelection (true, controller.hud.GetPlayingArea());
    }

    void DrawSelection ()
    {
        GUI.skin = ResourceManager.SelectBoxSkin;
        Rect selectBox = WorkManager.CalculateSelectionBox (selectionBounds, playingArea);
        GUI.BeginGroup (playingArea);
        DrawSelectionBox (selectBox);
        GUI.EndGroup();
    }

    protected virtual void DrawSelectionBox (Rect selectBox)
    {
        GUI.Box (selectBox, "");
        CalculateCurrentHealth ();
        GUI.Label (new Rect (selectBox.x, selectBox.y - 7, selectBox.width * healthPercentage, 5), "", healthStyle);
    }

    protected virtual void CalculateCurrentHealth ()
    {
        healthPercentage = (float)hitPoints / (float)maxHitPoints;
        if (healthPercentage > 0.65f) healthStyle.normal.background = ResourceManager.HealthyTexture;
        else if (healthPercentage > 0.35f) healthStyle.normal.background = ResourceManager.DamagedTexture;
        else healthStyle.normal.background = ResourceManager.CriticalTexture;
    }
}
