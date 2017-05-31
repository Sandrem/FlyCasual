using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using System.Linq;

public class MainMenuScript : MonoBehaviour {

    public GameObject ButtonsHolder;
    public GameObject SquadBuilder;
    public GameObject BackgroundImage;

    // Use this for initialization
    void Start () {
        //TODO: Adjust size for small screen resolutions
        ButtonsHolder.transform.position = new Vector3(Screen.width / 20, Screen.height - Screen.height / 20, 0.0f);
        BackgroundImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height * 16f/9f, Screen.height);
        //BackgroundImage.transform.position = new Vector3(Screen.width - Screen.height, 0, 0);
        Global.test = "Changed";
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void NewGame()
    {
        OpenSquadronBuilder();
    }

    public void StartGame()
    {
        SetPlayers();
        SceneManager.LoadScene("Battle");
    }

    private void OpenSquadronBuilder()
    {
        GenerateShipsList();
        ButtonsHolder.SetActive(false);
        SquadBuilder.SetActive(true);
    }

    private void GenerateShipsList()
    {
        IEnumerable<string> namespaceIEnum =
            from types in Assembly.GetExecutingAssembly().GetTypes()
            where types.Namespace != null
            where types.Namespace.StartsWith("Ship.")
            select types.Namespace;

        List<string> namespaceList = new List<string>();
        foreach (var ns in namespaceIEnum)
        {
            if (!namespaceList.Contains(ns))
            {
                namespaceList.Add(ns);
                Debug.Log("Ship: " + ns);

                List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => String.Equals(t.Namespace, ns, StringComparison.Ordinal))
                    .ToList();

                foreach (var type in typelist)
                {
                    Debug.Log("--- Pilot: " + type.Name);
                    //Ship.GenericShip newShipContainer = (Ship.GenericShip)System.Activator.CreateInstance(type, Players.PlayerNo.Player1, -1, Vector3.zero);
                    //Debug.Log("--- PilotName: " + newShipContainer.PilotName);
                }
            }
        }

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

    private void SetPlayers()
    {
        Global.AddPlayer(GetPlayerType(1));
        Global.AddPlayer(GetPlayerType(2));
    }

    private System.Type GetPlayerType(int playerNo)
    {
        int index = GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("PlayersPanel").Find("Player"+ playerNo + "Panel").Find("Group").Find("Dropdown").GetComponent<Dropdown>().value;
        switch (index)
        {
            case 0: return typeof(Players.HumanPlayer);
            case 1: return typeof(Players.HotacAiPlayer);
        }
        return null;
    }
}
