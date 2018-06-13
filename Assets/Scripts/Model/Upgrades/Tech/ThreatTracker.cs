using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using Abilities;
using BoardTools;
using SubPhases;
using UpgradesList;
using ActionsList;
using System;

namespace UpgradesList
{

    public class ThreatTracker : GenericUpgrade
    {
        public ThreatTracker() : base()
        {
            Types.Add(UpgradeType.Tech);
            Name = "Threat Tracker";
            Cost = 3;

            UpgradeAbilities.Add(new ThreatTrackerAbility());
        }
    }

}

namespace Abilities
{
    public class ThreatTrackerAbility : GenericAbility
    {
        private GenericShip threatTrackerTarget = null;
        private List<GenericAction> threatTrackerActions = new List<GenericAction>();

        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishGlobal += CheckThreatTrackerAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishGlobal -= CheckThreatTrackerAbility;
        }

        public void ThreatTrackerCallback()
        {
            Selection.ChangeActiveShip(threatTrackerTarget);
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Triggers.FinishTrigger();
        }

        private void CheckThreatTrackerAbility(GenericShip ship)
        {
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                threatTrackerTarget = ship;
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskThreatTrackerAbility);
            }
        }

        private void AskThreatTrackerAbility(object sender, System.EventArgs e)
        {
            
            ShotInfo shotInfo = new ShotInfo(HostShip, threatTrackerTarget, HostShip.PrimaryWeapon);
            List<GenericAction> actionBar = HostShip.GetActionsFromActionBar();
            List<GenericAction> freeActions = new List<GenericAction>() { new ActionsList.BoostAction(), new ActionsList.BarrelRollAction() };
            threatTrackerActions.Clear();
            freeActions.ForEach(delegate (GenericAction action)
            {
                if (actionBar.Exists(barAction => barAction.GetType() == action.GetType()))
                {
                    threatTrackerActions.Add(action);
                }
            });

            if (threatTrackerActions.Count > 0 && shotInfo.InArc && shotInfo.Range <= 2)
            {
                AskToUseAbility(NeverUseByDefault, PerformThreatTracker);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void PerformThreatTracker(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);
            HostShip.AskPerformFreeAction(
                threatTrackerActions,
                ThreatTrackerCallback
            );
        }
    }
}
