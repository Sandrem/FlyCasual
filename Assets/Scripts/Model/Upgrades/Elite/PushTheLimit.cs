using ActionsList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using Abilities;
using Tokens;
using System.Linq;

namespace UpgradesList
{
    public class PushTheLimit : GenericUpgrade
    {
        public PushTheLimit() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Push The Limit";
            Cost = 3;

            UpgradeAbilities.Add(new PushTheLimitAbility());
        }
    }
}

namespace Abilities
{
    public class PushTheLimitAbility : GenericAbility
    {
        private int consecutiveActions = 0;

        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
            Phases.Events.OnEndPhaseStart_NoTriggers += Cleanup;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
            Phases.Events.OnEndPhaseStart_NoTriggers -= Cleanup;
        }

        private void Cleanup()
        {
            IsAbilityUsed = false;
            consecutiveActions = 0;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action == null)
            {
                consecutiveActions = 0;
            }
            else if (consecutiveActions < 1 && !IsAbilityUsed)
            {
                consecutiveActions++;
                HostShip.OnActionDecisionSubphaseEnd += DoSecondAction;
            }
        }

        private void DoSecondAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= DoSecondAction;

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Push The Limit Action",
                    TriggerOwner = ship.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeAction,
                    EventHandler = PerformPushAction
                }
            );
        }

        private void PerformPushAction(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = HostShip.GetAvailableActions();
            List<GenericAction> actionBarActions = actions.Where(n => n.IsInActionBar).ToList();
            HostShip.AskPerformFreeAction(actionBarActions, AddStressToken);
        }

        private void AddStressToken()
        {
            if (!base.HostShip.IsFreeActionSkipped)
            {
                IsAbilityUsed = true;
                base.HostShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
            // Reset after every Push the Limit opportunity
            consecutiveActions = 0;
        }
    }
}
