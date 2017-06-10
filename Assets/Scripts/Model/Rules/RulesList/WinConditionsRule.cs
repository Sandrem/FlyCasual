using UnityEngine;

namespace RulesList
{
    public class WinConditionsRule
    {
        private GameManagerScript Game;

        public WinConditionsRule(GameManagerScript game)
        {
            Game = game;
        }

        public void CheckWinConditions()
        {
            int eliminatedTeam = Roster.CheckIsAnyTeamIsEliminated();

            if (eliminatedTeam != 0)
            {
                Game.UI.AddTestLogEntry("Player " + Roster.AnotherPlayer(eliminatedTeam) + " Wins!");
                Game.UI.ShowGameResults("Player " + Roster.AnotherPlayer(eliminatedTeam) + " Wins!");
            }
        }
    }
}
