using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Upgrade;

namespace Ship.SecondEdition.VCX100LightFreighter
{
    public class VCX100LightFreighter : FirstEdition.VCX100.VCX100
    {
        public VCX100LightFreighter() : base()
        {
            ShipInfo.ShipName = "VCX-100 Light Freighter";

            ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.Front, 4);
            ShipInfo.Shields = 4;

            ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Torpedo);
            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);

            ShipInfo.ActionIcons.RemoveActions(typeof(EvadeAction));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReinforceAction)));

            DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
            DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn), MovementComplexity.Complex);

            IconicPilots[Faction.Rebel] = typeof(KananJarrus);

            ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/6/61/Maneuver_vcx-100.png";

            ShipAbilities.Add(new Abilities.SecondEdition.TailGunnerAbility());
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TailGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAnotherShipDocked += ActivateRearArc;
            HostShip.OnAnotherShipUndocked += DectivateRearArc;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAnotherShipDocked -= ActivateRearArc;
            HostShip.OnAnotherShipUndocked -= DectivateRearArc;
        }

        private void ActivateRearArc(GenericShip ship)
        {
            HostShip.PrimaryWeapons.Add(new PrimaryWeaponClass(HostShip, new ShipArcInfo(ArcType.Rear, ship.ShipInfo.Firepower)));
            HostShip.ArcsInfo.Arcs.Add(new ArcRear(HostShip.ShipBase));
            HostShip.SetShipInsertImage();
        }

        private void DectivateRearArc(GenericShip ship)
        {
            HostShip.PrimaryWeapons
                .RemoveAll(
                    a => a.WeaponType == WeaponTypes.PrimaryWeapon
                    && a.WeaponInfo.AttackValue == ship.ShipInfo.Firepower
                    && a.WeaponInfo.ArcRestrictions.Contains(ArcType.Rear)
                );
            HostShip.ArcsInfo.Arcs.RemoveAll(a => a is ArcRear);
            HostShip.SetShipInsertImage();
        }
    }
}