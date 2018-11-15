using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Upgrade;

namespace Ship.SecondEdition.YT2400LightFreighter
{
    public class YT2400LightFreighter : FirstEdition.YT2400.YT2400
    {
        public YT2400LightFreighter() : base()
        {
            ShipInfo.ShipName = "YT-2400 Light Freighter";

            ShipInfo.ArcInfo.Firepower = 4;
            ShipInfo.Hull = 6;
            ShipInfo.Shields = 4;

            ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Cannon);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

            ShipInfo.ActionIcons.Actions.RemoveAll(a => a.ActionType == typeof(BarrelRollAction));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(RotateArcAction)));

            ShipAbilities.Add(new Abilities.SecondEdition.SensorBlindspot());

            IconicPilots[Faction.Rebel] = typeof(WildSpaceFringer);

            // ManeuversImageUrl

            /* HotacManeuverTable = new AI.XWingTable(); */
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
