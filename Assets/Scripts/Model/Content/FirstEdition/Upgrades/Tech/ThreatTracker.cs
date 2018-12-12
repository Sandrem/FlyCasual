using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ThreatTracker : GenericUpgrade
    {
        public ThreatTracker() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Threat Tracker",
                UpgradeType.Tech,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.ThreatTrackerAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ThreatTrackerAbility : GenericAbility
    {
        private GenericShip threatTrackerTarget = null;
        private List<GenericAction> threatTrackerActions = new List<GenericAction>();
        private char targetLock = ' ';

        public override void ActivateAbility()
        {
            GenericShip.OnCombatActivationGlobal += CheckThreatTrackerAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnCombatActivationGlobal -= CheckThreatTrackerAbility;
        }

        public void ThreatTrackerCallback()
        {
            HostShip.Tokens.SpendToken(typeof(BlueTargetLockToken), delegate
            {
                Selection.ChangeActiveShip(threatTrackerTarget);
                Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                Triggers.FinishTrigger();
            }, targetLock);
        }

        private void CheckThreatTrackerAbility(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;

            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapons);
            if (!shotInfo.InArc || shotInfo.Range >= 3) return;

            if (!ActionsHolder.HasTargetLockOn(HostShip, ship)) return;

            threatTrackerTarget = ship;

            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskThreatTrackerAbility);
        }

        private void AskThreatTrackerAbility(object sender, System.EventArgs e)
        {
            List<GenericAction> freeActions = new List<GenericAction>() { new BoostAction(), new BarrelRollAction() };
            threatTrackerActions.Clear();
            freeActions.ForEach(delegate (GenericAction action)
            {
                if (HostShip.ActionBar.HasAction(action.GetType()))
                {
                    threatTrackerActions.Add(action);
                }
            });

            if (threatTrackerActions.Count > 0)
            {
                AskToUseAbility(NeverUseByDefault, PerformThreatTracker, DontUseAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void DontUseAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
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