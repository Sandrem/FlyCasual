using Ship;
using Ship.TIESF;
using Upgrade;

namespace UpgradesList
{
    public class SpecialOpsTraining : GenericUpgrade
    {
        public SpecialOpsTraining() : base()
        {
            Type = UpgradeType.Title;
            Name = "Special Ops Training";
            Cost = 0;            
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIESF;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.ChangeFirepowerBy(1);
        }

        //Spegne un arco
        private void ToggleRearArc(bool isActive)
        {
            Host.ArcInfo.GetRearArc().ShotPermissions.CanShootPrimaryWeapon = isActive;
        }

        //Controlla l'arco
        private void CheckAssignStress()
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(HostShip, TargetShip);
            if (shotInfo.InMobileArc && shotInfo.Range >= 1 && shotInfo.Range <= 2)
            {
                Messages.ShowError(HostShip.PilotName + " assigns Stress Token\nto " + TargetShip.PilotName);
                TargetShip.AssignToken(new Tokens.StressToken(), SelectShipSubPhase.FinishSelection);
            }
            else
            {
                if (!shotInfo.InMobileArc) Messages.ShowError("Target is not inside Mobile Arc");
                else if (shotInfo.Distance >= 3) Messages.ShowError("Target is outside range 2");
            }
        }
    }
}

//Miranda doni
namespace Abilities
{
    public class MirandaDoniAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
            Phases.OnRoundEnd += ClearAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
            Phases.OnRoundEnd -= ClearAbility;
        }

        private void CheckConditions()
        {
            if (!IsAbilityUsed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotStart, StartQuestionSubphase);
            }
        }

        private void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            MirandaDoniDecisionSubPhase selectMirandaDoniSubPhase = (MirandaDoniDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(MirandaDoniDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectMirandaDoniSubPhase.InfoText = "Use " + Name + "?";

            if (HostShip.Shields > 0)
            {
                selectMirandaDoniSubPhase.AddDecision("Spend 1 shield to roll 1 extra die", RegisterRollExtraDice);
                selectMirandaDoniSubPhase.AddTooltip("Spend 1 shield to roll 1 extra die", HostShip.ImageUrl);
            }

            if (HostShip.Shields < HostShip.MaxShields)
            {
                selectMirandaDoniSubPhase.AddDecision("Roll 1 fewer die to recover 1 shield", RegisterRegeneration);
                selectMirandaDoniSubPhase.AddTooltip("Roll 1 fewer die to recover 1 shield", HostShip.ImageUrl);
            }

            selectMirandaDoniSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

            selectMirandaDoniSubPhase.DefaultDecision = GetDefaultDecision();

            selectMirandaDoniSubPhase.ShowSkipButton = true;

            selectMirandaDoniSubPhase.Start();
        }

        private string GetDefaultDecision()
        {
            string result = "No";

            if (HostShip.Shields < HostShip.MaxShields) result = "Roll 1 fewer die to recover 1 shield";

            return result;
        }

        private void RegisterRollExtraDice(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.AfterGotNumberOfAttackDice += RollExtraDice;

            DecisionSubPhase.ConfirmDecision();
        }

        private void RollExtraDice(ref int count)
        {
            count++;
            HostShip.LoseShield();

            Messages.ShowInfo("Miranda Doni spends shield to roll extra die");

            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }

        private void RegisterRegeneration(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.AfterGotNumberOfAttackDice += RegenerateShield;

            DecisionSubPhase.ConfirmDecision();
        }

        private void RegenerateShield(ref int count)
        {
            count--;
            HostShip.TryRegenShields();

            Messages.ShowInfo("Miranda Doni rolls 1 fewer die to recover 1 shield");

            HostShip.AfterGotNumberOfAttackDice -= RegenerateShield;
        }

        private void ClearAbility()
        {
            IsAbilityUsed = false;
        }

        private class MirandaDoniDecisionSubPhase : DecisionSubPhase { }
    }
}
