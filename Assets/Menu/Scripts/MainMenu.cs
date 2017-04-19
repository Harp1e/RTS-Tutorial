using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RTS;

public class MainMenu : Menu
{
    protected void OnEnable ()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;

    }

    protected void OnDisable ()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    } 

    protected override void SetButtons ()
    {
        buttons = new string[] { "New Game", "Change Player", "Quit Game" };
    }

    protected override void HandleButton (string text)
    {
        switch (text)
        {
            case "New Game":
                NewGame ();
                break;
            case "Change Player":
                ChangePlayer ();
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
     
    private void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode)
    {
        Cursor.visible = true;
        if(PlayerManager.GetPlayerName() == "")
        {
            GetComponent<MainMenu> ().enabled = false;
            GetComponent<SelectPlayerMenu> ().enabled = true;
        }
        else
        {
            GetComponent<MainMenu> ().enabled = true;
            GetComponent<SelectPlayerMenu> ().enabled = false;
        }
    }

    void ChangePlayer ()
    {
        GetComponent<MainMenu> ().enabled = false;
        GetComponent<SelectPlayerMenu> ().enabled = true;
        SelectionList.LoadEntries (PlayerManager.GetPlayerNames ());
    }
}
