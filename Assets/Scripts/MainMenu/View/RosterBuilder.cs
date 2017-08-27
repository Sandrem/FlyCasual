using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Players;

public static partial class RosterBuilder {

    //Ship Panels

    private static GameObject CreateShipPanel(PlayerNo playerNo)
    {
        GameObject prefab = GetShipPanelPefab();
        Transform parent = GetShipsPanel(playerNo);

        GameObject newPanel = MonoBehaviour.Instantiate(prefab, parent);
        newPanel.transform.localPosition = Vector3.zero;
        SubscribeControlButtons(playerNo, newPanel);

        return newPanel;
    }

    private static void GeneratePlayerShipConfigurations(PlayerNo playerNo)
    {
        foreach (Transform shipPanel in GetShipsPanel(playerNo))
        {
            if (shipPanel.name == "AddShipPanel") continue;
            string pilotNameFull = shipPanel.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>().captionText.text;
            string pilotNameId = AllPilots[pilotNameFull];

            List<string> upgradesList = new List<string>();

            foreach (Transform upgradePanel in shipPanel.transform.Find("GroupUpgrades"))
            {
                string upgradeName = upgradePanel.GetComponent<Dropdown>().captionText.text;
                if (AllUpgrades.ContainsKey(upgradeName))
                {
                    upgradesList.Add(AllUpgrades[upgradeName]);
                }
            }

            int shipCost = int.Parse(shipPanel.Find("Panel/CostPanel").GetComponentInChildren<Text>().text);

            Global.AddShip(pilotNameId, upgradesList, playerNo, shipCost);
        }
    }

    private static void CopyShip(PlayerNo playerNo, GameObject panel)
    {
        if (GetShipsCount(playerNo) < 8)
        {
            Transform parent = panel.transform.parent;
            GameObject newPanel = MonoBehaviour.Instantiate(panel, parent);

            SubscribeControlButtons(playerNo, newPanel);
            foreach (Transform upgradeLine in newPanel.transform.Find("GroupUpgrades"))
            {
                SubscribeUpgradeDropdowns(playerNo, upgradeLine.gameObject);
                AddUpgradeTooltip(upgradeLine.gameObject);
            }

            SubscribeShipDropdown(playerNo, newPanel);
            SubscribePilotDropdown(playerNo, newPanel);
            AddPilotTooltip(newPanel.transform.Find("GroupShip/DropdownPilot").gameObject);

            UpdateShipCost(playerNo, panel);
            OrganizeAllShipsLists();
        }
        else
        {
            Messages.ShowError("You cannot have more than 8 ships");
        }
    }

    private static void RemoveShip(PlayerNo playerNo, GameObject panel)
    {
        MonoBehaviour.DestroyImmediate(panel);
        UpdateSquadCost(playerNo);
        OrganizeAllShipsLists();
    }

    //Events

    private static void SubscribeShipDropdown(PlayerNo playerNo, GameObject panel)
    {
        Dropdown shipDropdown = panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>();
        shipDropdown.onValueChanged.AddListener(delegate
        {
            OnShipChanged(playerNo, panel);
        });
    }

    private static void SubscribePilotDropdown(PlayerNo playerNo, GameObject panel)
    {
        Dropdown pilotDropdown = panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>();
        pilotDropdown.onValueChanged.AddListener(delegate
        {
            OnPilotChanged(playerNo, panel);
        });
    }

    private static void SubscribeUpgradeDropdowns(PlayerNo playerNo, GameObject panel)
    {
        Dropdown upgradeDropdown = panel.transform.GetComponent<Dropdown>();
        upgradeDropdown.onValueChanged.AddListener(delegate
        {
            OnUpgradeChanged(playerNo, panel.transform.parent.parent.gameObject);
        });
    }

    private static void SubscribeControlButtons(PlayerNo playerNo, GameObject panel)
    {
        panel.transform.Find("Panel/RemoveButton").GetComponent<Button>().onClick.AddListener(delegate
        {
            RemoveShip(playerNo, panel);
        });

        panel.transform.Find("Panel/CopyButton").GetComponent<Button>().onClick.AddListener(delegate
        {
            CopyShip(playerNo, panel);
        });
    }

    private static void OnPilotChanged(PlayerNo playerNo, GameObject panel)
    {
        UpdateUpgradePanels(playerNo, panel);
        UpdateShipCost(playerNo, panel);
    }

    private static void OnShipChanged(PlayerNo playerNo, GameObject panel)
    {
        SetPilot(playerNo, panel);
        UpdateUpgradePanels(playerNo, panel);
        UpdateShipCost(playerNo, panel);
    }

    private static void OnUpgradeChanged(PlayerNo playerNo, GameObject panel)
    {
        UpdateShipCost(playerNo, panel);
    }

    //Tooltips
    private static void AddPilotTooltip(GameObject panel)
    {
        Tooltips.AddTooltip(panel, GetPilotTooltipImage);
    }

