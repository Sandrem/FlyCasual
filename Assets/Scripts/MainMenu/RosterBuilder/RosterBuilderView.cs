using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Players;

public static partial class RosterBuilder {

    //Ship Panels

    private static GameObject CreateShipPanel(PlayerNo playerNo)
    {
        GameObject prefab = GetShipPanelPefab();
        Transform parent = GetShipsPanel(playerNo);

        GameObject newPanel = MonoBehaviour.Instantiate(prefab, parent);
        newPanel.transform.localPosition = Vector3.zero;
        newPanel.transform.Find("Panel").Find("RemoveButton").GetComponent<Button>().onClick.AddListener(delegate { RemoveShip(newPanel); });
        newPanel.transform.Find("Panel").Find("CopyButton").GetComponent<Button>().onClick.AddListener(delegate { CopyShip(newPanel); });

        return newPanel;
    }

    private static void GeneratePlayerShipConfigurations(PlayerNo playerNo)
    {
        foreach (Transform shipPanel in GetShipsPanel(playerNo))
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

 

    //Set values to panels

    private static void SetShip(GameObject panel, PlayerNo playerNo)
    {
        List<string> results = new List<string>();
        foreach (var ships in AllShips)
        {
            Ship.GenericShip newShip = (Ship.GenericShip) Activator.CreateInstance(Type.GetType(ships.Value + "." + ships.Value.Substring(5)));
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
        pilotDropdown.onValueChanged.AddListener(delegate { UpdateUpgradePanels(panel); });

        SetAvailableUpgrades(panel, pilotDropdown.captionText.text);
    }

    private static void UpdateUpgradePanels(GameObject panel)
    {
        string pilotName = panel.transform.Find("GroupPilot").Find("Dropdown").GetComponent<Dropdown>().captionText.text;
        string pilotId = AllPilots[pilotName];
        Ship.GenericShip ship = (Ship.GenericShip)Activator.CreateInstance(Type.GetType(pilotId));

        foreach (var slot in ship.BuiltInSlots)
        {
            if (panel.transform.Find("Upgrade" + slot.Key.ToString() + "Panel") == null)
            {
                AddGroup(panel, slot.Key.ToString());
            }
        }

        List<GameObject> toRemove = new List<GameObject>();
        foreach (Transform group in panel.transform)
        {
            if (group.name.StartsWith("Upgrade"))
            {
                Type type = typeof(Upgrade.UpgradeSlot);
                string upgradeId = group.Find("Text").GetComponent<Text>().text;
                Upgrade.UpgradeSlot slot = (Upgrade.UpgradeSlot)Enum.Parse(type, upgradeId);
                if (!ship.BuiltInSlots.ContainsKey(slot)) toRemove.Add(group.gameObject);
            }
        }
        foreach (var group in toRemove)
        {
            MonoBehaviour.DestroyImmediate(group);
            OrganizePanelGroups(panel);
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

    //Get information from panels

    private static Faction GetPlayerFaction(PlayerNo playerNo)
    {
        int index = GetPlayerPanel(playerNo).Find("GroupFaction").Find("Dropdown").GetComponent<Dropdown>().value;
        switch (index)
        {
            case 0:
                return Faction.Rebels;
            case 1:
                return Faction.Empire;
        }
        return Faction.Empire;
    }

    private static Type GetPlayerType(PlayerNo playerNo)
    {
        int index = GetPlayerPanel(playerNo).Find("GroupPlayer").Find("Dropdown").GetComponent<Dropdown>().value;
        switch (index)
        {
            case 0: return typeof(HumanPlayer);
            case 1: return typeof(HotacAiPlayer);
        }
        return null;
    }

    //Faction change

    private static void CheckPlayerFactonChange(PlayerNo playerNo)
    {
        Faction playerFaction = GetPlayerFaction(playerNo);

        List<Transform> playerShips = new List<Transform>();
        foreach (Transform shipPanel in GetShipsPanel(playerNo))
        {
            playerShips.Add(shipPanel);
        }

        foreach (Transform shipPanel in playerShips)
        {
            if (shipPanel.name == "AddShipPanel") continue;
            string pilotName = shipPanel.Find("GroupPilot").Find("Dropdown").GetComponent<Dropdown>().captionText.text;
            Ship.GenericShip newPilot = (Ship.GenericShip)Activator.CreateInstance(Type.GetType(AllPilots[pilotName]));
            if (newPilot.faction != playerFaction) RemoveShip(shipPanel.gameObject);
        }
    }

    //Get GameObjects

    private static GameObject GetShipPanelPefab()
    {
        return GameObject.Find("ScriptHolder").GetComponent<MainMenuScript>().RosterBuilderPrefab;
    }

    private static Transform GetPlayerPanel(PlayerNo playerNo)
    {
        return GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("PlayersPanel").Find("Player" + Tools.PlayerToInt(playerNo) + "Panel");
    }

    private static Transform GetShipsPanel(PlayerNo playerNo)
    {
        return GameObject.Find("Canvas").transform.Find("Panel").Find("SquadBuilderPanel").Find("ShipsPanel").Find("Player" + Tools.PlayerToInt(playerNo) + "Ships");
    }

    //Organization

    private static void OrganizeAllShipsLists()
    {
        OrganizeShipsList(PlayerNo.Player1);
        OrganizeShipsList(PlayerNo.Player2);
    }

    private static void OrganizeShipsList(PlayerNo playerNo)
    {
        float offset = 0;
        Transform addShipPanel = GameObject.Find("Global").transform;
        foreach (Transform shipPanel in GetShipsPanel(playerNo))
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

        OrganizeAllShipsLists();
    }

}
