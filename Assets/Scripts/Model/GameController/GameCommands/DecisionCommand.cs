using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class DecisionCommand : GameCommand
    {
        public DecisionCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            string decisionName = GetString("name");

            Console.Write($"Decision is taken: {decisionName}");

            DecisionSubPhase.ExecuteDecision(decisionName);
        }
    }

}
