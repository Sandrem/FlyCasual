using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class ConfirmDiceCheckCommand : GameCommand
    {
        public ConfirmDiceCheckCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            (Phases.CurrentSubPhase as DiceRollCheckSubPhase).Confirm();
        }
    }

}
