using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Upgrade;

namespace Ship.SecondEdition.YT2400LightFreighter
{
    public class YT2400LightFreighter : FirstEdition.YT2400.YT2400
    {
        public YT2400LightFreighter() : base()
        {
            ShipInfo.ShipName = "YT-2400 Light Freighter";

            ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.DoubleTurret, 4);

            ShipInfo.Hull = 6;
            ShipInfo.Shields = 4;

            ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Cannon);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

            ShipInfo.ActionIcons.RemoveActions(typeof(BarrelRollAction));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(RotateArcAction)));

            ShipAbilities.Add(new Abilities.SecondEdition.SensorBlindspot());

            IconicPilots[Faction.Rebel] = typeof(DashRendar);

            ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/11/Maneuver_yt-2400.png";

            OldShipTypeName = "YT-2400";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SensorBlindspot : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckSensorBlindspot;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckSensorBlindspot;
        }

        private void CheckSensorBlindspot(ref int count)
        {
            if (Combat.ShotInfo.Range < 2) count -= 2;
        }
    }
}
