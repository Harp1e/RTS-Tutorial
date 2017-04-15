using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;
using System;

public class UserInput : MonoBehaviour {

    Player player;

	void Start () {
        player = GetComponent<Player> ();
	}
	

	void Update () {
        if (player.human)
        {
            MoveCamera ();
            RotateCamera ();
            MouseActivity ();
        }
	}

    private void MouseActivity ()
    {
        if (Input.GetMouseButtonDown (0)) LeftMouseClick ();
        else if (Input.GetMouseButtonDown (1)) RightMouseClick ();
        MouseHover ();
    }

    private void RightMouseClick ()
    {
        if (player.hud.MouseInBounds() && !Input.GetKey(KeyCode.LeftAlt) && player.SelectedObject)
        {
            player.SelectedObject.SetSelection (false, player.hud.GetPlayingArea());
            player.SelectedObject = null;
        }
    }

    private void LeftMouseClick ()
    {
        if (player.hud.MouseInBounds())
        {
            GameObject hitObject = FindHitObject ();
            Vector3 hitPoint = FindHitPoint ();
            if (hitObject && hitPoint != ResourceManager.InvalidPosition)
            {
                if (player.SelectedObject) player.SelectedObject.MouseClick (hitObject, hitPoint, player);
                else if (hitObject.name != "Ground")
                {
                    WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject> ();
                    if (worldObject)
                    {
                        player.SelectedObject = worldObject;
                        worldObject.SetSelection (true, player.hud.GetPlayingArea ());
                    }
                }
            }
        }
    }

    private void RotateCamera ()
    {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        // detect rotation if ALT + RMB
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(1))
        {
            destination.x -= Input.GetAxis ("Mouse Y") * ResourceManager.RotateAmount;
            destination.y -= Input.GetAxis ("Mouse X") * ResourceManager.RotateAmount;
        }

        if (destination != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards (origin, destination, Time.deltaTime * ResourceManager.RotateSpeed);
        }
    }

    private void MoveCamera ()
    {
        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;
        Vector3 movement = new Vector3 (0, 0, 0);
        bool mouseScroll = false;

        // Horizontal camera movement
        if (xPos >= 0 && xPos < ResourceManager.ScrollWidth)
        {
            movement.x -= ResourceManager.ScrollSpeed;
            player.hud.SetCursorState (CursorState.PanLeft);
            mouseScroll = true;
        }
        else if (xPos <= Screen.width && xPos > Screen.width -ResourceManager.ScrollWidth)
        {
            movement.x += ResourceManager.ScrollSpeed;
            player.hud.SetCursorState (CursorState.PanRight);
            mouseScroll = true;
        }

        // Vertical camera movement
        if (yPos >= 0 && yPos < ResourceManager.ScrollWidth)
        {
            movement.z -= ResourceManager.ScrollSpeed;
            player.hud.SetCursorState (CursorState.PanDown);
            mouseScroll = true;
        }
        else if (yPos <= Screen.width && yPos > Screen.height - ResourceManager.ScrollWidth)
        {
            movement.z += ResourceManager.ScrollSpeed;
            player.hud.SetCursorState (CursorState.PanUp);
            mouseScroll = true;
        }

        movement = Camera.main.transform.TransformDirection (movement);
        movement.y = 0;
        // add vertical movement
        movement.y -= ResourceManager.ScrollSpeed * Input.GetAxis ("Mouse ScrollWheel");

        // calculate desired camera pos.
        Vector3 origin = Camera.main.transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        // limit height above ground
        if (destination.y > ResourceManager.MaxCameraHeight)
        {
            destination.y = ResourceManager.MaxCameraHeight;
        }
        else if (destination.y < ResourceManager.MinCameraHeight)
        {
            destination.y = ResourceManager.MinCameraHeight;
        }

        // update camera pos if necessary
        if (destination != origin)
        {
            Camera.main.transform.position = Vector3.MoveTowards (origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
        }
        if (!mouseScroll)
        {
            player.hud.SetCursorState (CursorState.Select);
        }
    }

    GameObject FindHitObject ()
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) return hit.collider.gameObject;
        return null;
    }

    Vector3 FindHitPoint ()
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit)) return hit.point;
        return ResourceManager.InvalidPosition;
    }

    void MouseHover ()
    {
        if (player.hud.MouseInBounds())
        {
            GameObject hoverObject = FindHitObject ();
            if (hoverObject)
            {
                if (player.SelectedObject) player.SelectedObject.SetHoverState (hoverObject);
                else if (hoverObject.name != "Ground")
                {
                    Player owner = hoverObject.transform.root.GetComponent<Player> ();
                    if (owner)
                    {
                        Unit unit = hoverObject.transform.parent.GetComponent<Unit> ();
                        Building building = hoverObject.transform.parent.GetComponent<Building> ();
                        if (owner.username == player.username && (unit || building)) player.hud.SetCursorState (CursorState.Select);
                    }
                }
            }
        }
    }
}
