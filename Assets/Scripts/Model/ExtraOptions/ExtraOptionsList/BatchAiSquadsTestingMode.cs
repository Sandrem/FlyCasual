using Players;
using System.Collections.Generic;
using UnityEngine;

namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class BatchAiSquadsTestingModeExtraOption : ExtraOption
        {
            public static Dictionary<PlayerNo, int> Results = new Dictionary<PlayerNo, int>()
            {
                { PlayerNo.Player1, 0 },
                { PlayerNo.Player2, 0 }
            };

            public BatchAiSquadsTestingModeExtraOption()
            {
                Name = "Batch Ai Squads Testing Mode";
                Description = "For AI vs AI mode only: No animations / delays, dialog windows, dice results are generated without rolling. When match is finished - new match is started. Number of wins is shown instead of titles of players";
            }

            protected override void Activate()
            {
                DebugManager.BatchAiSquadTestingMode = true;
            }

            protected override void Deactivate()
            {
                DebugManager.BatchAiSquadTestingMode = false;
            }

            public static void ClearResults()
            {
                Results[PlayerNo.Player1] = 0;
                Results[PlayerNo.Player2] = 0;
            }
        }
    }
}
