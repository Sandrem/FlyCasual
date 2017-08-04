using UnityEngine;

namespace RulesList
{
    public class WinConditionsRule
    {
        private GameManagerScript Game;

        public WinConditionsRule(GameManagerScript game)
        {
            Game = game;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Phases.OnRoundEnd += CheckWinConditions;
        }

        public void CheckWinConditions()
        {
            int eliminatedTeam = Roster.CheckIsAnyTeamIsEliminated();

            if (eliminatedTeam != 0)
            {
                if (eliminatedTeam < 3)
                {
                    Game.UI.AddTestLogEntry("Player " + Roster.AnotherPlayer(eliminatedTeam) + " Wins!");
                    Game.UI.ShowGameResults("Player " + Roster.AnotherPlayer(eliminatedTeam) + " Wins!");
                }
                else
                {
                    Game.UI.AddTestLogEntry("Draw!");
                    Game.UI.ShowGameResults("Draw!");
                }
            }
        }
    }
}
