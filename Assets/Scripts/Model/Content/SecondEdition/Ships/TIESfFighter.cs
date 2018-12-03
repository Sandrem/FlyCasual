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

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);

                IconicPilots[Faction.FirstOrder] = typeof(OmegaSquadronExpert);

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/44/Maneuver_tie_phantom.png";
            }
        }
    }
}