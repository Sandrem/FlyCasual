using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {

    public static string test = "I am accessible from every scene";

    private static List<ShipConfiguration> shipConfigurations;

    private static List<System.Type> playerTypes;

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
        List<ShipConfiguration> result = new List<ShipConfiguration>()
        {
            new ShipConfiguration
            (
                "Ship.XWing.LukeSkywalker",
                new List<string>() { "Upgrade.R2D2", "Upgrade.Marksmanship" },
                Players.PlayerNo.Player1
            ),
            new ShipConfiguration
            (
                "Ship.TIEFighter.MaulerMithel",
                new List<string>() { "Upgrade.Determination" },
                Players.PlayerNo.Player2
            ),
            new ShipConfiguration
            (
                "Ship.TIEFighter.NightBeast",
                new List<string>(),
                Players.PlayerNo.Player2
            )
        };
        return result;
    }

    private static List<System.Type> GetPlayerTypes()
    {
        List<System.Type> result = new List<System.Type>
        {
            typeof(Players.HumanPlayer),
            typeof(Players.HumanPlayer)
        };
        return result;
    }





}
