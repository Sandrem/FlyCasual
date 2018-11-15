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
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), typeof(EvadeAction)));
                IconicPilots[Faction.Imperial] = typeof(SienarSpecialist);
            }
        }
    }
}