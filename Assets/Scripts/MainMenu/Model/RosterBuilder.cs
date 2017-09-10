using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using System.Linq;
using Players;
using Ship;
using Upgrade;

public static partial class RosterBuilder {

    private class SquadBuilderUpgrade
    {
        public UpgradeSlot Slot;
        public GenericUpgrade InstalledUpgrade;
        public GameObject Panel;

        public SquadBuilderUpgrade(UpgradeSlot slot, GameObject panel)
        {
            Slot = slot;
            Panel = panel;
        }
    }

    private class SquadBuilderShip
    {
        public GenericShip Ship;
        public GameObject Panel;
        public PlayerNo Player;

        public List<SquadBuilderUpgrade> Upgrades = new List<SquadBuilderUpgrade>();

        public SquadBuilderShip(GenericShip ship, GameObject panel, PlayerNo playerNo)
        {
            Ship = ship;
            Panel = panel;
            Player = playerNo;

            InitializeShip();
        }

        private void InitializeShip()
        {
            Ship.InitializePilotForSquadBuilder();
        }
    }

    private static class SquadBuilderRoster
    {
        private static List<SquadBuilderShip> roster = new List<SquadBuilderShip>();

        public static void AddShip(SquadBuilderShip ship)
        {
            roster.Add(ship);
        }

        public static void RemoveShip(SquadBuilderShip ship)
        {
            if (roster.Contains(ship)) roster.Remove(ship);
        }

        public static void RemoveShip(PlayerNo playerNo, GameObject panel)
        {
            SquadBuilderShip ship = roster.Find(n => n.Panel == panel);
            if (ship != null) RemoveShip(ship);
        }
    }
    

    private static Dictionary<string, string> AllShips = new Dictionary<string, string>();

    private static Dictionary<string, string> AllPilots = new Dictionary<string, string>();
    private static Dictionary<string, int> PilotSkill = new Dictionary<string, int>();

    private static Dictionary<string, string> AllUpgrades = new Dictionary<string, string>();

    public static void Initialize()
    {
        InitializeSquadBuilderRoster();
        SetPlayers();
        SetPlayerFactions();
        GenerateShipsList();
        AddInitialShips();
    }

    //Initialization

    private static void InitializeSquadBuilderRoster()
    {

    }

    private static void SetPlayers()
    {
        Global.RemoveAllPlayers();
        Global.AddPlayer(GetPlayerType(PlayerNo.Player1));
        Global.AddPlayer(GetPlayerType(PlayerNo.Player2));
    }

    //TODO: Change to property of Player
    private static void SetPlayerFactions()
    {
        Global.RemoveAllFactions();
        Global.AddFaction(GetPlayerFaction(PlayerNo.Player1));
        Global.AddFaction(GetPlayerFaction(PlayerNo.Player2));
    }

    private static void GeneratePlayersShipConfigurations()
    {
        Global.RemoveAllShips();
        GeneratePlayerShipConfigurations(PlayerNo.Player1);
        GeneratePlayerShipConfigurations(PlayerNo.Player2);
    }

    private static void AddInitialShips()
    {
        if (GetShipsCount(PlayerNo.Player1) == 0) TryAddShip(PlayerNo.Player1);
        if (GetShipsCount(PlayerNo.Player2) == 0) TryAddShip(PlayerNo.Player2);
    }

    public static void TryAddShip(PlayerNo playerNo)
    {
        if (GetShipsCount(playerNo) < 8)
        {
            AddShip(playerNo);
        }
        else
        {
            Messages.ShowError("You cannot have more than 8 ships");
        }
    }

    private static void AddShip(PlayerNo playerNo)
    {
        List<string> shipResults = GetShipsByFaction(Global.GetPlayerFaction(playerNo));
        string shipNameId = AllShips[shipResults.First()];

        List<string> pilotResults = GetPilotsList(shipNameId).OrderByDescending(n => PilotSkill[n]).ToList();
        string pilotId = AllPilots[pilotResults.First()];
        GenericShip ship = (GenericShip)Activator.CreateInstance(Type.GetType(pilotId));

        GameObject panel = CreateShipPanel(playerNo);

        SquadBuilderShip squadBuilderShip = new SquadBuilderShip(ship, panel, playerNo);
        SquadBuilderRoster.AddShip(squadBuilderShip);

        SetShipsDropdown(squadBuilderShip, shipResults);
        SetPilotsDropdown(squadBuilderShip, pilotResults);
        SetAvailableUpgrades(squadBuilderShip);
        OrganizeUpgradeLines(panel);

        UpdateShipCost(squadBuilderShip);

        OrganizeShipsList(playerNo);
    }

