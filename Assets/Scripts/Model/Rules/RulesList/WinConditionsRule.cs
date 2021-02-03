using ExtraOptions.ExtraOptionsList;
using Players;
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
                    if (DebugManager.BatchAiSquadTestingModeActive)
                    {
                        BatchAiSquadsTestingModeExtraOption.Results[Tools.IntToPlayer(Roster.AnotherPlayer(eliminatedTeam))]++;
                    }
                    UI.AddTestLogEntry(GetPlayerName(Roster.AnotherPlayer(eliminatedTeam)) + " Wins!");
                    UI.ShowGameResults(GetPlayerName(Roster.AnotherPlayer(eliminatedTeam)) + " Wins!");
                }
                else
                {
                    if (DebugManager.BatchAiSquadTestingModeActive)
                    {
                        BatchAiSquadsTestingModeExtraOption.Results[PlayerNo.Player1]++;
                        BatchAiSquadsTestingModeExtraOption.Results[PlayerNo.Player2]++;
                    }
                    UI.AddTestLogEntry("Draw!");
                    UI.ShowGameResults("Draw!");
                }

                if (DebugManager.ReleaseVersion) AnalyticsEvent.GameOver();

                Rules.FinishGame();
            }
        }

        private string GetPlayerName(int playerNo)
        {
            if (Roster.GetPlayer(1).NickName != Roster.GetPlayer(2).NickName)
            {
                return Roster.GetPlayer(playerNo).NickName;
            }
            else
            {
                return "Player " + playerNo;
            }
        }
    }
}
