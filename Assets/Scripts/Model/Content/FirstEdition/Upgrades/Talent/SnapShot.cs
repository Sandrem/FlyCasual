using ActionsList;
using BoardTools;
using Ship;
using System;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class SnapShot : GenericSpecialWeapon
    {
        public SnapShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Snap Shot",
                UpgradeType.Talent,
                cost: 2,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    // Hacking the range to remove this as a possible weapon when ability is not triggered
                    minRange: -1,
                    maxRange: -1
                ),
                abilityType: typeof(Abilities.FirstEdition.SnapShotAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class SnapShotAbility : GenericAbility
    {

        private GenericShip snapShotTarget = null;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddSnapShotRestriction;
            GenericShip.OnMovementFinishGlobal += CheckSnapShotAbility;
            Phases.Events.OnActivationPhaseEnd_Triggers += CleanUpSnapShotAbility;
            Phases.Events.OnCombatPhaseEnd_Triggers += CleanUpSnapShotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddSnapShotRestriction;
            GenericShip.OnMovementFinishGlobal -= CheckSnapShotAbility;
            Phases.Events.OnActivationPhaseEnd_Triggers -= CleanUpSnapShotAbility;
            Phases.Events.OnCombatPhaseEnd_Triggers -= CleanUpSnapShotAbility;
        }

        private void AddSnapShotRestriction()
        {
            if (Combat.Attacker.ShipId == HostShip.ShipId && Combat.ChosenWeapon.GetType() == HostUpgrade.GetType())
            {
                Combat.Attacker.OnTryAddAvailableDiceModification += SnapShotRestrictionForAttacker;
                Combat.Attacker.OnAttackFinish += RemoveSnapShotRestrictionForAttacker;

                Combat.Defender.OnTryAddAvailableDiceModification += SnapShotRestrictionForDefender;
                Combat.Defender.OnAttackFinish += RemoveSnapShotRestrictionForDefender;
            }
        }

        protected virtual void SnapShotRestrictionForDefender(GenericShip ship, GenericAction action, ref bool data)
        {
            // Do nothing
        }

        protected virtual void SnapShotRestrictionForAttacker(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action.DiceModificationTiming != DiceModificationTimingType.Opposite)
            {
                Messages.ShowErrorToHuman("Snap Shot: You cannot modify your attack dice");
                canBeUsed = false;
            }
        }

        private void RemoveSnapShotRestrictionForDefender(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= SnapShotRestrictionForDefender;
            ship.OnAttackFinish -= RemoveSnapShotRestrictionForDefender;
        }

        private void RemoveSnapShotRestrictionForAttacker(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= SnapShotRestrictionForAttacker;
            ship.OnAttackFinish -= RemoveSnapShotRestrictionForAttacker;
        }

        private void CleanUpSnapShotAbility()
        {
            ClearIsAbilityUsedFlag();
            snapShotTarget = null;
            HostShip.IsAttackPerformed = false;
            HostShip.IsAttackSkipped = false;
            HostShip.IsCannotAttackSecondTime = false;

            DisableWeaponRange();
        }

        public void AfterSnapShotAttackSubPhase()
        {
            HostShip.IsAttackPerformed = true;
            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;
            HostShip.OnAttackFinishAsAttacker -= SetIsAbilityIsUsed;
            Selection.ChangeActiveShip(snapShotTarget);
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Triggers.FinishTrigger();
        }

        protected virtual void EnableWeaponRange()
        {
            (HostUpgrade as IShipWeapon).WeaponInfo.MaxRange = 1;
            (HostUpgrade as IShipWeapon).WeaponInfo.MinRange = 1;
        }

        protected virtual void DisableWeaponRange()
        {
            (HostUpgrade as IShipWeapon).WeaponInfo.MaxRange = -1;
            (HostUpgrade as IShipWeapon).WeaponInfo.MinRange = -1;
        }

        private void CheckSnapShotAbility(GenericShip ship)
        {
            if (!IsAbilityUsed && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                snapShotTarget = ship;

                EnableWeaponRange();
                ShotInfo shotInfo = new ShotInfo(HostShip, snapShotTarget, (HostUpgrade as IShipWeapon));

                if (shotInfo.Range <= (HostUpgrade as IShipWeapon).WeaponInfo.MaxRange &&
                    shotInfo.Range >= (HostUpgrade as IShipWeapon).WeaponInfo.MinRange &&
                    shotInfo.IsShotAvailable)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskSnapShotAbility);
                }
                else
                {
                    DisableWeaponRange();
                }
            }
        }

        private void AskSnapShotAbility(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                PerformSnapShot,
                dontUseAbility: CancelSnapShot,
                callback: delegate {
                    Selection.ChangeActiveShip(snapShotTarget);
                    Triggers.FinishTrigger();
                },
                descriptionLong: "Do you want to perform \"Snap Shot\" attack against " + snapShotTarget.PilotInfo.PilotName + "?",
                imageHolder: HostUpgrade
            );
        }

        private void CancelSnapShot(object sender, EventArgs e)
        {
            DisableWeaponRange();
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private bool SnapShotAttackFilter(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;
            if (defender != snapShotTarget)
            {
                if (!isSilent) Messages.ShowErrorToHuman(
                    string.Format("Snap Shot's target must be {0}", snapShotTarget.PilotInfo.PilotName));
                result = false;
            }
            else if (!(weapon.GetType() == HostUpgrade.GetType()))
            {
                if (!isSilent) Messages.ShowErrorToHuman(
                                    string.Format("This attack must be Snap Shot attack"));
                result = false;
            }

            return result;
        }

        public void SnapShotSetUsed()
        {
            IsAbilityUsed = true;
        }

        private void PerformSnapShot(object sender, EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.OnAttackFinishAsAttacker += SetIsAbilityIsUsed;
                HostShip.IsCannotAttackSecondTime = true;
                Combat.StartSelectAttackTarget(
                    HostShip,
                    AfterSnapShotAttackSubPhase,
                    SnapShotAttackFilter,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may perform a bonus Snap Shot attack against " + snapShotTarget.PilotInfo.PilotName,
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

    }
}