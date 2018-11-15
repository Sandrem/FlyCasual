using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Movement;
using Upgrade;

namespace Ship.SecondEdition.VCX100LightFreighter
{
    public class VCX100LightFreighter : FirstEdition.VCX100.VCX100
    {
        public VCX100LightFreighter() : base()
        {
            ShipInfo.ShipName = "VCX-100 Light Freighter";

            ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.System);
            ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Torpedo);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);

            ShipInfo.ActionIcons.Actions.RemoveAll(a => a.ActionType == typeof(EvadeAction));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReinforceAftAction)));

            DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
            DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn), MovementComplexity.Complex);

            IconicPilots[Faction.Rebel] = typeof(LothalRebel);

            // ManeuversImageUrl

            /* HotacManeuverTable = new AI.XWingTable(); */
        }
    }
}