using System;
using System.Collections.Generic;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class DroidTriFighterGeneric : DroidTriFighter
    {
        public DroidTriFighterGeneric()
        {
            PilotInfo = new PilotCardInfo(
                "Droid Tri-Fighter Generic",
                1,
                31
            );
        }
    }
}