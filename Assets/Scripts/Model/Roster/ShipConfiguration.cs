using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ShipConfiguration
{
    public string PilotName { get; private set; }
    public List<string> Upgrades { get; private set; }
    public Players.PlayerNo PlayerNo { get; private set; }

    public ShipConfiguration(string pilotName, List<string> upgrades, Players.PlayerNo playerNo)
    {
        PilotName = pilotName;
        Upgrades = upgrades;
        PlayerNo = playerNo;
    }

}
