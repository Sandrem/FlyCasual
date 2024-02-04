using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;
using Ship;
using Ship.CardInfo;
using System;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class TIESfFighter : GenericShip
        {
            public TIESfFighter() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "TIE/sf Fighter",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, Type>
                        {
                            { Faction.FirstOrder, typeof(Quickdraw) }
                        }
                    ),
                    new ShipArcsInfo
                    (
                        new ShipArcInfo(ArcType.Front, 2),
                        new ShipArcInfo(ArcType.SingleTurret, 2)
                    ),
                    2, 3, 3,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(BarrelRollAction)),
                        new ActionInfo(typeof(EvadeAction))
                    ),
                    new ShipUpgradesInfo(),
                    linkedActions: new List<LinkedActionInfo>
                    {
                        new LinkedActionInfo(typeof(FocusAction), typeof(RotateArcAction), ActionColor.White),
                        new LinkedActionInfo(typeof(TargetLockAction), typeof(RotateArcAction), ActionColor.White),
                        new LinkedActionInfo(typeof(EvadeAction), typeof(RotateArcAction), ActionColor.White),
                        new LinkedActionInfo(typeof(BarrelRollAction), typeof(RotateArcAction), ActionColor.White)
                    }
                );

                ShipAbilities.Add(new Abilities.SecondEdition.HeavyWeaponTurret());

                ModelInfo = new ShipModelInfo
                (
                    "TIE/SF Fighter",
                    "First Order",
                    previewScale: 2f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
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

                ShipIconLetter = 'S';
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an action, you may perform a red boost action.
    public class HeavyWeaponTurret : GenericAbility
    {
        public override string Name { get { return "Heavy Weapon Turret"; } }

        public override void ActivateAbility()
        {
            HostShip.OnGameStart += RestrictMissileArcRequirements;
            HostShip.OnGetAvailableArcFacings += RestrictArcFacings;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGameStart -= RestrictMissileArcRequirements;
            HostShip.OnGetAvailableArcFacings -= RestrictArcFacings;
        }

        private void RestrictArcFacings(List<ArcFacing> facings)
        {
            facings.Remove(ArcFacing.Left);
            facings.Remove(ArcFacing.Right);
        }

        private void RestrictMissileArcRequirements()
        {
            foreach (GenericUpgrade weaponUpgrade in HostShip.UpgradeBar.GetSpecialWeaponsAll())
            {
                IShipWeapon specialWeapon = weaponUpgrade as IShipWeapon;
                if (specialWeapon.WeaponType == WeaponTypes.Missile) {
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