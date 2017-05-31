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

    private Dictionary<string, string> AllShips = new Dictionary<string, string>();
    private Dictionary<string, string> AllPilots = new Dictionary<string, string>();

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
        GeneratePlayersShipConfigurations();
        SceneManager.LoadScene("Battle");
    }

    private void OpenSquadronBuilder()
    {
        ButtonsHolder.SetActive(false);
        SquadBuilder.SetActive(true);
        SetPlayers();
        SetPlayerFactions();
        GenerateShipsList();
        SetShips();
        SetPilots();
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
                Ship.GenericShip newShipTypeContainer = (Ship.GenericShip)System.Activator.CreateInstance(System.Type.GetType(ns + "." + ns.Substring(5)));
                AllShips.Add(newShipTypeContainer.Type, ns);
            }
        }

    }

    private void SetShips()
    {
        SetShip(PlayerNo.Player1);
        SetShip(PlayerNo.Player2);
    }

    private void SetShip(PlayerNo playerNo)
    {
        List<string> results = new List<string>();
        foreach (var ships in AllShips)
        {
            Ship.GenericShip newShip = (Ship.GenericShip) System.Activator.CreateInstance(System.Type.GetType(ships.Value + "." + ships.Value.Substring(5)));
            if (newShip.faction == Global.GetPlayerFaction(playerNo))
            {
                results.Add(ships.Key);
            }
        }

        foreach (Transform shipPanel in GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("ShipsPanel").Find("Player" + Tools.PlayerToInt(playerNo) + "Ships"))
        {
            Dropdown shipDropdown = shipPanel.Find("GroupShip").Find("Dropdown").GetComponent<Dropdown>();
            shipDropdown.ClearOptions();
            shipDropdown.AddOptions(results);
        }

    }

    private void SetPilots()
    {
        SetPilot(PlayerNo.Player1);
        SetPilot(PlayerNo.Player2);
    }

    private void SetPilot(PlayerNo playerNo)
    {
        string shipNameFull = GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("ShipsPanel").Find("Player" + Tools.PlayerToInt(playerNo) + "Ships").Find("ShipPanel1").Find("GroupShip").Find("Dropdown").GetComponent<Dropdown>().captionText.text;
        string shipNameId = AllShips[shipNameFull];

        List<string> results = GetPilotsList(shipNameId);

        foreach (Transform shipPanel in GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("ShipsPanel").Find("Player" + Tools.PlayerToInt(playerNo) + "Ships"))
        {
            Dropdown pilotDropdown = shipPanel.Find("GroupPilot").Find("Dropdown").GetComponent<Dropdown>();
            pilotDropdown.ClearOptions();
            pilotDropdown.AddOptions(results);
        }

    }

    private List<string> GetPilotsList(string shipName)
    {
        List<string> result = new List<string>();

        List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, shipName, StringComparison.Ordinal))
            .ToList();

        foreach (var type in typelist)
        {
            Ship.GenericShip newShipContainer = (Ship.GenericShip)System.Activator.CreateInstance(type);
            if (newShipContainer.PilotName != null)
            {
                AllPilots.Add(newShipContainer.PilotName, type.ToString());
                result.Add(newShipContainer.PilotName);
            }
        }

        return result;
    }

    private Faction GetPlayerFaction(PlayerNo playerNo)
    {
        int index = GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("PlayersPanel").Find("Player" + Tools.PlayerToInt(playerNo) + "Panel").Find("GroupFaction").Find("Dropdown").GetComponent<Dropdown>().value;
        switch (index)
        {
            case 0:
                return Faction.Rebels;
            case 1:
                return Faction.Empire;
        }
        return Faction.Empire;
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
        Global.RemoveAllPlayers();
        Global.AddPlayer(GetPlayerType(PlayerNo.Player1));
        Global.AddPlayer(GetPlayerType(PlayerNo.Player2));
    }

    private void SetPlayerFactions()
    {
        Global.AddFaction(GetPlayerFaction(PlayerNo.Player1));
        Global.AddFaction(GetPlayerFaction(PlayerNo.Player2));
    }

    private System.Type GetPlayerType(PlayerNo playerNo)
    {
        int index = GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("PlayersPanel").Find("Player"+ Tools.PlayerToInt(playerNo) + "Panel").Find("GroupPlayer").Find("Dropdown").GetComponent<Dropdown>().value;
        switch (index)
        {
            case 0: return typeof(Players.HumanPlayer);
            case 1: return typeof(Players.HotacAiPlayer);
        }
        return null;
    }

    private void GeneratePlayersShipConfigurations()
    {
        GeneratePlayerShipConfigurations(PlayerNo.Player1);
        GeneratePlayerShipConfigurations(PlayerNo.Player2);
    }

    private void GeneratePlayerShipConfigurations(PlayerNo playerNo)
    {
        foreach (Transform shipPanel in GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("ShipsPanel").Find("Player" + Tools.PlayerToInt(playerNo) + "Ships"))
        {
            string pilotNameFull = shipPanel.Find("GroupPilot").Find("Dropdown").GetComponent<Dropdown>().captionText.text;
            string pilotNameId = AllPilots[pilotNameFull];

            Global.AddShip(pilotNameId, new List<string>(), playerNo);
        }
    }
}
