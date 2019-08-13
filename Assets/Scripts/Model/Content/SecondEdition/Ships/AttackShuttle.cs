using ActionsList;
using Actions;
using Arcs;
using Ship;

namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class AttackShuttle : FirstEdition.AttackShuttle.AttackShuttle
        {
            public AttackShuttle() : base()
            {
                ShipInfo.Hull = 3;
                ShipInfo.Shields = 1;

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(EvadeAction)));

                IconicPilots[Faction.Rebel] = typeof(HeraSyndulla);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/46/Maneuver_attack_shuttle.png";

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
            if (Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.PrimaryWeapon
                && (Combat.ArcForShot.ArcType == ArcType.Front || Combat.ArcForShot.ArcType == ArcType.SingleTurret)
            )
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
