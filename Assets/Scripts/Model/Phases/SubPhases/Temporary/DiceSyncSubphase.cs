using System;
using System.Collections.Generic;

namespace SubPhases
{
    public class DiceSyncSubphase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes => new List<GameCommandTypes>()
        {
            GameCommandTypes.SyncDiceResults
        };

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            CallBack();
        }
    }
}
