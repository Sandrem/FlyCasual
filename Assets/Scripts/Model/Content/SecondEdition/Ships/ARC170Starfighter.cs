using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class ARC170Starfighter : FirstEdition.ARC170.ARC170
        {
            public ARC170Starfighter() : base()
            {
                ShipInfo.ShipName = "ARC-170 Starfighter";
                ShipInfo.ArcInfo.Firepower = 3;
                ShipInfo.BaseSize = BaseSize.Medium;

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));

                IconicPilots[Faction.Rebel] = typeof(NorraWexley);

                //TODO: ManeuversImageUrl
            }
        }
    }
}