    private static void ChangeShip(SquadBuilderShip squadBuilderShip)
    {
        squadBuilderShip.Ship = ChangeShipHolder(squadBuilderShip);

        UpdateUpgradePanels(squadBuilderShip);
        UpdateShipCost(squadBuilderShip);

        OrganizeShipsList(squadBuilderShip.Player);
    }

    private static GenericShip ChangeShipHolder(SquadBuilderShip squadBuilderShip)
    {
        GenericShip result = null;

        string shipName = GetNameOfChangedShip(squadBuilderShip);
        List<string> pilotResults = GetPilotsList(shipName).OrderByDescending(n => PilotSkill[n]).ToList();

        string pilotId = AllPilots[pilotResults.First()];
        GenericShip ship = (GenericShip)Activator.CreateInstance(Type.GetType(pilotId));

        SetPilotsDropdown(squadBuilderShip, pilotResults);
        //UpdateAvailableUpgrades(squadBuilderShip);
        //OrganizeUpgradeLines(panel);

        return result;
    }

    private static void ChangePilot(SquadBuilderShip squadBuilderShip)
    {
        squadBuilderShip.Ship = ChangePilotHolder(squadBuilderShip);

        //SetAvailableUpgrades(squadBuilderShip);
        //OrganizeUpgradeLines(squadBuilderShip.Panel);
        UpdateUpgradePanels(squadBuilderShip);
        UpdateShipCost(squadBuilderShip);

        OrganizeShipsList(squadBuilderShip.Player);
    }

    private static GenericShip ChangePilotHolder(SquadBuilderShip squadBuilderShip)
    {
        GenericShip result = null;

        string pilotId = GetNameOfChangedPilot(squadBuilderShip);
        GenericShip ship = (GenericShip)Activator.CreateInstance(Type.GetType(pilotId));

        return result;
    }

    private static List<string> GetShipsByFaction(Faction faction)
    {
        List <string> result = new List<string>();
        foreach (var ships in AllShips)
        {
            GenericShip newShip = (GenericShip)Activator.CreateInstance(Type.GetType(ships.Value + "." + ships.Value.Substring(5)));
            if (newShip.faction == faction)
            {
                result.Add(ships.Key);
            }
        }

        return result;
    }

