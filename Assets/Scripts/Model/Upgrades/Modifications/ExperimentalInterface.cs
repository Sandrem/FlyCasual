using ActionsList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using Tokens;
using Abilities;

namespace UpgradesList
{
    public class ExperimentalInterface : GenericUpgrade
    {
        public ExperimentalInterface() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Experimental Interface";
            Cost = 3;

            isUnique = true;

            Avatar = new AvatarInfo(Faction.None, new Vector2(55, 4));

            UpgradeAbilities.Add(new ExperimentalInterfaceAbility());
        }
    }
}

namespace Abilities
{
    public class ExperimentalInterfaceAbility : GenericAbility
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
                    Name = "Experimental Interface Action",
                    TriggerOwner = ship.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeAction,
                    EventHandler = PerformExperimentalInterfaceAction
                }
            );
        }

        private void PerformExperimentalInterfaceAction(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = HostShip.GetAvailableActions();
            List<GenericAction> notActionBarActions = actions.Where(n => !n.IsInActionBar).ToList();
            HostShip.AskPerformFreeAction(notActionBarActions, AddStressToken);
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
            // Reset after every Experimental Interface opportunity
            consecutiveActions = 0;
        }
    }
}