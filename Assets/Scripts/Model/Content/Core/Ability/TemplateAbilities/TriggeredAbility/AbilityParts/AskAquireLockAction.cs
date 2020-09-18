using Abilities.Parameters;
using Arcs;
using Ship;
using SubPhases;
using System;
using Tokens;

namespace Abilities
{
    public class AskAquireLockAction : AbilityPart
    {
        private TriggeredAbility Ability;

        public AbilityDescription Description { get; }
        public Func<GenericShip> GetTargetShip { get; }
        public Func<string> GetMessage { get; }
        public AbilityPart Action { get; }

        public AskAquireLockAction(AbilityDescription description, Func<GenericShip> targetShip, Func<string> showMessage, AbilityPart action)
        {
            Description = description;
            GetTargetShip = targetShip;
            GetMessage = showMessage;
            Action = action;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;

            Ability.AskToUseAbility
            (
                Description.Name,
                Ability.AlwaysUseByDefault,
                AcquireTargetLock,
                descriptionLong: Description.Description,
                imageHolder: Description.ImageSource,
                dontUseAbility: GoNext
            );
        }

        private void AcquireTargetLock(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo(GetMessage());
            ActionsHolder.AcquireTargetLock
            (
                Ability.HostShip,
                GetTargetShip(),
                delegate { Action.DoAction(Ability); },
                delegate { Action.DoAction(Ability); }
            );
        }

        private void GoNext(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Action.DoAction(Ability);
        }
    }
}
