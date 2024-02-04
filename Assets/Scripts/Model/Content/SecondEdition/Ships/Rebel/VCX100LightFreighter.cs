using System;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Ship.CardInfo;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.VCX100LightFreighter
{
    public class VCX100LightFreighter : GenericShip
    {
        public VCX100LightFreighter() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "VCX-100 Light Freighter",
                BaseSize.Large,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.Rebel, typeof(HeraSyndulla) }
                    }
                ),
                new ShipArcsInfo(ArcType.Front, 4),
                0, 10, 4,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(ReinforceAction))
                ),
                new ShipUpgradesInfo()
            );

            ShipAbilities.Add(new Abilities.SecondEdition.TailGunnerAbility());

            ModelInfo = new ShipModelInfo
            (
                "VCX-100",
                "VCX-100",
                new Vector3(-3.7f, 9.17f, 5.55f),
                4f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "Falcon-Fly1",
                    "Falcon-Fly2",
                    "Falcon-Fly3"
                },
                "Falcon-Fire", 4
            );

            ShipIconLetter = 'G';
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