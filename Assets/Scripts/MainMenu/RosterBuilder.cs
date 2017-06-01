using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using System.Linq;
using Players;

public static class RosterBuilder {

    private static Dictionary<string, string> AllShips = new Dictionary<string, string>();
    private static Dictionary<string, string> AllPilots = new Dictionary<string, string>();
    private static Dictionary<string, string> AllUpgrades = new Dictionary<string, string>();

    public static void Initialize()
    {
        SetPlayers();
        SetPlayerFactions();
        GenerateShipsList();
        AddInitialShips();
    }

    public static void PrepareForGameStart()
    {
        SetPlayers();
        GeneratePlayersShipConfigurations();
    }

    private static void AddInitialShips()
    {
        AddShip(PlayerNo.Player1);
        AddShip(PlayerNo.Player2);
    }

    public static void AddShip(PlayerNo playerNo)
    {
        GameObject prefab = GameObject.Find("ScriptHolder").GetComponent<MainMenuScript>().RosterBuilderPrefab;
        Transform parent = GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("ShipsPanel").Find("Player" + Tools.PlayerToInt(playerNo) + "Ships");

        GameObject newPanel = MonoBehaviour.Instantiate(prefab, parent);
        newPanel.transform.localPosition = Vector3.zero;
        newPanel.transform.Find("Panel").Find("RemoveButton").GetComponent<Button>().onClick.AddListener(delegate { RemoveShip(newPanel); });
        newPanel.transform.Find("Panel").Find("CopyButton").GetComponent<Button>().onClick.AddListener(delegate { CopyShip(newPanel); });

        SetShip(newPanel, playerNo);

        OrganizeShipsList(playerNo);
    }

    public static void CopyShip(GameObject panel)
    {
        Transform parent = panel.transform.parent;
        GameObject newPanel = MonoBehaviour.Instantiate(panel, parent);
        newPanel.transform.Find("Panel").Find("RemoveButton").GetComponent<Button>().onClick.AddListener(delegate { RemoveShip(newPanel); });
        newPanel.transform.Find("Panel").Find("CopyButton").GetComponent<Button>().onClick.AddListener(delegate { CopyShip(newPanel); });

        OrganizeAllShipsLists();
    }

    public static void RemoveShip(GameObject panel)
    {
        MonoBehaviour.DestroyImmediate(panel);
        OrganizeAllShipsLists();
    }

    private static void OrganizeAllShipsLists()
    {
        OrganizeShipsList(PlayerNo.Player1);
        OrganizeShipsList(PlayerNo.Player2);
    }

    private static void OrganizeShipsList(PlayerNo playerNo)
    {
        float offset = 0;
        Transform addShipPanel = GameObject.Find("Global").transform;
        foreach (Transform shipPanel in GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("ShipsPanel").Find("Player" + Tools.PlayerToInt(playerNo) + "Ships"))
        {
            if (shipPanel.Find("AddShipButton") != null)
            {
                addShipPanel = shipPanel;
            }
            else
            {
                shipPanel.localPosition = new Vector3(shipPanel.localPosition.x, offset, shipPanel.localPosition.z);
                offset = offset - shipPanel.GetComponent<RectTransform>().sizeDelta.y - 30 - 5;
            }
        }

        addShipPanel.localPosition = new Vector3(addShipPanel.localPosition.x, offset, addShipPanel.localPosition.z);
    }

    private static void GenerateShipsList()
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

    private static void SetShip(GameObject panel, PlayerNo playerNo)
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

        Dropdown shipDropdown = panel.transform.Find("GroupShip").Find("Dropdown").GetComponent<Dropdown>();
        shipDropdown.ClearOptions();
        shipDropdown.AddOptions(results);

