using System;
using Abilities.Parameters;

namespace Abilities
{
    public class AskToPerformAction : AbilityPart
    {
        private GenericAbility Ability;
        private readonly AbilityDescription Description;
        private readonly ActionInfo ActionInfo;
        private AbilityPart AfterActionIfSkipped;

        public Type ActionType { get; }
        public AbilityPart AfterAction { get; }

        public AskToPerformAction(
            AbilityDescription description,
            ActionInfo actionInfo,
            AbilityPart afterAction = null,
            AbilityPart afterActionIfSkipped = null
        )
        {
            Description = description;
            ActionInfo = actionInfo;
            AfterAction = afterAction;
            AfterActionIfSkipped = afterActionIfSkipped;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;

            Ability.HostShip.AskPerformFreeAction
            (
                ActionInfo.GenerateAction(),
                Finish,
                Description.Name,
                Description.Description,
                Description.ImageSource,
                false
            );
        }

        private void Finish()
        {
            if (!Ability.HostShip.IsFreeActionSkipped && AfterAction != null)
            {
                AfterAction.DoAction(Ability);
            }
            else if (Ability.HostShip.IsFreeActionSkipped && AfterActionIfSkipped != null)
            {
                AfterActionIfSkipped.DoAction(Ability);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
