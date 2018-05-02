using ActionsList;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class CloakToken : GenericToken
    {
        public CloakToken(GenericShip host) : base(host)
        {
            Name = "Cloak Token";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/CloakAction.png";
        }

        public override void WhenAssigned()
        {
            GenericAction cloakAction = null;
            foreach (var action in Host.PrintedActions)
            {
                if (action.GetType() == typeof(CloakAction))
                {
                    cloakAction = action;
                    break;
                }
            }
            Host.PrintedActions.Remove(cloakAction);

            Host.ChangeAgilityBy(+2);
            Host.OnTryPerformAttack += CannotAttackWhileCloaked;
            Host.OnActivationPhaseStart += RegisterAskDecloak;

            Host.ToggleIonized(true);
            Host.ToggleCloaked(true);
        }

        private void RegisterAskDecloak(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Decloak",
                TriggerType = TriggerTypes.OnActionSubPhaseStart,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = delegate
                {
                    Selection.ThisShip = ship;
                    AskDecloak();
                }
            });
        }

        private void AskDecloak()
        {
            Phases.StartTemporarySubPhaseOld(
                "Decloak Decision",
                typeof(DecloakDecisionSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(DecloakDecisionSubPhase));
                    Triggers.FinishTrigger();
                }
            );
        }

        private void CannotAttackWhileCloaked(ref bool result, List<string> stringList)
        {
            stringList.Add("Cannot attack while Cloaked");
            result = false;
        }

        public override void WhenRemoved()
        {
            Host.PrintedActions.Add(new CloakAction());
            Host.ChangeAgilityBy(-2);
            Host.OnTryPerformAttack -= CannotAttackWhileCloaked;
            Host.OnActivationPhaseStart -= RegisterAskDecloak;

            Host.ToggleIonized(false);
            Host.ToggleCloaked(false);
        }

    }

}