        SetPilot(panel, playerNo);
    }

    private static void SetPilot(GameObject panel, PlayerNo playerNo)
    {
        string shipNameFull = panel.transform.Find("GroupShip").Find("Dropdown").GetComponent<Dropdown>().captionText.text;
        string shipNameId = AllShips[shipNameFull];
        List<string> results = GetPilotsList(shipNameId);

        Dropdown pilotDropdown = panel.transform.Find("GroupPilot").Find("Dropdown").GetComponent<Dropdown>();
        pilotDropdown.ClearOptions();
        pilotDropdown.AddOptions(results);

        SetAvailableUpgrades(panel, pilotDropdown.captionText.text);
    }

    private static void SetAvailableUpgrades(GameObject panel, string pilotName)
    {
        string pilotId = AllPilots[pilotName];
        Ship.GenericShip ship = (Ship.GenericShip)System.Activator.CreateInstance(System.Type.GetType(pilotId));
        foreach (var upgrade in ship.BuiltInSlots)
        {
            AddGroup(panel, upgrade.Key.ToString());
        }
    }

    private static void AddGroup(GameObject panel, string upgradeId)
    {
        GameObject prefab = GameObject.Find("ScriptHolder").GetComponent<MainMenuScript>().UpgradeGroupPrefab;
        Transform parent = panel.transform;

        GameObject newPanel = MonoBehaviour.Instantiate(prefab, parent);
        newPanel.transform.localPosition = Vector3.zero;
        newPanel.name = "Upgrade" + upgradeId + "Panel";
        newPanel.transform.Find("Text").GetComponent<Text>().text = upgradeId;

        Type type = typeof(Upgrade.UpgradeSlot);
        List<string> upgradeList = GetUpgrades((Upgrade.UpgradeSlot)Enum.Parse(type, upgradeId));
        newPanel.transform.Find("Dropdown").GetComponent<Dropdown>().AddOptions(upgradeList);

        OrganizePanelGroups(panel);
    }

    private static List<string> GetUpgrades(Upgrade.UpgradeSlot slot)
    {
        List<string> results = new List<string>();

        List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, "UpgradesList", StringComparison.Ordinal))
            .ToList();

        foreach (var type in typelist)
        {
            Upgrade.GenericUpgrade newUpgrade = (Upgrade.GenericUpgrade)System.Activator.CreateInstance(type);
            if (newUpgrade.Type == slot)
            {
                if (!AllUpgrades.ContainsKey(newUpgrade.Name)) AllUpgrades.Add(newUpgrade.Name, type.ToString());
                results.Add(newUpgrade.Name);
            }
        }

        return results;
    }

    private static void OrganizePanelGroups(GameObject panel)
    {
        float offset = 0;
        foreach (Transform group in panel.transform)
        {
            if (group.name != "Panel")
            {
                group.localPosition = new Vector3(group.localPosition.x, offset, group.localPosition.z);
                offset = offset - 30;
            }
        }
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(panel.GetComponent<RectTransform>().sizeDelta.x, -offset + 10);
    }

    private static List<string> GetPilotsList(string shipName)
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
                if (!AllPilots.ContainsKey(newShipContainer.PilotName))
                {
                    AllPilots.Add(newShipContainer.PilotName, type.ToString());
                }
                result.Add(newShipContainer.PilotName);
            }
        }

        return result;
    }

    private static Faction GetPlayerFaction(PlayerNo playerNo)
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

    private static void SetPlayers()
    {
        Global.RemoveAllPlayers();
        Global.AddPlayer(GetPlayerType(PlayerNo.Player1));
        Global.AddPlayer(GetPlayerType(PlayerNo.Player2));
    }

    private static void SetPlayerFactions()
    {
        Global.AddFaction(GetPlayerFaction(PlayerNo.Player1));
        Global.AddFaction(GetPlayerFaction(PlayerNo.Player2));
    }

    private static System.Type GetPlayerType(PlayerNo playerNo)
    {
        int index = GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("PlayersPanel").Find("Player"+ Tools.PlayerToInt(playerNo) + "Panel").Find("GroupPlayer").Find("Dropdown").GetComponent<Dropdown>().value;
        switch (index)
        {
            case 0: return typeof(Players.HumanPlayer);
            case 1: return typeof(Players.HotacAiPlayer);
        }
        return null;
    }

    private static void GeneratePlayersShipConfigurations()
    {
        GeneratePlayerShipConfigurations(PlayerNo.Player1);
        GeneratePlayerShipConfigurations(PlayerNo.Player2);
    }

    private static void GeneratePlayerShipConfigurations(PlayerNo playerNo)
    {
        foreach (Transform shipPanel in GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("ShipsPanel").Find("Player" + Tools.PlayerToInt(playerNo) + "Ships"))
        {
            if (shipPanel.name == "AddShipPanel") continue;
            string pilotNameFull = shipPanel.Find("GroupPilot").Find("Dropdown").GetComponent<Dropdown>().captionText.text;
            string pilotNameId = AllPilots[pilotNameFull];

            List<string> upgradesList = new List<string>();

            foreach (Transform upgradePanel in shipPanel.transform)
            {
                if (upgradePanel.name.StartsWith("Upgrade"))
                {
                    string upgradeName = upgradePanel.Find("Dropdown").GetComponent<Dropdown>().captionText.text;
                    if (AllUpgrades.ContainsKey(upgradeName))
                    {
                        upgradesList.Add(AllUpgrades[upgradeName]);
                    }
                }
            }

            Global.AddShip(pilotNameId, upgradesList, playerNo);
        }
    }
}
