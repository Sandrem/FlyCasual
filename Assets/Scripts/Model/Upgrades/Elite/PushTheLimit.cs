using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
	public class PushTheLimit : GenericUpgrade
	{

		public PushTheLimit() : base()
		{
			Type = UpgradeType.Elite;
			Name = ShortName = "Push The Limit";
			ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/5/59/Push_The_Limit.png";
			Cost = 3;
		}

		public override void AttachToShip(Ship.GenericShip host)
		{
			base.AttachToShip(host);

			host.OnActionDecisionSubphaseEnd += DoSecondAction;
		}

		private void DoSecondAction(Ship.GenericShip ship)
		{
			if (!ship.HasToken(typeof(Tokens.StressToken)) || ship.CanPerformActionsWhileStressed)
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
			List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();
			base.Host.AskPerformFreeAction(actions, AddStressToken);
		}

		private void AddStressToken()
		{
			if (!base.Host.IsFreeActionSkipped) {
				base.Host.AssignToken (new Tokens.StressToken(), delegate {
					Phases.CurrentSubPhase.CallBack();
					Triggers.FinishTrigger();
				});	
			}
			else
			{
				Triggers.FinishTrigger();
			}
		}
	}
}