using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Global : MonoBehaviour {

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

    private void Initialize()
    {
        PlayerTypes = GetPlayerTypes();
        ShipConfigurations = GetShipConfigurations();
    }

}