    // Generate lists of everything

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
                if (!AllShips.ContainsKey(newShipTypeContainer.Type))
                {
                    AllShips.Add(newShipTypeContainer.Type, ns);
                }
            }
        }
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
            if ((newShipContainer.PilotName != null) && (!newShipContainer.IsHidden))
            {
                string pilotKey = newShipContainer.PilotName + " (" + newShipContainer.Cost + ")";
                if (!AllPilots.ContainsKey(pilotKey))
                {
                    AllPilots.Add(pilotKey, type.ToString());
                    PilotSkill.Add(pilotKey, newShipContainer.PilotSkill);
                }
                result.Add(pilotKey);
            }
        }

        return result;
    }

    private static void SetAvailableUpgrades(SquadBuilderShip squadBuilderShip)
    {
        foreach (var upgradeSlot in squadBuilderShip.Ship.UpgradeBar.GetUpgradeSlots())
        {
            AddUpgrade(squadBuilderShip, upgradeSlot);
        }
    }

    private static void AddUpgrade(SquadBuilderShip squadBuilderShip, UpgradeSlot upgradeSlot)
    {
        List<string> upgradeList = GetUpgradesList(squadBuilderShip, upgradeSlot);
        GameObject panel = CreateUpgradePanel(squadBuilderShip, upgradeSlot);

        SquadBuilderUpgrade upgrade = new SquadBuilderUpgrade(upgradeSlot, panel);
        squadBuilderShip.Upgrades.Add(upgrade);

        SetUpgradesDropdown(squadBuilderShip, upgrade, upgradeList);
    }

    private static List<string> GetUpgradesList(SquadBuilderShip squadBuilderShip, UpgradeSlot upgradeSlot)
    {
        List<string> results = new List<string>();

        List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, "UpgradesList", StringComparison.Ordinal))
            .ToList();

        foreach (var type in typelist)
        {
            Upgrade.GenericUpgrade newUpgrade = (Upgrade.GenericUpgrade)System.Activator.CreateInstance(type);
            if (!newUpgrade.IsHidden)
            {
                if (newUpgrade.Type == upgradeSlot.Type && newUpgrade.IsAllowedForShip(squadBuilderShip.Ship))
                {
                    string upgradeKey = newUpgrade.Name + " (" + newUpgrade.Cost + ")";
                    if (!AllUpgrades.ContainsKey(upgradeKey))
                    {
                        AllUpgrades.Add(upgradeKey, type.ToString());
                    }
                    results.Add(upgradeKey);
                }
            }
        }

        return results;
    }

    //Change faction

    public static void PlayerFactonChange()
    {
        SetPlayerFactions();
        CheckPlayerFactonChange(PlayerNo.Player1);
        CheckPlayerFactonChange(PlayerNo.Player2);
    }

    //Go to battle

    public static void StartGame()
    {
        SetPlayers();
        GeneratePlayersShipConfigurations();
        if (ValidatePlayersRosters())
        {
            SceneManager.LoadScene("Battle");
        }
    }

    private static bool ValidatePlayersRosters()
    {
        if (!ValidatePlayerRoster(PlayerNo.Player1)) return false;
        if (!ValidatePlayerRoster(PlayerNo.Player2)) return false;
        return true;
    }

    private static bool ValidatePlayerRoster(PlayerNo playerNo)
    {
        if (!ValidateUniqueCards(playerNo)) return false;
        if (!ValidateSquadCost(playerNo)) return false;
        return true;
    }

    private static bool ValidateSquadCost(PlayerNo playerNo)
    {
        bool result = true;

        int squadCost = 0;

        foreach (var shipConfig in Global.ShipConfigurations)
        {
            if (shipConfig.PlayerNo == playerNo)
            {
                GenericShip shipContainer = (GenericShip)System.Activator.CreateInstance(System.Type.GetType(shipConfig.PilotName));
                squadCost += shipContainer.Cost;

                foreach (var upgrade in shipConfig.Upgrades)
                {
                    Upgrade.GenericUpgrade upgradeContainer = (Upgrade.GenericUpgrade)System.Activator.CreateInstance(System.Type.GetType(upgrade));
                    squadCost += upgradeContainer.Cost;
                }
            }
        }

        if (!DebugManager.DebugNoSquadPointsLimit)
        {
            if (squadCost > 100)
            {
                Messages.ShowError("Cost of squadron cannot be more than 100");
                result = false;
            }
        }

        return result;
    }

    private static bool ValidateUniqueCards(PlayerNo playerNo)
    {
        bool result = true;

        List<string> uniqueCards = new List<string>();
        foreach (var shipConfig in Global.ShipConfigurations)
        {
            if (shipConfig.PlayerNo == playerNo)
            {
                GenericShip shipContainer = (GenericShip)System.Activator.CreateInstance(System.Type.GetType(shipConfig.PilotName));

                if (shipContainer.IsUnique)
                {
                    if (CheckDuplicate(uniqueCards, shipContainer.PilotName)) return false;
                }

                foreach (var upgrade in shipConfig.Upgrades)
                {
                    GenericUpgrade upgradeContainer = (GenericUpgrade)System.Activator.CreateInstance(System.Type.GetType(upgrade));
                    if (upgradeContainer.isUnique)
                    {
                        if (CheckDuplicate(uniqueCards, upgradeContainer.Name)) return false;
                    }
                }
            }
        }
        return result;
    }

    private static bool CheckDuplicate(List<string> uniqueCards, string cardName)
    {
        if (uniqueCards.Contains(cardName))
        {
            Messages.ShowError("Only one card with unique name " + cardName + " can be present");
            return true;
        }
        else
        {
            uniqueCards.Add(cardName);
            return false;
        }
    }

}
