using Upgrade;
using System.Collections.Generic;
using System.Linq;
using Actions;
using ActionsList;
using Tokens;
using Ship;
using System;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class SnapShot : GenericSpecialWeapon
    {
        public SnapShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "SnapShot",
                UpgradeType.Talent,
                cost: 9,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 2,
                    maxRange: 2,
                    noRangeBonus: true
                ),
                abilityType: typeof(Abilities.SecondEdition.SnapShotAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/0c/6b/0c6b9e6c-7c2f-4322-bcf0-f6f2fce44323/swz47_upgrade-snap-shot.png";
        }
    }
}

namespace Abilities.SecondEdition
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

        protected virtual void SnapShotRestrictionForDefender(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action.DiceModificationTiming == DiceModificationTimingType.Opposite)
            {
                Messages.ShowErrorToHuman("Snap Shot: You cannot modify attacker's attack dice");
                canBeUsed = false;
            }
        }

        protected virtual void SnapShotRestrictionForAttacker(GenericShip ship, GenericAction diceModification, ref bool canBeUsed)
        {
            if (diceModification.DiceModificationTiming != DiceModificationTimingType.Opposite && !diceModification.IsNotRealDiceModification)
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
            // Do nothing
        }

        protected virtual void DisableWeaponRange()
        {
            // Do nothing
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