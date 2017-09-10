using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;
using Ship;

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

    void Update()
    {
        Tooltips.CheckTooltip();
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
                /*new ShipConfiguration
                (
                    "Ship.YT1300.OuterRimSmuggler",
                    new List<string>(),
                    PlayerNo.Player1,
                    0
                ),
                new ShipConfiguration
                (
                    "Ship.TIEAdvanced.TempestSquadronPilot",
                    new List<string>(),
                    PlayerNo.Player2,
                    0
                )
                new ShipConfiguration
                (
                    "Ship.XWing.LukeSkywalker",
                    new List<string>() { "UpgradesList.R2D2", "UpgradesList.Marksmanship", "UpgradesList.ProtonTorpedoes" },
                    PlayerNo.Player1,
                    1
                ),
                new ShipConfiguration
                (
                    "Ship.TIEFighter.MaulerMithel",
                    new List<string>() { "UpgradesList.Determination" },
                    PlayerNo.Player2,
                    1
                ),
                new ShipConfiguration
                (
                    "Ship.TIEFighter.NightBeast",
                    new List<string>(),
                    PlayerNo.Player2,
                    1
                )*/
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
                    typeof(Players.HumanPlayer)
                };
            return result;
        }
    }

    public static void RemoveAllPlayers()
    {
        playerTypes = new List<System.Type>();
    }

    public static void RemoveAllShips()
    {
        shipConfigurations = new List<ShipConfiguration>();
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

    public static void AddShip(GenericShip ship, PlayerNo playerNo, int shipCost)
    {
        Debug.Log(ship.PilotName + " " + ship.UpgradeBar.GetInstalledUpgrades().Count + " " + playerNo + " " + shipCost);
        shipConfigurations.Add(new ShipConfiguration(ship, playerNo, shipCost));
    }

    public static Faction GetPlayerFaction(PlayerNo playerNo)
    {
        Faction result = Faction.Rebels;
        if (playerNo == PlayerNo.Player1) result = playerFactions[0];
        if (playerNo == PlayerNo.Player2) result = playerFactions[1];
        return result;
    }

}
