using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Players;
using Upgrade;

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
        /*foreach (Transform shipPanel in GetShipsPanel(playerNo))
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
        }*/
    }

    private static void CopyShip(PlayerNo playerNo, GameObject panel)
    {
        /*if (GetShipsCount(playerNo) < 8)
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
        }*/
    }

    private static void RemoveShip(PlayerNo playerNo, GameObject panel)
    {
        SquadBuilderRoster.RemoveShip(playerNo, panel);

        MonoBehaviour.DestroyImmediate(panel);
        UpdateSquadCost(playerNo);
        OrganizeAllShipsLists();
    }

    //Events

    private static void SubscribeShipDropdown(SquadBuilderShip squadBuilderShip)
    {
        Dropdown shipDropdown = squadBuilderShip.Panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>();
        shipDropdown.onValueChanged.AddListener(delegate
        {
            OnShipChanged(squadBuilderShip);
        });
    }

    private static void SubscribePilotDropdown(SquadBuilderShip squadBuilderShip)
    {
        Dropdown pilotDropdown = squadBuilderShip.Panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>();
        pilotDropdown.onValueChanged.AddListener(delegate
        {
            OnPilotChanged(squadBuilderShip);
        });
    }

    private static void SubscribeUpgradeDropdowns(SquadBuilderShip squadBuilderShip, SquadBuilderUpgrade upgrade)
    {
        Dropdown upgradeDropdown = upgrade.Panel.transform.GetComponent<Dropdown>();
        upgradeDropdown.onValueChanged.AddListener(delegate
        {
            OnUpgradeChanged(squadBuilderShip, upgrade);
        });
    }

    private static void SubscribeControlButtons(PlayerNo playerNo, GameObject panel)
    {
        panel.transform.Find("Panel/RemoveButton").GetComponent<Button>().onClick.AddListener(delegate
        {
            RemoveShip(playerNo, panel);
        });

        /*panel.transform.Find("Panel/CopyButton").GetComponent<Button>().onClick.AddListener(delegate
        {
            CopyShip(playerNo, panel);
        });*/
    }

    private static void OnPilotChanged(SquadBuilderShip squadBuilderShip)
    {
        ChangePilot(squadBuilderShip);
    }

    private static void OnShipChanged(SquadBuilderShip squadBuilderShip)
    {
        ChangeShip(squadBuilderShip);
    }

    private static string GetNameOfChangedShip(SquadBuilderShip squadBuilderShip)
    {
        return AllShips[squadBuilderShip.Panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>().captionText.text];
    }

    private static string GetNameOfChangedPilot(SquadBuilderShip squadBuilderShip)
    {
        return AllPilots[squadBuilderShip.Panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>().captionText.text];
    }

    private static void OnUpgradeChanged(SquadBuilderShip squadBuilderShip, SquadBuilderUpgrade upgrade)
    {
        ChangeUpgrade(squadBuilderShip, upgrade);
    }

    private static string GetNameOfChangedUpgrade(SquadBuilderUpgrade upgrade)
    {
        string result = "";
        string upgradeName = upgrade.Panel.transform.GetComponent<Dropdown>().captionText.text;
        if (AllUpgrades.ContainsKey(upgradeName)) result = AllUpgrades[upgradeName];
        return result;
    }

    private static void UpgradeEffectApply(PlayerNo playerNo, GameObject panel)
    {
        /*Dropdown upgradeDropdown = panel.transform.GetComponent<Dropdown>();
        string upgradeName = upgradeDropdown.captionText.text;
        if (AllUpgrades.ContainsKey(upgradeName))
        {
            Upgrade.GenericUpgrade upgradeContainer = (Upgrade.GenericUpgrade)Activator.CreateInstance(System.Type.GetType(AllUpgrades[upgradeName]));

            string pilotKey = panel.transform.parent.parent.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>().captionText.text;
            Ship.GenericShip shipContainer = (Ship.GenericShip)Activator.CreateInstance(Type.GetType(AllPilots[pilotKey]));

            upgradeContainer.SquadBuilderEffectApply(shipContainer);

            UpdateUpgradePanels(playerNo, panel.transform.parent.parent.gameObject);
        }*/
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
            GenericUpgrade upgrade = (GenericUpgrade)Activator.CreateInstance(Type.GetType(AllUpgrades[upgradeKey]));
            return upgrade.ImageUrl;
        }
        return null;
    }

    //Set values to panels

    private static void SetShipsDropdown(SquadBuilderShip squadBuilderShip, List<string> shipNames)
    {
        Dropdown shipDropdown = squadBuilderShip.Panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>();
        shipDropdown.ClearOptions();
        shipDropdown.AddOptions(shipNames);
        SubscribeShipDropdown(squadBuilderShip);
    }

    private static void SetPilotsDropdown(SquadBuilderShip squadBuilderShip, List<string> pilotNames)
    {
        Dropdown pilotDropdown = squadBuilderShip.Panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>();
        pilotDropdown.ClearOptions();
        pilotDropdown.AddOptions(pilotNames);
        pilotDropdown.value = pilotNames.Count - 1;
        SubscribePilotDropdown(squadBuilderShip);

        AddPilotTooltip(squadBuilderShip.Panel.transform.Find("GroupShip/DropdownPilot").gameObject);
    }

    //TODO: Rework
    private static void UpdateUpgradePanels(SquadBuilderShip squadBuilderShip)
    {
        List<SquadBuilderUpgrade> upgrades = new List<SquadBuilderUpgrade>(squadBuilderShip.Upgrades);

        foreach (var upgrade in upgrades)
        {
            squadBuilderShip.Upgrades.Remove(upgrade);
            MonoBehaviour.DestroyImmediate(upgrade.Panel);
        }

        SetAvailableUpgrades(squadBuilderShip);
        OrganizeUpgradeLines(squadBuilderShip.Panel);

        OrganizeShipsList(squadBuilderShip.Player);
    }

    private static GameObject CreateUpgradePanel(SquadBuilderShip squadBuilderShip, UpgradeSlot upgradeSlot)
    {
        GameObject prefab = GameObject.Find("ScriptHolder").GetComponent<MainMenu>().UpgradeLinePrefab;
        Transform parent = squadBuilderShip.Panel.transform.Find("GroupUpgrades");

        GameObject newUpgradeLine = MonoBehaviour.Instantiate(prefab, parent);
        newUpgradeLine.transform.localPosition = Vector3.zero;
        newUpgradeLine.name = "Upgrade" + upgradeSlot.Type + "Line";

        return newUpgradeLine;
    }

    private static void SetUpgradesDropdown(SquadBuilderShip squadBuilderShip, SquadBuilderUpgrade upgrade, List<String> upgradeList)
    {
        Dropdown upgradesDropdown = upgrade.Panel.transform.GetComponent<Dropdown>();
        upgradesDropdown.ClearOptions();

        string emptySlot = "Empty Slot: " + upgrade.Slot.Type;
        List<string> emptySlotList = new List<string>() { emptySlot };
        upgradesDropdown.AddOptions(emptySlotList);

        upgradesDropdown.AddOptions(upgradeList);

        SubscribeUpgradeDropdowns(squadBuilderShip, upgrade);
        AddUpgradeTooltip(upgrade.Panel);

        OrganizeUpgradeLines(squadBuilderShip.Panel);
    }

    // Update Costs

    private static void UpdateShipCost(SquadBuilderShip squadBuilderShip)
    {
        int totalShipCost = 0;

        totalShipCost += squadBuilderShip.Ship.Cost;

        foreach (var upgrade in squadBuilderShip.Upgrades)
        {
            if (upgrade.InstalledUpgrade != null)
            {
                totalShipCost += upgrade.InstalledUpgrade.Cost;
            }
        }

        squadBuilderShip.Panel.transform.Find("Panel/CostPanel").GetComponentInChildren<Text>().text = totalShipCost.ToString();
        UpdateSquadCost(squadBuilderShip.Player);
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

    //TODO: Change
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
