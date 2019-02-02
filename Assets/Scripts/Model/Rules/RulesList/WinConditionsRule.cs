using UnityEngine;
using UnityEngine.Analytics;

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
            Phases.Events.OnRoundEnd += CheckWinConditions;
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

                if (DebugManager.ReleaseVersion) AnalyticsEvent.GameOver();

                Rules.FinishGame();
            }
        }
    }
}
