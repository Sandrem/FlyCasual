using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ShipConfiguration
{

    public ShipConfiguration(string pilotName, List<string> upgrades, Players.PlayerNo playerNo)
    {
        PilotName = pilotName;
        Upgrades = upgrades;
        PlayerNo = playerNo;
    }

}
