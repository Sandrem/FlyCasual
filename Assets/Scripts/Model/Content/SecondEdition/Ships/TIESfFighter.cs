using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;
using Ship;
using Tokens;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class TIESfFighter : FirstEdition.TIESfFighter.TIESfFighter, TIE
        {
            public TIESfFighter() : base()
            {
                ShipInfo.ShipName = "TIE/sf Fighter";

                ShipInfo.ArcInfo = new ShipArcsInfo(
                    new ShipArcInfo(ArcType.Front, 2),
                    new ShipArcInfo(ArcType.SingleTurret, 2)
                );

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

                IconicPilots[Faction.FirstOrder] = typeof(Backdraft);

                ShipAbilities.Add(new Abilities.SecondEdition.HeavyWeaponTurret());

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/44/Maneuver_tie_phantom.png";
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