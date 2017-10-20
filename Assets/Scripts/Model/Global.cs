using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;
using Ship;

public class Global : MonoBehaviour {

    public static string test = "I am accessible from every scene";

    public static string CurrentVersion = "0.2.0";

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
                new ShipConfiguration
                (
                    new Ship.AWing.GreenSquadronPilot(),
                    PlayerNo.Player1,
                    19
                ),
                new ShipConfiguration
                (
                    new Ship.TIEAdvanced.TempestSquadronPilot(),
                    PlayerNo.Player2,
                    21
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
            List<System.Type> result = null;
            if (Network.IsServer)
            {
                result = new List<System.Type>
                {
                    typeof(HumanPlayer),
                    typeof(NetworkOpponentPlayer)
                };
            }
            else
            {
                result = new List<System.Type>
                {
                    typeof(NetworkOpponentPlayer),
                    typeof(HumanPlayer)
                };
            }
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
        shipConfigurations.Add(new ShipConfiguration(ship, playerNo, shipCost));
    }

    public static Faction GetPlayerFaction(PlayerNo playerNo)
    {
        Faction result = Faction.Rebels;
        if (playerNo == PlayerNo.Player1) result = playerFactions[0];
        if (playerNo == PlayerNo.Player2) result = playerFactions[1];
        return result;
    }

    public static void StartBattle()
    {
        if (Network.IsNetworkGame) HideOpponentSquad();
        Phases.StartPhases();
    }

    private static void HideOpponentSquad()
    {
        Transform opponentSquad = GameObject.Find("GlobalUI").transform.Find("OpponentSquad");
        if (opponentSquad != null) opponentSquad.gameObject.SetActive(false);
    }

}
