using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;
using Ship;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class TIESfFighter : FirstEdition.TIESfFighter.TIESfFighter, TIE
        {
            public TIESfFighter() : base()
            {
                ShipInfo.ShipName = "TIE/sf Fighter";

                ShipInfo.DefaultShipFaction = Faction.FirstOrder;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.FirstOrder };

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(EvadeAction)));

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(FocusAction), typeof(RotateArcAction), ActionColor.White));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(TargetLockAction), typeof(RotateArcAction), ActionColor.White));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(EvadeAction), typeof(RotateArcAction), ActionColor.White));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(RotateArcAction), ActionColor.White));

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);

                IconicPilots[Faction.FirstOrder] = typeof(OmegaSquadronExpert);

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/44/Maneuver_tie_phantom.png";
            }
        }
    }
}