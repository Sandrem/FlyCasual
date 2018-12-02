using Upgrade;
using System.Collections.Generic;
using Tokens;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class Opportunist : GenericUpgrade
    {
        public Opportunist() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Opportunist",
                UpgradeType.Talent,
                cost: 4,
                abilityType: typeof(Abilities.FirstEdition.OpportunistAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class OpportunistAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterOpportunistAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterOpportunistAbility;
        }

        public void RegisterOpportunistAbility()
        {
            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Opportunist",
                    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnAttackStart,
                    EventHandler = StartOpportunistDecisionSubPhase
                }
            );
        }

        private void StartOpportunistDecisionSubPhase(object sender, System.EventArgs e)
        {
            //card constraints say user can't have a stress token, and defender can't have focus or evade tokens
            if (!Combat.Attacker.Tokens.HasToken(typeof(StressToken)) && (!Combat.Defender.Tokens.HasToken(typeof(FocusToken)) && !Combat.Defender.Tokens.HasToken(typeof(Tokens.EvadeToken))))
            {
                var opportunistDecision = (OpportunistDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(OpportunistDecisionSubPhase),
                    Triggers.FinishTrigger
                );

                opportunistDecision.InfoText = "Use Opportunist ability?";
                opportunistDecision.AddDecision("Yes", UseOpportunistAbility);
                opportunistDecision.AddDecision("No", DontUseOpportunistAbility);
                opportunistDecision.DefaultDecisionName = "Yes";
                opportunistDecision.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseOpportunistAbility(object sender, System.EventArgs e)
        {
            Combat.Attacker.Tokens.AssignToken(typeof(StressToken), AllowRollAdditionalDice);
        }

        private void AllowRollAdditionalDice()
        {
            Combat.Attacker.AfterGotNumberOfAttackDice += IncreaseByOne;
            DecisionSubPhase.ConfirmDecision();
        }

        private void IncreaseByOne(ref int value)
        {
            value++;
            Combat.Attacker.AfterGotNumberOfAttackDice -= IncreaseByOne;
        }

        private void DontUseOpportunistAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private class OpportunistDecisionSubPhase : DecisionSubPhase
        {
            public override void SkipButton()
            {
                ConfirmDecision();
            }
        }
    }
}