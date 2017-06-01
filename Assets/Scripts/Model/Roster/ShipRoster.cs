using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;

public static partial class Roster
{
    //Players

    public static List<GenericPlayer> Players = new List<GenericPlayer>();

    private static GenericPlayer player1;
    public static GenericPlayer Player1 { get { return Players[0]; } }

    private static GenericPlayer player2;
    public static GenericPlayer Player2 { get { return Players[1]; } }

    //Ships

    public static Dictionary<string, Ship.GenericShip> AllShips = new Dictionary<string, Ship.GenericShip>();

    private static Dictionary<string, Ship.GenericShip> shipsPlayer1;
    public static Dictionary<string, Ship.GenericShip> ShipsPlayer1 { get { return Players[0].Ships; } }

    private static Dictionary<string, Ship.GenericShip> shipsPlayer2;
    public static Dictionary<string, Ship.GenericShip> ShipsPlayer2 {get { return Players[1].Ships; } }

    private static GameManagerScript Game;

    public static void Start()
    {
        CreatePlayers();
        SpawnAllShips();
    }

}
