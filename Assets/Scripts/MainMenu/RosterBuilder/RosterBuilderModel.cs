using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using Players;

public static partial class RosterBuilder {

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
        GeneratePlayerShipConfigurations(PlayerNo.Player1);
        GeneratePlayerShipConfigurations(PlayerNo.Player2);
    }

    private static void AddInitialShips()
    {
        AddShip(PlayerNo.Player1);
        AddShip(PlayerNo.Player2);
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
            //Show Error
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

    private static void SetAvailableUpgrades(GameObject panel, string pilotName)
    {
        string pilotId = AllPilots[pilotName];
        Ship.GenericShip ship = (Ship.GenericShip)System.Activator.CreateInstance(System.Type.GetType(pilotId));
        foreach (var upgrade in ship.BuiltInSlots)
        {
            AddUpgradeLine(panel, upgrade.Key.ToString());
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

    public static void PrepareForGameStart()
    {
        SetPlayers();
        GeneratePlayersShipConfigurations();
    }

}
