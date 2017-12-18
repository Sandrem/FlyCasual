using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Players;
using Upgrade;
using Ship;

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

    private static void CopyShip(PlayerNo playerNo, GameObject panel)
    {
        if (GetShipsCount(playerNo) < 8)
        {
            SquadBuilderShip originalShip = SquadBuilderRoster.GetShips().Find(n => n.Panel == panel);

            AddShip(playerNo);
            SquadBuilderShip copiedShip = SquadBuilderRoster.GetShipsByPlayer(playerNo).Last();

            CopyShipType(originalShip, copiedShip);
            CopySkinDropdown(originalShip, copiedShip);
            CopyShipPilot(originalShip, copiedShip);
            CopyShipUpgrades(originalShip, copiedShip);

        }
        else
        {
            Messages.ShowError("You cannot have more than 8 ships");
        }
    }

    private static void CopyShipType(SquadBuilderShip originalShip, SquadBuilderShip copiedShip)
    {
        Dropdown copiedShipDropdown = copiedShip.Panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>();
        string originalShipName = originalShip.Panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>().captionText.text;

        for (int i = 0; i < copiedShipDropdown.options.Count; i++)
        {
            if (copiedShipDropdown.options[i].text == originalShipName)
            {
                copiedShipDropdown.value = i;
                break;
            }
        }
    }

    private static void CopySkinDropdown(SquadBuilderShip originalShip, SquadBuilderShip copiedShip)
    {
        Dropdown copiedShipDropdown = copiedShip.Panel.transform.Find("GroupShip/DropdownSkin").GetComponent<Dropdown>();
        Dropdown originalShipDropdown = originalShip.Panel.transform.Find("GroupShip/DropdownSkin").GetComponent<Dropdown>();
        copiedShipDropdown.value = originalShipDropdown.value;
    }

    private static void CopyShipPilot(SquadBuilderShip originalShip, SquadBuilderShip copiedShip)
    {
        Dropdown copiedPilotDropdown = copiedShip.Panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>();
        string originalPilotName = originalShip.Panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>().captionText.text;

        for (int i = 0; i < copiedPilotDropdown.options.Count; i++)
        {
            if (copiedPilotDropdown.options[i].text == originalPilotName)
            {
                copiedPilotDropdown.value = i;
                break;
            }
        }
    }

    private static void CopyShipUpgrades(SquadBuilderShip originalShip, SquadBuilderShip copiedShip)
    {
        foreach (var originalUpgrade in originalShip.GetUpgrades())
        {
            SquadBuilderUpgrade copiedUpgrade = copiedShip.GetUpgrades().Find(n => n.Slot.Type == originalUpgrade.Slot.Type && n.Slot.Counter == originalUpgrade.Slot.Counter);
            Dropdown copiedUpgradeDropdown = copiedUpgrade.Panel.GetComponent<Dropdown>();
            string originalUpgradeName = originalUpgrade.Panel.GetComponent<Dropdown>().captionText.text;

            for (int i = 0; i < copiedUpgradeDropdown.options.Count; i++)
            {
                if (copiedUpgradeDropdown.options[i].text == originalUpgradeName)
                {
                    copiedUpgradeDropdown.value = i;
                    break;
                }
            }
        }
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

        panel.transform.Find("Panel/CopyButton").GetComponent<Button>().onClick.AddListener(delegate
        {
            CopyShip(playerNo, panel);
        });
    }

    private static void OnPilotChanged(SquadBuilderShip squadBuilderShip)
    {
        ChangePilot(squadBuilderShip);
    }

    private static void OnShipChanged(SquadBuilderShip squadBuilderShip)
    {
        ChangeShip(squadBuilderShip);
    }

    private static string GetNameOfShip(SquadBuilderShip squadBuilderShip)
    {
        return AllShips.Find(n => n.ShipName == squadBuilderShip.Panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>().captionText.text).ShipNamespace;
    }

    private static string GetNameOfPilot(SquadBuilderShip squadBuilderShip)
    {
        return AllPilots.Find(n => n.PilotNameWithCost == squadBuilderShip.Panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>().captionText.text).PilotTypeName;
    }

    private static void OnUpgradeChanged(SquadBuilderShip squadBuilderShip, SquadBuilderUpgrade upgrade)
    {
        ChangeUpgrade(squadBuilderShip, upgrade);
    }

    private static string GetNameOfUpgrade(SquadBuilderUpgrade upgrade)
    {
        string result = "";
        string upgradeNameWithAnyCost = upgrade.Panel.transform.GetComponent<Dropdown>().captionText.text;

        int indexOfCostStart = upgradeNameWithAnyCost.IndexOf('(');

        if (indexOfCostStart != -1)
        {
            string upgradeNameWithoutCost = upgradeNameWithAnyCost.Substring(0, indexOfCostStart);

            foreach (var upgradeHolder in AllUpgrades)
            {
                if (upgradeHolder.UpgradeNameWithCost.StartsWith(upgradeNameWithoutCost))
                {
                    result = upgradeHolder.UpgradeTypeName;
                    break;
                }
            }
        }

        //OLD
        //if (AllUpgrades.Find(n => n.UpgradeNameWithCost == upgradeNameWithAnyCost) != null) result = AllUpgrades.Find(n => n.UpgradeNameWithCost == upgradeNameWithAnyCost).UpgradeTypeName;

        return result;
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
        GenericShip ship = (GenericShip)Activator.CreateInstance(Type.GetType(AllPilots.Find(n => n.PilotNameWithCost == pilotKey).PilotTypeName));
        return ship.ImageUrl;
    }

    private static string GetUpgradeTooltipImage(GameObject panel)
    {
        string upgradeKey = panel.GetComponent<Dropdown>().captionText.text;

        if (AllUpgrades.Find(n => n.UpgradeNameWithCost == upgradeKey) == null) return null;

        if (AllUpgrades.Find(n => n.UpgradeNameWithCost == upgradeKey).UpgradeTypeName != null)
        {
            GenericUpgrade upgrade = (GenericUpgrade)Activator.CreateInstance(Type.GetType(AllUpgrades.Find(n => n.UpgradeNameWithCost == upgradeKey).UpgradeTypeName));
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

    private static void SetSkinsDropdown(SquadBuilderShip squadBuilderShip, List<string> skinNames)
    {
        Dropdown skinDropdown = squadBuilderShip.Panel.transform.Find("GroupShip/DropdownSkin").GetComponent<Dropdown>();
        skinDropdown.ClearOptions();
        skinDropdown.AddOptions(skinNames);

        for (int i = 0; i < skinDropdown.options.Count; i++)
        {
            if (skinDropdown.options[i].text == squadBuilderShip.Ship.SkinName)
            {
                skinDropdown.value = i;
                break;
            }
        }
    }

    private static void UpdateUpgradePanelsFull(SquadBuilderShip squadBuilderShip)
    {
        List<SquadBuilderUpgrade> upgrades = new List<SquadBuilderUpgrade>(squadBuilderShip.GetUpgrades());

        foreach (var upgrade in upgrades)
        {
            squadBuilderShip.RemoveUpgrade(upgrade);
        }

        SetAvailableUpgrades(squadBuilderShip);
        OrganizeUpgradeLines(squadBuilderShip.Panel);

        OrganizeShipsList(squadBuilderShip.Player);
    }

    private static void UpdateUpgradePanelsDiff(SquadBuilderShip squadBuilderShip)
    {
        List<SquadBuilderUpgrade> oldUpgrades = new List<SquadBuilderUpgrade>(squadBuilderShip.GetUpgrades());
        List<UpgradeSlot> newUpgrades = squadBuilderShip.Ship.UpgradeBar.GetUpgradeSlots();

        foreach (var oldUpgradeSlot in oldUpgrades)
        {
            UpgradeSlot newUpgrade = newUpgrades.Find(n => n.Type == oldUpgradeSlot.Slot.Type && n.Counter == oldUpgradeSlot.Slot.Counter);
            if (newUpgrade == null)
            {
                squadBuilderShip.RemoveUpgrade(oldUpgradeSlot);
            }
            else
            {
                squadBuilderShip.ChangeUpgradeSlot(oldUpgradeSlot, newUpgrade);
            }
        }

        foreach (var newUpgradeSlot in newUpgrades)
        {
            SquadBuilderUpgrade oldUpgrade = oldUpgrades.Find(n => n.Slot.Type == newUpgradeSlot.Type && n.Slot.Counter == newUpgradeSlot.Counter);
            if (oldUpgrade == null)
            {
                AddUpgrade(squadBuilderShip, newUpgradeSlot);
            }
        }

        OrganizeUpgradeLines(squadBuilderShip.Panel);
        UpdateShipCost(squadBuilderShip);
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
        if (upgrade.Panel != null)
        {
            Dropdown upgradesDropdown = upgrade.Panel.transform.GetComponent<Dropdown>();
            upgradesDropdown.ClearOptions();

            string emptySlot = "Empty Slot: " + upgrade.Slot.Type;
            List<string> emptySlotList = new List<string>() { emptySlot };
            upgradesDropdown.AddOptions(emptySlotList);

            upgradesDropdown.AddOptions(upgradeList);

            OrganizeUpgradeLines(squadBuilderShip.Panel);
        }
    }

    // Helper function for reducing costs.
    private static int ReduceUpgradeCost(int cost, int decrease)
    {
        if(cost >= 0)
        {
            cost = Math.Max(cost - decrease, 0);
        }

        return cost;
    }


    // Update Costs

    private static void UpdateShipCost(SquadBuilderShip squadBuilderShip)
    {
        int totalShipCost = 0;

        totalShipCost += squadBuilderShip.Ship.Cost;

        foreach (var upgrade in squadBuilderShip.GetUpgrades())
        {
            if (!upgrade.Slot.IsEmpty)
            {
                totalShipCost += ReduceUpgradeCost(upgrade.Slot.InstalledUpgrade.Cost, upgrade.Slot.CostDecrease);
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

    private static int GetPlayerShipsCostCalculated(PlayerNo playerNo)
    {
        return int.Parse(GetPlayerPanel(playerNo).Find("SquadCostPanel/CostCurrent").GetComponent<Text>().text);
    }

    private static int GetShipCostCalculated(SquadBuilderShip ship)
    {
        return int.Parse(ship.Panel.transform.Find("Panel/CostPanel").GetComponentInChildren<Text>().text);
    }

    //Get information from panels

    private static Faction GetPlayerFaction(PlayerNo playerNo)
    {
        Faction result = Faction.Imperial;
        int index = GetPlayerPanel(playerNo).Find("GroupFaction/Dropdown").GetComponent<Dropdown>().value;
        switch (index)
        {
            case 0:
                result = Faction.Rebel;
                break;
            case 1:
                result = Faction.Imperial;
                break;
            case 2:
                result = Faction.Scum;
                break;
        }
        SquadBuilderRoster.playerFactions[playerNo] = result;
        return result;
    }

    private static Type GetPlayerType(PlayerNo playerNo)
    {
        int index = GetPlayerPanel(playerNo).Find("GroupPlayer/Dropdown").GetComponent<Dropdown>().value;
        switch (index)
        {
            case 0: return typeof(HumanPlayer);
            case 1: return typeof(HotacAiPlayer);
            case 2: return typeof(NetworkOpponentPlayer);
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

        bool isFactionChanged = false;
        foreach (Transform shipPanel in playerShips)
        {
            if (shipPanel.name == "AddShipPanel") continue;
            string pilotName = shipPanel.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>().captionText.text;
            GenericShip newPilot = (GenericShip)Activator.CreateInstance(Type.GetType(AllPilots.Find(n => n.PilotNameWithCost == pilotName).PilotTypeName));
            if (newPilot.faction != playerFaction)
            {
                isFactionChanged = true;
                RemoveShip(playerNo, shipPanel.gameObject);
            }
        }

        if (isFactionChanged) AddInitialShip(playerNo);
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
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(580, -offset + 90);

        OrganizeAllShipsLists();
    }

    private static string GetSkinName(SquadBuilderShip ship)
    {
        return ship.Panel.transform.Find("GroupShip/DropdownSkin").GetComponent<Dropdown>().captionText.text;
    }

}
