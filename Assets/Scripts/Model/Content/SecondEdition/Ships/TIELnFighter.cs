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

                IconicPilots = new Dictionary<Faction, System.Type> {
                    { Faction.Imperial, typeof(BlackSquadronAce) },
                    { Faction.Rebel, typeof(ZebOrrelios) }
                };

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/6/62/Maneuver_tie_ln_fighter.png";

                OldShipTypeName = "TIE Fighter";
            }
        }
    }
}
