using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using System.Linq;
using Players;

public class MainMenuScript : MonoBehaviour {

    public GameObject ButtonsHolder;
    public GameObject SquadBuilder;
    public GameObject BackgroundImage;

    public GameObject RosterBuilderPrefab;

    // Use this for initialization
    void Start () {
        //TODO: Adjust size for small screen resolutions
        ButtonsHolder.transform.position = new Vector3(Screen.width / 20, Screen.height - Screen.height / 20, 0.0f);
        BackgroundImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height * 16f/9f, Screen.height);
        //BackgroundImage.transform.position = new Vector3(Screen.width - Screen.height, 0, 0);
    }
	
    public void NewGame()
    {
        OpenSquadronBuilder();
    }

    public void StartGame()
    {
        RosterBuilder.PrepareForGameStart();
        SceneManager.LoadScene("Battle");
    }

    private void OpenSquadronBuilder()
    {
        ButtonsHolder.SetActive(false);
        SquadBuilder.SetActive(true);
        RosterBuilder.Initialize();
    }

    public void CloseSquadronBuilder()
    {
        SquadBuilder.SetActive(false);
        ButtonsHolder.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AddShipPlayer1()
    {
        RosterBuilder.AddShip(PlayerNo.Player1);
    }

    public void AddShipPlayer2()
    {
        RosterBuilder.AddShip(PlayerNo.Player2);
    }

}
