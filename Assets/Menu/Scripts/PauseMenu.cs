using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RTS;

public class PauseMenu : Menu
{
    Player player;

    protected override void Start ()
    {
        base.Start ();
        player = transform.root.GetComponent<Player> ();
	}
	
	void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Escape)) Resume ();
	}

    protected override void SetButtons ()
    {
        buttons = new string[] { "Resume", "Save Game", "Load Game", "Exit Game" };
    }

    protected override void HandleButton (string text)
    {
        switch (text)
        {
            case "Resume":
                Resume ();
                break;
            case "Save Game":
                SaveGame ();
                break;
            case "Load Game":
                LoadGame ();
                break;
            case "Exit Game":
                ReturnToMainMenu ();
                break;
            default:
                break;
        }
    }

    protected override void HideCurrentMenu ()
    {
        GetComponent<PauseMenu> ().enabled = false;
    }

    void Resume ()
    {
        Time.timeScale = 1f;
        GetComponent<PauseMenu> ().enabled = false;
        if (player) player.GetComponent<UserInput> ().enabled = true;
        Cursor.visible = false;
        ResourceManager.MenuOpen = false;
    }

    void SaveGame ()
    {
        GetComponent<PauseMenu> ().enabled = false;
        SaveMenu saveMenu = GetComponent<SaveMenu> ();
        if (saveMenu)
        {
            saveMenu.enabled = true;
            saveMenu.Activate ();
        }
    }

    void ReturnToMainMenu ()
    {
        ResourceManager.LevelName = "";
        SceneManager.LoadScene ("MainMenu");
        Cursor.visible = true;
    }
}
