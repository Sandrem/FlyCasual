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

namespace Ship.SecondEdition.TIERbHeavy
{
    public class TIERbHeavy : GenericShip
    {
        public TIERbHeavy() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "TIE/rb Heavy",
                BaseSize.Medium,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.Imperial, typeof(CardiaAcademyPilot) }
                    }
                ),
                new ShipArcsInfo(ArcType.SingleTurret, 2), 1, 8, 0,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(ReinforceAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction), ActionColor.Red),
                    new ActionInfo(typeof(RotateArcAction))
                ),
                new ShipUpgradesInfo(),
                linkedActions: new List<LinkedActionInfo>
                {
                    new LinkedActionInfo(typeof(RotateArcAction), typeof(CalculateAction), ActionColor.Red)
                }
            );

            ModelInfo = new ShipModelInfo
            (
                "TIE Heavy",
                "Default",
                new Vector3(-3.73f, 7.9f, 5.55f),
                2.3f
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

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.TallonRoll, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.TallonRoll, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "TIE-Fly1",
                    "TIE-Fly2",
                    "TIE-Fly3",
                    "TIE-Fly4",
                    "TIE-Fly5",
                    "TIE-Fly6",
                    "TIE-Fly7"
                },
                "TIE-Fire", 2
            );

            ShipIconLetter = 'J';

            ShipAbilities.Add(new Abilities.SecondEdition.RotatingCannons());
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RotatingCannons : GenericAbility
    {
        public override string Name { get { return "Rotating Cannons"; } }

        public override void ActivateAbility()
        {
            HostShip.OnGameStart += RestrictCannonArcRequirements;
            HostShip.OnGetAvailableArcFacings += RestrictArcFacings;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGameStart -= RestrictCannonArcRequirements;
            HostShip.OnGetAvailableArcFacings -= RestrictArcFacings;
        }

        private void RestrictArcFacings(List<ArcFacing> facings)
        {
            facings.Remove(ArcFacing.Left);
            facings.Remove(ArcFacing.Right);
        }

        private void RestrictCannonArcRequirements()
        {
            foreach (GenericUpgrade weaponUpgrade in HostShip.UpgradeBar.GetSpecialWeaponsAll())
            {
                IShipWeapon specialWeapon = weaponUpgrade as IShipWeapon;
                if (specialWeapon.WeaponType == WeaponTypes.Cannon)
                {
                    if (specialWeapon.WeaponInfo.ArcRestrictions.Contains(ArcType.Front))
                    {
                        specialWeapon.WeaponInfo.ArcRestrictions.Remove(ArcType.Front);
                        specialWeapon.WeaponInfo.ArcRestrictions.Add(ArcType.SingleTurret);
                    }
                }
            }
        }
    }
}