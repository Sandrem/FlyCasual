using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;

public partial class ShipRoster
{

    public List<GenericPlayer> Players = new List<GenericPlayer>();

    private GenericPlayer player1;
    public GenericPlayer Player1
    {
        get { return Players[0]; }
    }

    private GenericPlayer player2;
    public GenericPlayer Player2
    {
        get { return Players[1]; }
    }

    //TODO: Change to list, use linq
    public Dictionary<string, Ship.GenericShip> AllShips = new Dictionary<string, Ship.GenericShip>();
    private List<string> team1 = new List<string>();
    private List<string> team2 = new List<string>();

}
