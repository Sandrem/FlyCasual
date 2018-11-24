using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Movement;
using Upgrade;

namespace Ship.SecondEdition.CustomizedYT1300LightFreighter
{
    public class CustomizedYT1300LightFreighter : Ship.SecondEdition.ModifiedYT1300LightFreighter.ModifiedYT1300LightFreighter
    {
        public CustomizedYT1300LightFreighter() : base()
        {
            ShipInfo.ShipName = "Customized YT-1300 Light Freighter";

            ShipInfo.DefaultShipFaction = Faction.Scum;
            ShipInfo.FactionsAll = new List<Faction>() { Faction.Scum };
            ShipInfo.ArcInfo.Firepower = 2;
            ShipInfo.Hull = 8;
            ShipInfo.Shields = 3;

            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Missile);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(RotateArcAction)));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction), ActionColor.Red));

            IconicPilots[Faction.Scum] = typeof(HanSolo);

            ModelInfo = new ShipModelInfo(
                "Customized YT-1300 Light Freighter",
                "Default"
            );

            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);

            // ManeuversImageUrl

            /* HotacManeuverTable = new AI.XWingTable(); */
        }
    }
}
