using System;
using Abilities.Parameters;

namespace Abilities
{
    public class AskToPerformAction : AbilityPart
    {
        private TriggeredAbility Ability;
        private readonly AbilityDescription Description;
        private readonly ActionInfo ActionInfo;

        public Type ActionType { get; }
        public AbilityPart AfterAction { get; }

        public AskToPerformAction(AbilityDescription description, ActionInfo actionInfo, AbilityPart afterAction = null
        )
        {
            Description = description;
            ActionInfo = actionInfo;
            AfterAction = afterAction;
        }

        public override void DoAction(TriggeredAbility ability)
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
            if (AfterAction != null)
            {
                AfterAction.DoAction(Ability);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
