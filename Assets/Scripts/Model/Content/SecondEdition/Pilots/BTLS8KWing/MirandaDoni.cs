using SubPhases;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.BTLS8KWing
    {
        public class MirandaDoni : BTLS8KWing
        {
            public MirandaDoni() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Miranda Doni",
                    4,
                    48,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.MirandaDoniAbility)
                );

                SEImageNumber = 62;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class MirandaDoniAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
            Phases.Events.OnRoundEnd += ClearAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
            Phases.Events.OnRoundEnd -= ClearAbility;
        }

        private void CheckConditions()
        {
            if (!IsAbilityUsed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotStart, StartQuestionSubphase);
            }
        }

        protected virtual void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            MirandaDoniDecisionSubPhase selectMirandaDoniSubPhase = (MirandaDoniDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(MirandaDoniDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectMirandaDoniSubPhase.InfoText = "Use " + Name + "?";

            if (HostShip.State.ShieldsCurrent > 0)
            {
                selectMirandaDoniSubPhase.AddDecision("Spend 1 shield to roll 1 extra die", RegisterRollExtraDice);
                selectMirandaDoniSubPhase.AddTooltip("Spend 1 shield to roll 1 extra die", HostShip.ImageUrl);
            }

            if (HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax)
            {
                selectMirandaDoniSubPhase.AddDecision("Roll 1 fewer die to recover 1 shield", RegisterRegeneration);
                selectMirandaDoniSubPhase.AddTooltip("Roll 1 fewer die to recover 1 shield", HostShip.ImageUrl);
            }

            selectMirandaDoniSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

            selectMirandaDoniSubPhase.DefaultDecisionName = GetDefaultDecision();

            selectMirandaDoniSubPhase.ShowSkipButton = true;

            selectMirandaDoniSubPhase.Start();
        }

        protected string GetDefaultDecision()
        {
            string result = "No";

            if (HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax) result = "Roll 1 fewer die to recover 1 shield";

            return result;
        }

        protected void RegisterRollExtraDice(object sender, System.EventArgs e)
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

        protected void RegisterRegeneration(object sender, System.EventArgs e)
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

        protected class MirandaDoniDecisionSubPhase : DecisionSubPhase { }
    }
}

namespace Abilities.SecondEdition
{
    public class MirandaDoniAbility : Abilities.FirstEdition.MirandaDoniAbility
    {
        protected override void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            MirandaDoniDecisionSubPhase selectMirandaDoniSubPhase = (MirandaDoniDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(MirandaDoniDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectMirandaDoniSubPhase.InfoText = "Use " + Name + "?";

            if (HostShip.State.ShieldsCurrent > 0)
            {
                selectMirandaDoniSubPhase.AddDecision("Spend 1 shield to roll 1 extra die", RegisterRollExtraDice);
                selectMirandaDoniSubPhase.AddTooltip("Spend 1 shield to roll 1 extra die", HostShip.ImageUrl);
            }

            if (HostShip.State.ShieldsCurrent == 0)
            {
                selectMirandaDoniSubPhase.AddDecision("Roll 1 fewer die to recover 1 shield", RegisterRegeneration);
                selectMirandaDoniSubPhase.AddTooltip("Roll 1 fewer die to recover 1 shield", HostShip.ImageUrl);
            }

            selectMirandaDoniSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

            selectMirandaDoniSubPhase.DefaultDecisionName = GetDefaultDecision();

            selectMirandaDoniSubPhase.ShowSkipButton = true;

            selectMirandaDoniSubPhase.Start();
        }
    }
}
