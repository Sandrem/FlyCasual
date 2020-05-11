using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class CancelShipSelectionCommand : GameCommand
    {
        public CancelShipSelectionCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            if ((Phases.CurrentSubPhase is SelectShipSubPhase))
            {
                (Phases.CurrentSubPhase as SelectShipSubPhase).CallRevertSubPhase();
            }
            else
            {
                Console.Write("Error during CancelShipSelection command execution. Current subphase is " + Phases.CurrentSubPhase.ToString(), color: "red");
            }
        }
    }

}
