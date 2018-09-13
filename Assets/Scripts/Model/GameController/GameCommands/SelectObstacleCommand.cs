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
        public SelectObstacleCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            SelectObstacleSubPhase.ConfirmSelectionOfObstacle(GetString("name"));
        }
    }

}
