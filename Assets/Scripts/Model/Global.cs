using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;

public class Global : MonoBehaviour {

    public static string test = "I am accessible from every scene";

    private static List<ShipConfiguration> shipConfigurations = new List<ShipConfiguration>();

    private static List<System.Type> playerTypes = new List<System.Type>();

    private static List<Faction> playerFactions = new List<Faction>();

    public static List<Faction> PlayerFactions
    {
        get { return playerFactions; }
        private set { playerFactions = value; }
    }

    public static List<System.Type> PlayerTypes
    {
        get { return playerTypes; }
        private set { playerTypes = value; }
    }

    public static List<ShipConfiguration> ShipConfigurations
    {
        get { return shipConfigurations; }
        private set { shipConfigurations = value; }
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static void Initialize()
    {
        PlayerTypes = GetPlayerTypes();
        ShipConfigurations = GetShipConfigurations();
    }

    private static List<ShipConfiguration> GetShipConfigurations()
    {
        List<ShipConfiguration> result = new List<ShipConfiguration>();
        if (shipConfigurations.Count != 0)
        {
            result = shipConfigurations;
        }
        else
        {
            result = new List<ShipConfiguration>()
                {
                    new ShipConfiguration
                    (
                        "Ship.XWing.LukeSkywalker",
                        new List<string>() { "Upgrade.R2D2", "Upgrade.Marksmanship", "Upgrade.ProtonTorpedoes" },
                        PlayerNo.Player1
                    ),
                    new ShipConfiguration
                    (
                        "Ship.TIEFighter.MaulerMithel",
                        new List<string>() { "Upgrade.Determination" },
                        PlayerNo.Player2
                    ),
                    new ShipConfiguration
                    (
                        "Ship.TIEFighter.NightBeast",
                        new List<string>(),
                        PlayerNo.Player2
                    )
                };  
        }
        return result;
    }

    private static List<System.Type> GetPlayerTypes()
    {
        if (playerTypes.Count != 0)
        {
            return playerTypes;
        }
        else
        {
            List<System.Type> result = new List<System.Type>
                {
                    typeof(Players.HumanPlayer),
                    typeof(Players.HotacAiPlayer)
                };
            return result;
        }
    }

    public static void RemoveAllPlayers()
    {
        playerTypes = new List<System.Type>();
    }

    public static void RemoveAllFactions()
    {
        playerFactions = new List<Faction>();
    }

    public static void AddPlayer(System.Type playerType)
    {
        playerTypes.Add(playerType);
    }

    public static void AddFaction(Faction factionType)
    {
        playerFactions.Add(factionType);
    }

    public static void AddShip(string name, List<string> upgradeList, PlayerNo playerNo)
    {
        shipConfigurations.Add(new ShipConfiguration(name, upgradeList, playerNo));
    }

    public static Faction GetPlayerFaction(PlayerNo playerNo)
    {
        Faction result = Faction.Rebels;
        if (playerNo == PlayerNo.Player1) result = playerFactions[0];
        if (playerNo == PlayerNo.Player2) result = playerFactions[1];
        return result;
    }

}
