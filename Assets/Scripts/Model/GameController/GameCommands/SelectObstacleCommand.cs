using Players;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class SelectObstacleCommand : GameCommand
    {
        public SelectObstacleCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            string obstacleName = GetString("name");

            Console.Write($"Obstacle is chosen: {obstacleName}");

            if (Phases.CurrentSubPhase is SelectObstacleSubPhase) SelectObstacleSubPhase.ConfirmSelectionOfObstacle(obstacleName);
            else if (Phases.CurrentSubPhase is SelectTargetLockableSubPhase) SelectTargetLockableSubPhase.ConfirmSelectionOfObstacle(obstacleName);
        }
    }

}
