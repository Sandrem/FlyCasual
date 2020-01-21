using Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class DiceStatsTracker
{
    public static Dictionary<PlayerNo, PlayerDiceStats> DiceStats { get; private set; }

    public static void ReadFromString(string inputString)
    {
        if (DiceStats == null)
        {
            string[] playerBlocks = inputString.Split('&');
            DiceStats = new Dictionary<PlayerNo, PlayerDiceStats>()
            {
                { PlayerNo.Player1, new PlayerDiceStats(PlayerNo.Player1, null, playerBlocks[0]) },
                { PlayerNo.Player2, new PlayerDiceStats(PlayerNo.Player2, null, playerBlocks[1]) }
            };
        }
    }

    public static void Update()
    {
        // Only for Human vs AI
        if (Roster.GetPlayer(PlayerNo.Player1).GetType() == typeof(HumanPlayer) && Roster.GetPlayer(PlayerNo.Player2).GetType() == typeof(AggressorAiPlayer))
        {
            DiceStats[PlayerNo.Player1].Update(StatsViewScript.Instance.GetStats(PlayerNo.Player1));
            DiceStats[PlayerNo.Player2].Update(StatsViewScript.Instance.GetStats(PlayerNo.Player2));

            string statToSave = DiceStats[PlayerNo.Player1].ToString() + "&" + DiceStats[PlayerNo.Player2].ToString();
            PlayerPrefs.SetString("DiceStats", statToSave);
            PlayerPrefs.Save();
        }
    }
}
