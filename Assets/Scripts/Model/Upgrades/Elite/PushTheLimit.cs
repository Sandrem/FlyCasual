using ActionsList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
	public class PushTheLimit : GenericUpgrade
	{
        private bool IsUsed = false;

		public PushTheLimit() : base()
		{
			Type = UpgradeType.Elite;
			Name = "Push The Limit";
			Cost = 3;
		}

		public override void AttachToShip(Ship.GenericShip host)
		{
			base.AttachToShip(host);

            host.OnActionIsPerformed += CheckConditions;

            Phases.OnEndPhaseStart += Cleanup;
        }

        private void CheckConditions(GenericAction action)
        {
            if (!IsUsed)
            {
                Host.OnActionDecisionSubphaseEnd += DoSecondAction;
            }
        }

		private void DoSecondAction(Ship.GenericShip ship)
		{
            Host.OnActionDecisionSubphaseEnd -= DoSecondAction;

            if (!ship.HasToken(typeof(Tokens.StressToken)) || ship.CanPerformActionsWhileStressed)
			{
                IsUsed = true;
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
        }

		private void AddStressToken()
		{
			if (!base.Host.IsFreeActionSkipped) {
				base.Host.AssignToken (
                    new Tokens.StressToken(),
					Triggers.FinishTrigger
                );	
			}
			else
			{
				Triggers.FinishTrigger();
			}
		}
	}
}