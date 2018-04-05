using ActionsList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;

namespace UpgradesList
{
	public class PushTheLimit : GenericUpgrade
	{
        private bool IsUsed = false;
	    private int consecutiveActions = 0;

		public PushTheLimit() : base()
		{
            Types.Add(UpgradeType.Elite);
			Name = "Push The Limit";
			Cost = 3;
		}

		public override void AttachToShip(GenericShip host)
		{
			base.AttachToShip(host);

            host.OnActionIsPerformed += CheckConditions;

            Phases.OnEndPhaseStart_NoTriggers += Cleanup;
            Host.OnShipIsDestroyed += StopAbility;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action == null)
            {
                consecutiveActions = 0;
            }
            else if (consecutiveActions < 1 && !IsUsed)
            {
                consecutiveActions++;
                Host.OnActionDecisionSubphaseEnd += DoSecondAction;
            }
        }

		private void DoSecondAction(GenericShip ship)
		{
            Host.OnActionDecisionSubphaseEnd -= DoSecondAction;

            if (!ship.Tokens.HasToken(typeof(Tokens.StressToken)) || ship.CanPerformActionsWhileStressed)
			{
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
		}

		private void PerformPushAction(object sender, System.EventArgs e)
		{
			base.Host.GenerateAvailableActionsList();
			List<GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();
			base.Host.AskPerformFreeAction(actions, AddStressToken);
		}

        private void Cleanup()
        {
            IsUsed = false;
            consecutiveActions = 0;
        }

		private void AddStressToken()
		{
			if (!base.Host.IsFreeActionSkipped)
			{
			    IsUsed = true;
				base.Host.Tokens.AssignToken (
                    new Tokens.StressToken(base.Host),
					Triggers.FinishTrigger
                );
			}
			else
			{
				Triggers.FinishTrigger();
			}
		}

        private void StopAbility(GenericShip host, bool isFled)
        {
            Phases.OnEndPhaseStart_NoTriggers -= Cleanup;
        }
    }
}