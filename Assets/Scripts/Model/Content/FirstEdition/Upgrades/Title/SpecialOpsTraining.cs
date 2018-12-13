using Ship;
using Upgrade;
using System.Collections.Generic;
using SubPhases;
using Arcs;

namespace UpgradesList.FirstEdition
{
    public class SpecialOpsTraining : GenericUpgrade
    {
        public SpecialOpsTraining() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Special Ops Training",
                UpgradeType.Title,
                cost: 0,          
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIESfFighter.TIESfFighter)),
                abilityType: typeof(Abilities.FirstEdition.SpecialOpsTrainingAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class SpecialOpsTrainingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
        }

        private void CheckConditions()
        {
            if (IsAbilityUsed) return;
            if (Combat.ChosenWeapon.GetType() != typeof(PrimaryWeaponClass)) return;
            if (!Combat.ShotInfo.InPrimaryArc) return;

            RegisterAbilityTrigger(TriggerTypes.OnShotStart, StartQuestionSubphase);
        }

        private class SpecialOpsTrainingDecisionSubPhase : DecisionSubPhase { }

        private void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            if (!IsAbilityUsed)
            {
                SpecialOpsTrainingDecisionSubPhase selectSpecialOpsTrainingSubPhase = (SpecialOpsTrainingDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(SpecialOpsTrainingDecisionSubPhase),
                    Triggers.FinishTrigger
                );

                selectSpecialOpsTrainingSubPhase.InfoText = "Use " + Name + "?";
                selectSpecialOpsTrainingSubPhase.AddDecision("Roll 1 extra die from primary fire arc", RegisterRollExtraDice);
                selectSpecialOpsTrainingSubPhase.AddTooltip("Roll 1 extra die from primary fire arc", HostShip.ImageUrl);
                selectSpecialOpsTrainingSubPhase.AddDecision("Get a second attack from rear arc", RegisterExtraAttack);
                selectSpecialOpsTrainingSubPhase.AddTooltip("Get a second attack from rear arc", HostShip.ImageUrl);
                selectSpecialOpsTrainingSubPhase.DefaultDecisionName = GetDefaultDecision();
                selectSpecialOpsTrainingSubPhase.ShowSkipButton = true;
                selectSpecialOpsTrainingSubPhase.Start();
            }
            else
            {
                Messages.ShowErrorToHuman("Special Ops Training ability has already been used");
                Triggers.FinishTrigger();
            }
        }

        private string GetDefaultDecision()
        {
            string result = "Roll 1 extra die from primary fire arc";
            //TODO : Check if we have targets in both arcs
            return result;
        }

        private void RegisterRollExtraDice(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.OnAttackStartAsAttacker += ClearAbility;

            HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            DecisionSubPhase.ConfirmDecision();
        }

        private void RollExtraDice(ref int count)
        {
            count++;
            Messages.ShowInfo("TieSF rolls extra die from primary arc");
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }

        private void RegisterExtraAttack(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.OnCombatCheckExtraAttack += RegisterSpecialOpsExtraAttack;
            DecisionSubPhase.ConfirmDecision();
        }

        private void RegisterSpecialOpsExtraAttack(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSpecialOpsExtraAttack;
            this.ToggleFrontArc(false);
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoSpecialOpsExtraAttack);
        }

        private void DoSpecialOpsExtraAttack(object sender, System.EventArgs e)
        {

            Combat.StartAdditionalAttack(
                HostShip,
                delegate {
                    Selection.ThisShip.IsAttackPerformed = true;
                    ToggleFrontArc(true);
                    HostShip.OnAttackStartAsAttacker += ClearAbility;
                    Triggers.FinishTrigger();
                },
                null,
                HostUpgrade.UpgradeInfo.Name,
                "You may perform an additional attack from your auxiliary firing arc.",
                HostUpgrade
            );
        }

        //Arc toggle
        private void ToggleFrontArc(bool isActive)
        {
            // TODOREVERT
            //HostShip.ArcsInfo.GetArc<ArcPrimary>().ShotPermissions.CanShootPrimaryWeapon = isActive;
        }

        // IsAbilityUsed to avoid asking question during second attack
        private void ClearAbility()
        {
            HostShip.OnAttackStartAsAttacker -= ClearAbility;

            IsAbilityUsed = false;
        }

    }
}