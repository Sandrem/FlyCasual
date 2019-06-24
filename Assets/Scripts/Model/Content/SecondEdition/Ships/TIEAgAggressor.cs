using System.Collections;
using System.Collections.Generic;
using ActionsList;
using Actions;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAgAggressor
    {
        public class TIEAgAggressor : FirstEdition.TIEAggressor.TIEAggressor, TIE
        {
            public TIEAgAggressor() : base()
            {
                ShipInfo.ShipName = "TIE/ag Aggressor";
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(EvadeAction)));
                IconicPilots[Faction.Imperial] = typeof(LieutenantKestal);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/4d/Maneuver_tie_aggressor.png";

                OldShipTypeName = "TIE Aggressor";
            }
        }
    }
}