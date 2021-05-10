using SubPhases;
using System;

namespace GameCommands
{
    public class CancelShipSelectionCommand : GameCommand
    {
        public CancelShipSelectionCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            if ((Phases.CurrentSubPhase is SelectShipSubPhase))
            {
                (Phases.CurrentSubPhase as SelectShipSubPhase).CallRevertSubPhase();
            }
        }
    }

}