    private static void AddUpgradeTooltip(GameObject panel)
    {
        Tooltips.AddTooltip(panel, GetUpgradeTooltipImage);
    }

    private static string GetPilotTooltipImage(GameObject panel)
    {
        string pilotKey = panel.GetComponent<Dropdown>().captionText.text;
        Ship.GenericShip ship = (Ship.GenericShip)Activator.CreateInstance(Type.GetType(AllPilots[pilotKey]));
        return ship.ImageUrl;
    }

    private static string GetUpgradeTooltipImage(GameObject panel)
    {
        string upgradeKey = panel.GetComponent<Dropdown>().captionText.text;
        if (AllUpgrades.ContainsKey(upgradeKey))
        {
            Upgrade.GenericUpgrade upgrade = (Upgrade.GenericUpgrade)Activator.CreateInstance(Type.GetType(AllUpgrades[upgradeKey]));
            return upgrade.ImageUrl;
        }
        return null;
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

        Dropdown shipDropdown = panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>();
        shipDropdown.ClearOptions();
        shipDropdown.AddOptions(results);
        SubscribeShipDropdown(playerNo, panel);

        SetPilot(playerNo, panel);

        UpdateShipCost(playerNo, panel);
    }

    private static void SetPilot(PlayerNo playerNo, GameObject panel)
    {
        string shipNameFull = panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>().captionText.text;
        string shipNameId = AllShips[shipNameFull];
        List<string> results = GetPilotsList(shipNameId).OrderByDescending(n => PilotSkill[n]).ToList();

        Dropdown pilotDropdown = panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>();
        pilotDropdown.ClearOptions();
        pilotDropdown.AddOptions(results);
        pilotDropdown.value = results.Count - 1;
        SubscribePilotDropdown(playerNo, panel);

        SetAvailableUpgrades(playerNo, panel, pilotDropdown.captionText.text);
        AddPilotTooltip(panel.transform.Find("GroupShip/DropdownPilot").gameObject);

        OrganizeUpgradeLines(panel);
    }

    private static void UpdateUpgradePanels(PlayerNo playerNo, GameObject panel)
    {
        string pilotName = panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>().captionText.text;
        string pilotId = AllPilots[pilotName];
        Ship.GenericShip ship = (Ship.GenericShip)Activator.CreateInstance(Type.GetType(pilotId));

        foreach (var slot in ship.BuiltInSlots)
        {
            if (panel.transform.Find("GroupUpgrades/Upgrade" + slot.Key.ToString() + "Line") == null)
            {
                AddUpgradeLine(playerNo, panel, slot.Key.ToString());
            }
        }

        List<GameObject> toRemove = new List<GameObject>();
        Dictionary<Upgrade.UpgradeType, int> existingUpgradeSlots = new Dictionary<Upgrade.UpgradeType, int>();
        foreach (Transform group in panel.transform.Find("GroupUpgrades"))
        {
            Type type = typeof(Upgrade.UpgradeType);
            string upgradeId = group.GetComponent<Dropdown>().options[0].text;
            upgradeId = upgradeId.Substring(upgradeId.IndexOf(':') + 2);
            Upgrade.UpgradeType slot = (Upgrade.UpgradeType)Enum.Parse(type, upgradeId);

            if (existingUpgradeSlots.ContainsKey(slot))
            {
                existingUpgradeSlots[slot]++;
            }
            else
            {
                existingUpgradeSlots.Add(slot, 1);
            }

            if (!ship.BuiltInSlots.ContainsKey(slot))
            {
                toRemove.Add(group.gameObject);
            }
            else if (ship.BuiltInSlots[slot] < existingUpgradeSlots[slot])
            {
                toRemove.Add(group.gameObject);
            }
        }
        foreach (var group in toRemove)
        {
            MonoBehaviour.DestroyImmediate(group);
            OrganizeUpgradeLines(panel);
        }
    }

    private static void AddUpgradeLine(PlayerNo playerNo, GameObject panel, string upgradeId)
    {
        GameObject prefab = GameObject.Find("ScriptHolder").GetComponent<MainMenu>().UpgradeLinePrefab;
        Transform parent = panel.transform.Find("GroupUpgrades");

        GameObject newUpgradeLine = MonoBehaviour.Instantiate(prefab, parent);
        newUpgradeLine.transform.localPosition = Vector3.zero;
        newUpgradeLine.name = "Upgrade" + upgradeId + "Line";

        string emptySlot = "Empty Slot: " + upgradeId;
        List<string> emptySlotList = new List<string>() { emptySlot };
        newUpgradeLine.transform.GetComponent<Dropdown>().ClearOptions();
        newUpgradeLine.transform.GetComponent<Dropdown>().AddOptions(emptySlotList);

        Type type = typeof(Upgrade.UpgradeType);
        List<string> upgradeList = GetUpgrades((Upgrade.UpgradeType)Enum.Parse(type, upgradeId));
        newUpgradeLine.transform.GetComponent<Dropdown>().AddOptions(upgradeList);

        SubscribeUpgradeDropdowns(playerNo, newUpgradeLine);
        AddUpgradeTooltip(newUpgradeLine);

        OrganizeUpgradeLines(panel);
    }

