using Arcs;
using Ship;
using System;
using Tokens;

namespace Abilities
{
    public class AskAquireLockAction : AbilityPart
    {
        private TriggeredAbility Ability;

        public Func<GenericShip> GetTargetShip { get; }
        public Func<string> GetMessage { get; }
        public AbilityPart Action { get; }

        public AskAquireLockAction(Func<GenericShip> targetShip, Func<string> showMessage, AbilityPart action)
        {
            GetTargetShip = targetShip;
            GetMessage = showMessage;
            Action = action;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;

            Messages.ShowInfo(GetMessage());
            ActionsHolder.AcquireTargetLock
            (
                Ability.HostShip,
                GetTargetShip(),
                delegate { Action.DoAction(Ability); },
                delegate { Action.DoAction(Ability); }
            );
        }
    }
}
