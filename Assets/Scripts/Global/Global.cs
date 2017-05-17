using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {

    public static string test = "I am accessible from every scene";

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
    }
	
    public static List<ShipConfiguration> GetShipConfigurations()
    {
        List<ShipConfiguration> result = new List<ShipConfiguration>();
        ShipConfiguration newShip;
        newShip = new ShipConfiguration(
            "Ship.XWing.LukeSkywalker",
            new List<string>() { "Upgrade.R2D2", "Upgrade.Marksmanship" },
            Players.PlayerNo.Player1);
        result.Add(newShip);

        newShip = new ShipConfiguration(
            "Ship.TIEFighter.MaulerMithel",
            new List<string>() { "Upgrade.Determination" },
            Players.PlayerNo.Player2);
        result.Add(newShip);

        newShip = new ShipConfiguration(
            "Ship.TIEFighter.NightBeast",
            new List<string>(),
            Players.PlayerNo.Player2);
        result.Add(newShip);

        return result;
    }
}