    // Update Costs

    private static void UpdateShipCost(PlayerNo playerNo, GameObject panel)
    {
        int totalShipCost = 0;

        string pilotKey = panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>().captionText.text;
        Ship.GenericShip shipContainer = (Ship.GenericShip)Activator.CreateInstance(Type.GetType(AllPilots[pilotKey]));
        totalShipCost += shipContainer.Cost;

        foreach (Transform upgradePanel in panel.transform.Find("GroupUpgrades"))
        {
            string upgradeName = upgradePanel.transform.GetComponent<Dropdown>().captionText.text;
            
            if (AllUpgrades.ContainsKey(upgradeName))
            {
                Upgrade.GenericUpgrade upgradeContainer = (Upgrade.GenericUpgrade)Activator.CreateInstance(System.Type.GetType(AllUpgrades[upgradeName]));
                totalShipCost += upgradeContainer.Cost;
            }
        }

        panel.transform.Find("Panel/CostPanel").GetComponentInChildren<Text>().text = totalShipCost.ToString();
        UpdateSquadCost(playerNo);
    }

    private static void UpdateSquadCost(PlayerNo playerNo)
    {
        int squadCost = 0;

        foreach (Transform shipPanel in GetShipsPanel(playerNo))
        {
            if (shipPanel.name != "AddShipPanel")
            {
                squadCost += int.Parse(shipPanel.Find("Panel/CostPanel").GetComponentInChildren<Text>().text);
            }
        }

        GetPlayerPanel(playerNo).Find("SquadCostPanel/CostCurrent").GetComponent<Text>().text = squadCost.ToString();
        GetPlayerPanel(playerNo).Find("SquadCostPanel/CostCurrent").GetComponent<Text>().color = (squadCost > 100) ? Color.red : Color.white;
    }

    private static int GetShipCostCalculated(PlayerNo playerNo)
    {
        return int.Parse(GetPlayerPanel(playerNo).Find("SquadCostPanel/CostCurrent").GetComponent<Text>().text);
    }

    //Get information from panels

    private static Faction GetPlayerFaction(PlayerNo playerNo)
    {
        int index = GetPlayerPanel(playerNo).Find("GroupFaction/Dropdown").GetComponent<Dropdown>().value;
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
        int index = GetPlayerPanel(playerNo).Find("GroupPlayer/Dropdown").GetComponent<Dropdown>().value;
        switch (index)
        {
            case 0: return typeof(HumanPlayer);
            case 1: return typeof(HotacAiPlayer);
        }
        return null;
    }

    private static int GetShipsCount(PlayerNo playerNo)
    {
        int result = 0;

        foreach (var panel in GetShipsPanel(playerNo))
        {
            result++;
        }
        result--;

        return result;
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
            string pilotName = shipPanel.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>().captionText.text;
            Ship.GenericShip newPilot = (Ship.GenericShip)Activator.CreateInstance(Type.GetType(AllPilots[pilotName]));
            if (newPilot.faction != playerFaction) RemoveShip(playerNo, shipPanel.gameObject);
        }

        AddInitialShips();
    }

    //Get GameObjects

    private static GameObject GetShipPanelPefab()
    {
        return GameObject.Find("ScriptHolder").GetComponent<MainMenu>().RosterBuilderPrefab;
    }

    private static Transform GetPlayerPanel(PlayerNo playerNo)
    {
        return GameObject.Find("UI/Panels/RosterBuilderPanel/PlayersPanel/Player" + Tools.PlayerToInt(playerNo) + "Panel").transform;
    }

    private static Transform GetShipsPanel(PlayerNo playerNo)
    {
        return GameObject.Find("UI/Panels/RosterBuilderPanel/ShipsPanel/Player" + Tools.PlayerToInt(playerNo) + "Ships/Scroll View/Viewport/Content").transform;
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

        GetShipsPanel(playerNo).GetComponent<RectTransform>().sizeDelta = new Vector3(0, -offset + 40);
    }

    private static void OrganizeUpgradeLines(GameObject panel)
    {
        float offset = 0;
        int i = 0;
        foreach (Transform group in panel.transform.Find("GroupUpgrades"))
        {
            if (i%2 == 0)
            {
                group.localPosition = new Vector3(0, offset, group.localPosition.z);
            }
            else
            {
                group.localPosition = new Vector3(285, offset, group.localPosition.z);
                offset = offset - 30;
            }
            i++;
        }

        if (i % 2 == 1) offset = offset - 30;
        if (i == 0) offset = offset + 10;
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(580, -offset + 50);

        OrganizeAllShipsLists();
    }

}
