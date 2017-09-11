using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Players;

public partial class ShipConfiguration
{
    public GenericShip Ship { get; private set; }
    public PlayerNo Player { get; private set; }
    public int ShipCost { get; private set; }

    public ShipConfiguration(GenericShip ship, PlayerNo playerNo, int shipCost)
    {
        Ship = ship;
        Player = playerNo;
        ShipCost = shipCost;
    }

}
