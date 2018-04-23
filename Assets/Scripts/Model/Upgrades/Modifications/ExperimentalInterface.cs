using ActionsList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;

namespace UpgradesList
{
	public class ExperimentalInterface : GenericUpgrade
	{
        private bool IsUsed = false;

		public ExperimentalInterface() : base()
		{
            Types.Add(UpgradeType.Modification);
			Name = "Experimental Interface";
			Cost = 3;

            isUnique = true;

            AvatarOffset = new Vector2(55, 4);

            IsHidden = true;
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
            if (!IsUsed)
            {
                Host.OnActionDecisionSubphaseEnd += DoSecondAction;
            }
        }

		private void DoSecondAction(GenericShip ship)
		{
            Host.OnActionDecisionSubphaseEnd -= DoSecondAction;

            if (!ship.Tokens.HasToken(typeof(Tokens.StressToken)) || ship.CanPerformActionsWhileStressed)
			{
                IsUsed = true;
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Push The Limit Action",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnFreeAction,
                        EventHandler = PerformExperimentalInterfaceAction
                    }
                );
			}
		}

		private void PerformExperimentalInterfaceAction(object sender, System.EventArgs e)
		{
			base.Host.GenerateAvailableActionsList();
			List<GenericAction> actions = Selection.ThisShip.GetAvailableActionsList().Where(n => n.Source != null).ToList();
			base.Host.AskPerformFreeAction(actions, AddStressToken);
		}

        private void Cleanup()
        {
            IsUsed = false;
        }

		private void AddStressToken()
		{
			if (!base.Host.IsFreeActionSkipped) {
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