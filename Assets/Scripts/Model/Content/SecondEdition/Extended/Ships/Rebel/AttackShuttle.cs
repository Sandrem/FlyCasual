using ActionsList;
using Actions;
using Arcs;
using Ship;
using System.Collections.Generic;
using Movement;
using Ship.CardInfo;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class AttackShuttle : GenericShip
        {
            public AttackShuttle() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "Attack Shuttle",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Rebel, typeof(HeraSyndulla) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 3), 2, 3, 1,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(BarrelRollAction)),
                        new ActionInfo(typeof(EvadeAction))
                    ),
                    new ShipUpgradesInfo(),
                    linkedActions: new List<LinkedActionInfo>
                    {
                        new LinkedActionInfo(typeof(BarrelRollAction), typeof(EvadeAction))
                    },
                    legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal }
                );

                ModelInfo = new ShipModelInfo
                (
                    "Attack Shuttle",
                    "Attack Shuttle",
                    previewScale: 1.18f,
                    wingsPositions: WingsPositions.Opened
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
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
                        "XWing-Fly1",
                        "XWing-Fly2",
                        "XWing-Fly3"
                    },
                    "XWing-Laser", 3
                );

                ShipIconLetter = 'g';

                ShipAbilities.Add(new Abilities.SecondEdition.LockedAndLoadedability());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LockedAndLoadedability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDocked += ActivateDockedAbility;
            HostShip.OnUndocked += DeactivateDockedAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDocked -= ActivateDockedAbility;
            HostShip.OnUndocked -= DeactivateDockedAbility;
        }

        private void ActivateDockedAbility(GenericShip hostShip)
        {
            HostShip.DockingHost.OnAttackFinishAsAttacker += CheckAbility;
        }

        private void DeactivateDockedAbility(GenericShip hostShip)
        {
            HostShip.DockingHost.OnAttackFinishAsAttacker -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if ((Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.PrimaryWeapon && Combat.ArcForShot.ArcType == ArcType.Front) 
                || Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.Turret)
            {
                HostShip.DockingHost.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
            }
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.DockingHost.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseGunnerAbility);
        }

        private void UseGunnerAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.DockingHost.IsCannotAttackSecondTime)
            {
                HostShip.DockingHost.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip.DockingHost,
                    FinishAdditionalAttack,
                    IsRearArcShot,
                    "Locked and Loaded",
                    "You may perform a bonus primary rear firing arc attack",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot make additional attacks", HostShip.DockingHost.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            HostShip.DockingHost.IsAttackPerformed = true;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.DockingHost.IsAttackSkipped) HostShip.DockingHost.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        private bool IsRearArcShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            if (weapon.WeaponType == WeaponTypes.PrimaryWeapon && weapon.WeaponInfo.ArcRestrictions.Contains(ArcType.Rear))
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("This attack must use the ship's rear firing arc");
            }

            return result;
        }
    }
}
