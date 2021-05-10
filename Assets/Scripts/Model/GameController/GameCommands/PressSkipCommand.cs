using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class PressSkipCommand : GameCommand
    {
        public PressSkipCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            Console.Write("Button \"Skip\" is used");
            Phases.CurrentSubPhase.SkipButton();
        }
    }

}
