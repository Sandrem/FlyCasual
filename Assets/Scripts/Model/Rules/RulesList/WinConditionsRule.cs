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
            switch (eliminatedTeam)
            {
                case 1:
                    Game.UI.AddTestLogEntry("Empire Wins!");
                    Game.UI.ShowGameResults("Empire Wins!");
                    break;
                case 2:
                    Game.UI.AddTestLogEntry("Rebels Win!");
                    Game.UI.ShowGameResults("Rebels Win!");
                    break;
                default:
                    break;
            }
        }
    }
}
