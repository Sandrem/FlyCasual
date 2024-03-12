using ActionsList;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Foresight : GenericSpecialWeapon
    {
        public Foresight() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Foresight",
                UpgradeType.ForcePower,
                cost: 6,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    arc: Arcs.ArcType.Bullseye,
                    noRangeBonus: true,
                    minRange: 1,
                    maxRange: 3
                ),
                abilityType: typeof(Abilities.SecondEdition.ForesightAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/7b/b1/7bb148c4-74d6-4caf-a427-0270d40488b8/swz48_cards-foresight.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ForesightAbility : GenericAbility
    {

        private GenericShip foresightTarget = null;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddForesightRestriction;
            GenericShip.OnMovementFinishGlobal += CheckForesightAbility;
            Phases.Events.OnActivationPhaseEnd_Triggers += CleanUpForesightAbility;
            Phases.Events.OnCombatPhaseEnd_Triggers += CleanUpForesightAbility;

            AddDiceModification(
                "Foresight",
                CheckIsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>(){ DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddForesightRestriction;
            GenericShip.OnMovementFinishGlobal -= CheckForesightAbility;
            Phases.Events.OnActivationPhaseEnd_Triggers -= CleanUpForesightAbility;
            Phases.Events.OnCombatPhaseEnd_Triggers -= CleanUpForesightAbility;

            RemoveDiceModification();
        }

        private int GetAiPriority()
        {
            return 55;
        }

        private bool CheckIsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ChosenWeapon == HostUpgrade
                && Combat.Attacker.ShipId == HostShip.ShipId
                && Combat.DiceRollAttack.Focuses > 0;
        }

        private void AddForesightRestriction()
        {
            if (Combat.Attacker.ShipId == HostShip.ShipId && Combat.ChosenWeapon.GetType() == HostUpgrade.GetType())
            {
                Combat.Attacker.OnTryAddAvailableDiceModification += ForesightRestrictionForAttacker;
                Combat.Attacker.OnAttackFinish += RemoveFiresightRestrictionForAttacker;

                Combat.Defender.OnTryAddAvailableDiceModification += ForesightRestrictionForDefender;
                Combat.Defender.OnAttackFinish += RemoveForesightRestrictionForDefender;
            }
        }

        protected virtual void ForesightRestrictionForDefender(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action.DiceModificationTiming == DiceModificationTimingType.Opposite)
            {
                Messages.ShowErrorToHuman("Foresight: You cannot modify attack dice");
                canBeUsed = false;
            }
        }

        protected virtual void ForesightRestrictionForAttacker(GenericShip ship, GenericAction diceModification, ref bool canBeUsed)
        {
            if (diceModification.DiceModificationTiming != DiceModificationTimingType.Opposite
                && diceModification.DiceModificationName != "Foresight"
                && !diceModification.IsNotRealDiceModification)
            {
                Messages.ShowErrorToHuman("Foresight: You cannot modify your attack dice in another ways");
                canBeUsed = false;
            }
        }

        private void RemoveForesightRestrictionForDefender(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= ForesightRestrictionForDefender;
            ship.OnAttackFinish -= RemoveForesightRestrictionForDefender;
        }

        private void RemoveFiresightRestrictionForAttacker(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= ForesightRestrictionForAttacker;
            ship.OnAttackFinish -= RemoveFiresightRestrictionForAttacker;
        }

        private void CleanUpForesightAbility()
        {
            ClearIsAbilityUsedFlag();
            foresightTarget = null;
            HostShip.IsAttackPerformed = false;
            HostShip.IsAttackSkipped = false;
            HostShip.IsCannotAttackSecondTime = false;

            DisableWeaponRange();
        }

        public void AfterFiresightAttackSubPhase()
        {
            HostShip.IsAttackPerformed = true;
            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;
            HostShip.OnAttackFinishAsAttacker -= SetIsAbilityIsUsed;
            Selection.ChangeActiveShip(foresightTarget);
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Triggers.FinishTrigger();
        }

        protected virtual void EnableWeaponRange()
        {
            // Do nothing
        }

        private void DisableWeaponRange()
        {
            // Do nothing
        }

        private void CheckForesightAbility(GenericShip ship)
        {
            if (!IsAbilityUsed && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                foresightTarget = ship;

                EnableWeaponRange();
                ShotInfo shotInfo = new ShotInfo(HostShip, foresightTarget, (HostUpgrade as IShipWeapon));

                if (shotInfo.Range <= (HostUpgrade as IShipWeapon).WeaponInfo.MaxRange &&
                    shotInfo.Range >= (HostUpgrade as IShipWeapon).WeaponInfo.MinRange &&
                    shotInfo.IsShotAvailable &&
                    HostShip.State.Force > 0 &&
                    HostShip.IsCanUseForceNow())
                {
                    RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskForesightAbility);
                }
                else
                {
                    DisableWeaponRange();
                }
            }
        }

        private void AskForesightAbility(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                PerformForesightShot,
                dontUseAbility: CancelForesightShot,
                callback: delegate {
                    Selection.ChangeActiveShip(foresightTarget);
                    Triggers.FinishTrigger();
                },
                descriptionLong: "Do you want to perform \"Foresight\" attack against " + foresightTarget.PilotInfo.PilotName + "?",
                imageHolder: HostUpgrade
            );
        }

        private void CancelForesightShot(object sender, EventArgs e)
        {
            DisableWeaponRange();
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private bool ForesightAttackFilter(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;
            if (defender != foresightTarget)
            {
                if (!isSilent) Messages.ShowErrorToHuman(
                    string.Format("Foresight's target must be {0}", foresightTarget.PilotInfo.PilotName));
                result = false;
            }
            else if (!(weapon.GetType() == HostUpgrade.GetType()))
            {
                if (!isSilent) Messages.ShowErrorToHuman("This attack must be Foresight attack");
                result = false;
            }

            return result;
        }

        public void ForesightSetUsed()
        {
            IsAbilityUsed = true;
        }

        private void PerformForesightShot(object sender, EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.OnAttackFinishAsAttacker += SetIsAbilityIsUsed;
                HostShip.IsCannotAttackSecondTime = true;

                HostShip.State.SpendForce(
                    1,
                    delegate
                    {
                        Combat.StartSelectAttackTarget(
                            HostShip,
                            AfterFiresightAttackSubPhase,
                            ForesightAttackFilter,
                            HostUpgrade.UpgradeInfo.Name,
                            "You may perform a bonus Foresight attack against " + foresightTarget.PilotInfo.PilotName,
                            HostUpgrade
                        );
                    }
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

    }
}