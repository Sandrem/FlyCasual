using System;
using Abilities.Parameters;

namespace Abilities
{
    public class AskToPerformAction : AbilityPart
    {
        private TriggeredAbility Ability;
        private readonly ActionInfo ActionInfo;

        public Type ActionType { get; }
        public AbilityPart AfterAction { get; }

        public AskToPerformAction(ActionInfo actionInfo, AbilityPart afterAction = null
        )
        {
            ActionInfo = actionInfo;
            AfterAction = afterAction;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;

            Ability.HostShip.AskPerformFreeAction
            (
                ActionInfo.GenerateAction(),
                delegate { AfterAction.DoAction(ability); },
                "Description short",
                "Description long",
                (IImageHolder) Ability.HostReal,
                false
            );
        }
    }
}
