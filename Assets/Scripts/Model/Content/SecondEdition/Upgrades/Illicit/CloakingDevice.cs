using ActionsList;
using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class CloakingDevice : GenericUpgrade
    {
        public CloakingDevice() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Cloaking Device",
                UpgradeType.Illicit,
                cost: 5,
                isLimited: true,
                charges: 2,
                restriction: new BaseSizeRestriction(BaseSize.Small, BaseSize.Medium), 
                seImageNumber: 57,
                abilityType: typeof(Abilities.SecondEdition.CloakingDeviceAbility)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class CloakingDeviceAbility : GenericAbility
    {
        bool cloakingDeviceTriggerRegistered = false;
        bool cloakingDeviceActionEnabled = false;
        Action curCallback = null;

        public override void ActivateAbility()
        {
            // Only activate this ability if our cloaking device has charges and we are not currently cloaked.
            // Note that this is actually adding the action to their action choices.
            if (HostShip.Tokens.HasToken(typeof(CloakToken)) == false)
            {
                foreach (GenericUpgrade cloakingDevice in HostShip.UpgradeBar.GetInstalledUpgrades(UpgradeType.Illicit))
                {
                    if(cloakingDevice.NameCanonical == "cloakingdevice" && cloakingDevice.State.Charges > 0 && cloakingDeviceActionEnabled == false)
                    {
                        HostShip.OnGenerateActions += AddAction;
                        cloakingDeviceActionEnabled = true;
                    }
                }
            }

        }

        public override void DeactivateAbility()
        {
            // Remove this action from their action choices.
            HostShip.OnGenerateActions -= AddAction;
            cloakingDeviceActionEnabled = false;
        }

        protected void AddAction(GenericShip ship)
        {
            // Add this action to the available actions for this ship during the Action phase.
            ship.AddAvailableAction(new GenericAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip,
                Source = HostUpgrade,
                DoAction = DoAction,
                Name = HostName
            });
        }

        protected void DoAction()
        {
            // Perform our Cloaking Device action.  This includes losing a charge, gaining a Cloak token,
            // and making sure we can potentially lose the cloak during the Planning phase.  This also removes
            // Cloaking Device from your available actions while you're cloaked by it.
            HostUpgrade.State.LoseCharge();
            Selection.ThisShip.Tokens.AssignToken(typeof(CloakToken), Phases.CurrentSubPhase.CallBack);
            
            Phases.Events.OnPlanningPhaseStart += RegisterDecloakChance;
            HostShip.OnDecloak += RemoveDecloakChance;
            DeactivateAbility();
        }

        private void RegisterDecloakChance()
        {
            // Set up a trigger for the start of the Planning Phase.  This trigger is where you can potentially
            // decloak earlier than expected.
            if (cloakingDeviceTriggerRegistered == false)
            {
                RegisterAbilityTrigger(TriggerTypes.OnPlanningSubPhaseStart, CheckAutoDecloak);
                cloakingDeviceTriggerRegistered = true;
            }
        }

        private void RemoveDecloakChance()
        {
            // This function is called when we decloak.  It removes the trigger for accidentally decloaking during the 
            // planning phase, regardless of how we decloaked.  It also allows the ship to take the Cloaking Device action
            // again, as long as it has a charge.
            Phases.Events.OnPlanningPhaseStart -= RegisterDecloakChance;

            HostShip.OnDecloak -= RemoveDecloakChance;
            ActivateAbility();
        }

        private void CheckAutoDecloak(object sender, EventArgs e)
        {
            // Test to see if we decloak unexpectedly.  This function sets up the test.  DieCheckFinish completes the test.
            // Note that this test doesn't run if we don't have a cloak token.
            curCallback = Phases.CurrentSubPhase.CallBack;
            cloakingDeviceTriggerRegistered = false;
            if (HostShip.Tokens.HasToken(typeof(CloakToken)) == false)
            {
                cloakingDeviceTriggerRegistered = false;
                Triggers.FinishTrigger();
            }
            else
            {
                Messages.ShowInfo("Cloaking Device: " + HostShip.PilotInfo.PilotName + " is checking for cloak deactivation.");
                PerformDiceCheck(
                    HostName,
                    DiceKind.Attack,
                    1,
                    DiceCheckFinished,
                    Triggers.FinishTrigger);
            }
        }

        protected virtual void DiceCheckFinished()
        {
            // Check our results for accidental decloaking.  If we got a focus result, we have to decloak.
            if(DiceCheckRoll.Focuses > 0)
            {
                Messages.ShowInfo("Cloaking Device: " + HostShip.PilotInfo.PilotName + "'s cloaking device deactivates.");
                ShowDecloakDecision(AbilityDiceCheck.ConfirmCheck);
            }
            else
            {
                // We aren't automatically decloaking.  As our trigger was just finished, we'll need to re-register it.
                AbilityDiceCheck.ConfirmCheck();
                Phases.Events.OnPlanningPhaseStart += RegisterDecloakChance;
            }
        }

        private void ShowDecloakDecision(Action callback)
        {
            // We have accidentally decloaked.  We have two choices.  1.  Just remove our cloak token.  2.  Decloak in the normal fashion
            // (i.e. decloak with a 2 straight forward or 2 straight to either side).
            var selectionSubPhase = (CloakingDeviceDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Cloaking Device - Discard or Reposition",
                typeof(CloakingDeviceDecisionSubPhase),
                callback
            );

            selectionSubPhase.InfoText = String.Format("You must decloak.  Do you want to discard your cloak token or reposition and decloak?");
            selectionSubPhase.AddDecision("Discard the Cloak token", delegate { DiscardCloakToken(); DecisionSubPhase.ConfirmDecisionNoCallback(); });

            selectionSubPhase.AddDecision("Reposition and decloak", delegate {DecisionSubPhase.ConfirmDecisionNoCallback(); DecloakNormally(); });

            selectionSubPhase.DefaultDecisionName = "Discard the Cloak token";
            selectionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            selectionSubPhase.Start();
        }

        private class CloakingDeviceDecisionSubPhase : DecisionSubPhase { }

        private void DiscardCloakToken()
        {
            // We chose to just discard our cloak token.
            HostShip.Tokens.RemoveToken(typeof(CloakToken), DecisionSubPhase.ConfirmDecisionNoCallback);
            FinishCloakingDeviceDecloak();
        }

        private void DecloakNormally()
        {
            // We chose to decloak normally.  Utilize the DecloakSubPhases code to handle our decloak.  This keeps us consistent even if
            // decloaking changes in the future.
            if (Selection.ThisShip != HostShip)
            {
                Selection.ThisShip = HostShip;
            }

            string tempstring = Phases.CurrentPhase.Name;
            tempstring = Phases.CurrentSubPhase.Name;
            var decloakSubPhase = (DecloakPlanningSubPhase)Phases.StartTemporarySubPhaseNew(
                "Decloak",
                typeof(DecloakPlanningSubPhase),
                FinishCloakingDeviceDecloak
            );
            decloakSubPhase.Start();
        }

        protected void FinishCloakingDeviceDecloak()
        {
            // Finish up this Cloaking Device call.
            cloakingDeviceTriggerRegistered = false;
            ActivateAbility();
            DecisionSubPhase.ConfirmDecision();
        }
    }
}

