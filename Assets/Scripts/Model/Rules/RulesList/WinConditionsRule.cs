using UnityEngine;

namespace RulesList
{
    public class WinConditionsRule
    {

        public WinConditionsRule()
        {
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
                    UI.AddTestLogEntry("Player " + Roster.AnotherPlayer(eliminatedTeam) + " Wins!");
                    UI.ShowGameResults("Player " + Roster.AnotherPlayer(eliminatedTeam) + " Wins!");
                }
                else
                {
                    UI.AddTestLogEntry("Draw!");
                    UI.ShowGameResults("Draw!");
                }

                Phases.GameIsEnded = true;
            }
        }
    }
}
