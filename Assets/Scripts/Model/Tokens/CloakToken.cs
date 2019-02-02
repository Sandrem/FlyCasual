using ActionsList;
using Editions;
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
            TokenColor = TokenColors.Blue;
            PriorityUI = 50;
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/CloakAction.png";
        }

        public override void WhenAssigned()
        {
            Host.ChangeAgilityBy(+2);
            Host.OnTryPerformAttack += CannotAttackWhileCloaked;
            Host.OnSystemsAbilityActivation += RegisterAskDecloak;

            Host.ToggleIonized(true);
            Host.ToggleCloaked(true);
        }

        private void RegisterAskDecloak(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Decloak",
                TriggerType = TriggerTypes.OnActionSubPhaseStart,
                TriggerOwner = ship.Owner.PlayerNo,
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
            Host.ChangeAgilityBy(-2);
            Host.OnTryPerformAttack -= CannotAttackWhileCloaked;
            Host.OnActivationPhaseStart -= RegisterAskDecloak;
            Host.OnSystemsAbilityActivation -= RegisterAskDecloak;

            Host.ToggleIonized(false);
            Host.ToggleCloaked(false);
        }

    }

}
