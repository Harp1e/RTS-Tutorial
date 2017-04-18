using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RTS;

public class MainMenu : Menu
{
    protected override void SetButtons ()
    {
        buttons = new string[] { "New Game", "Quit Game" };
    }

    protected override void HandleButton (string text)
    {
        switch (text)
        {
            case "New Game":
                NewGame ();
                break;
            case "Quit Game":
                ExitGame ();
                break;
            default:
                break;
        }
    }

    void NewGame ()
    {
        ResourceManager.MenuOpen = false;
        SceneManager.LoadScene ("Map");
        Time.timeScale = 1f;
    }
}
