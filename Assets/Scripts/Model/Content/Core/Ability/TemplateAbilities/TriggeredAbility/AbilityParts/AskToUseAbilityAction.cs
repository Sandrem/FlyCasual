using System;
using Abilities.Parameters;
using SubPhases;

namespace Abilities
{
    public class AskToUseAbilityAction : AbilityPart
    {
        private GenericAbility Ability;
        private readonly AbilityDescription Description;

        public AbilityPart OnYes { get; }
        public AbilityPart OnNo { get; }
        public ConditionsBlock Conditions { get; }
        public Func<bool> AiUseByDefault { get; }

        public AskToUseAbilityAction
        (
            AbilityDescription description, 
            AbilityPart onYes,
            AbilityPart onNo = null,
            ConditionsBlock conditions = null,
            Func<bool> aiUseByDefault = null
        )
        {
            Description = description;
            OnYes = onYes;
            OnNo = onNo;
            Conditions = conditions;
            AiUseByDefault = aiUseByDefault ?? delegate { return false; };
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;
            ConditionArgs args = new ConditionArgs() { ShipAbilityHost = Ability.HostShip, ShipToCheck = Ability.HostShip };

            if (Conditions != null && Conditions.Passed(args))
            {
                Ability.AskToUseAbility
                (
                    Description.Name,
                    AiUseByDefault,
                    DoOnYes,
                    DoOnNo,
                    descriptionLong: Description.Description,
                    imageHolder: Description.ImageSource,
                    requiredPlayer: Ability.HostShip.Owner.PlayerNo
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void DoOnYes(object sender, EventArgs e)
        {
            if (OnYes != null)
            {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                OnYes.DoAction(Ability);
            }
            else
            {
                DecisionSubPhase.ConfirmDecision();
            }
        }

        private void DoOnNo(object sender, EventArgs e)
        {
            if (OnNo != null)
            {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                OnNo.DoAction(Ability);
            }
            else
            {
                DecisionSubPhase.ConfirmDecision();
            }
        }
    }
}
