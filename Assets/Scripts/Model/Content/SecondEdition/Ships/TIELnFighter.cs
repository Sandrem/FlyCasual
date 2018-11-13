using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class TIELnFighter : FirstEdition.TIEFighter.TIEFighter
        {
            public TIELnFighter() : base()
            {
                ShipInfo.ShipName = "TIE/ln Fighter";

                IconicPilots[Faction.Imperial] = typeof(BlackSquadronAce);
            }
        }
    }
}
