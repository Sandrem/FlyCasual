using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using System.Linq;
using Players;

public static partial class RosterBuilder {

    private static Dictionary<string, string> AllShips = new Dictionary<string, string>();

    private static Dictionary<string, string> AllPilots = new Dictionary<string, string>();
    private static Dictionary<string, int> PilotSkill = new Dictionary<string, int>();

    private static Dictionary<string, string> AllUpgrades = new Dictionary<string, string>();

    public static void Initialize()
    {
        SetPlayers();
        SetPlayerFactions();
        GenerateShipsList();
        AddInitialShips();
    }

    //Initialization

    private static void SetPlayers()
    {
        Global.RemoveAllPlayers();
        Global.AddPlayer(GetPlayerType(PlayerNo.Player1));
        Global.AddPlayer(GetPlayerType(PlayerNo.Player2));
    }

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
        if (GetShipsCount(PlayerNo.Player1) == 0) AddShip(PlayerNo.Player1);
        if (GetShipsCount(PlayerNo.Player2) == 0) AddShip(PlayerNo.Player2);
    }

    public static void AddShip(PlayerNo playerNo)
    {
        if (GetShipsCount(playerNo) < 8)
        {

            GameObject newPanel = CreateShipPanel(playerNo);
            SetShip(newPanel, playerNo);

            OrganizeShipsList(playerNo);
        }
        else
        {
            Messages.ShowError("You cannot have more than 8 ships");
        }
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
            if (newShipContainer.PilotName != null)
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
                string upgradeKey = newUpgrade.Name + " (" + newUpgrade.Cost + ")";
                if (!AllUpgrades.ContainsKey(upgradeKey))
                {
                    AllUpgrades.Add(upgradeKey, type.ToString());
                }
                results.Add(upgradeKey);
            }
        }

        return results;
    }

    private static void SetAvailableUpgrades(PlayerNo playerNo, GameObject panel, string pilotName)
    {
        string pilotId = AllPilots[pilotName];
        Ship.GenericShip ship = (Ship.GenericShip)System.Activator.CreateInstance(System.Type.GetType(pilotId));
        foreach (var upgrade in ship.BuiltInSlots)
        {
            AddUpgradeLine(playerNo, panel, upgrade.Key.ToString());
        }
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
        DetermineInitiative();
        if (ValidatePlayersRosters())
        {
            SceneManager.LoadScene("Battle");
        }
    }

    private static void DetermineInitiative()
    {
        int costP1 = GetShipCostCalculated(PlayerNo.Player1);
        int costP2 = GetShipCostCalculated(PlayerNo.Player2);

        if (costP1 < costP2)
        {
            Global.PlayerWithInitiative = PlayerNo.Player1;
        }
        else if (costP1 > costP2)
        {
            Global.PlayerWithInitiative = PlayerNo.Player2;
        }
        else
        {
            int randomPlayer = UnityEngine.Random.Range(1, 3);
            Global.PlayerWithInitiative = Tools.IntToPlayer(randomPlayer);
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
                Ship.GenericShip shipContainer = (Ship.GenericShip)System.Activator.CreateInstance(System.Type.GetType(shipConfig.PilotName));
                squadCost += shipContainer.Cost;

                foreach (var upgrade in shipConfig.Upgrades)
                {
                    Upgrade.GenericUpgrade upgradeContainer = (Upgrade.GenericUpgrade)System.Activator.CreateInstance(System.Type.GetType(upgrade));
                    squadCost += upgradeContainer.Cost;
                }
            }
        }

        if (squadCost > 100)
        {
            Messages.ShowError("Cost of squadron cannot be more than 100");
            result = false;
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
                Ship.GenericShip shipContainer = (Ship.GenericShip)System.Activator.CreateInstance(System.Type.GetType(shipConfig.PilotName));

                if (shipContainer.IsUnique)
                {
                    if (CheckDuplicate(uniqueCards, shipContainer.PilotName)) return false;
                }

                foreach (var upgrade in shipConfig.Upgrades)
                {
                    Upgrade.GenericUpgrade upgradeContainer = (Upgrade.GenericUpgrade)System.Activator.CreateInstance(System.Type.GetType(upgrade));
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
