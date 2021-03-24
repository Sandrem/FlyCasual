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
        public Func<bool> AiUseByDefault { get; }

        public AskToUseAbilityAction
        (
            AbilityDescription description, 
            AbilityPart onYes,
            AbilityPart onNo = null,
            Func<bool> aiUseByDefault = null
        )
        {
            Description = description;
            OnYes = onYes;
            OnNo = onNo;
            AiUseByDefault = aiUseByDefault ?? delegate { return false; };
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;

            Ability.AskToUseAbility(
                Description.Name,
                AiUseByDefault,
                DoOnYes,
                DoOnNo,
                descriptionLong: Description.Description,
                imageHolder: Description.ImageSource,
                requiredPlayer: Ability.HostShip.Owner.PlayerNo
            );
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
