using Ship;
using Upgrade;
using System.Collections.Generic;
using System;
using SubPhases;
using Movement;
using GameModes;
using Tokens;
using System.Linq;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class AdaptiveAilerons : GenericUpgrade
    {
        public AdaptiveAilerons() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Adaptive Ailerons",
                UpgradeType.Title,
                cost: 0,          
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIEStriker.TIEStriker)),
                abilityType: typeof(Abilities.FirstEdition.AdaptiveAileronsAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class AdaptiveAileronsAbility : GenericAbility
    {
        private GenericMovement SavedManeuver;

        private static readonly List<string> ChangedManeuversCodes = new List<string>() { "1.L.B", "1.F.S", "1.R.B" };
        private Dictionary<string, MovementComplexity> SavedManeuverColors;

        bool doAilerons = false;

        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed += RegisterAdaptiveAileronsAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed -= RegisterAdaptiveAileronsAbility;
        }

        private void RegisterAdaptiveAileronsAbility(GenericShip ship)
        {
            // AI doesn't know how to boost
            if (HostShip.Owner.GetType().IsSubclassOf(typeof(Players.GenericAiPlayer))) return;

            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsReadyToBeRevealed, CheckCanUseAbility);
        }

        private void CheckCanUseAbility(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasToken(typeof(StressToken)) && (HostShip.PilotInfo.PilotName != "\"Duchess\""))
            {
                Triggers.FinishTrigger();
            }
            else
            {
                DoAdaptiveAileronsAbility();
            }
        }

        private void DoAdaptiveAileronsAbility()
        {
            SavedManeuver = HostShip.AssignedManeuver;

            SavedManeuverColors = new Dictionary<string, MovementComplexity>();
            foreach (var changedManeuver in ChangedManeuversCodes)
            {
                SavedManeuverColors.Add(changedManeuver, HostShip.DialInfo.PrintedDial.First(n => n.Key.ToString() == changedManeuver).Value);
                HostShip.Maneuvers[changedManeuver] = MovementComplexity.Normal;
            }

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Ailerons Planning",
                    TriggerType = TriggerTypes.OnAbilityDirect,
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    EventHandler = CheckForDuchess
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, ExecuteSelectedManeuver);
        }

        private void CheckForDuchess(object sender, EventArgs e)
        {
            if (HostShip.PilotInfo.PilotName == "\"Duchess\"")
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    NeverUseByDefault,
                    UseAilerons,
                    DontUseAilerons,
                    descriptionLong: "Do you want to activate your Adaptive Ailerons?",
                    imageHolder: HostShip
                );
            }
            else
            {
                SelectAdaptiveAileronsManeuver(sender, e);
            }
        }

        private void DontUseAilerons(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private void UseAilerons(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            SelectAdaptiveAileronsManeuver(sender, e);
        }

        private void SelectAdaptiveAileronsManeuver(object sender, EventArgs e)
        {
            doAilerons = true;
            HostShip.Owner.ChangeManeuver(
                ShipMovementScript.SendAssignManeuverCommand,
                Triggers.FinishTrigger,
                AdaptiveAileronsFilter
            );
        }

        private void RestoreManuverColors(GenericShip ship)
        {
            foreach (var changedManeuver in ChangedManeuversCodes)
            {
                HostShip.Maneuvers[changedManeuver] = SavedManeuverColors[changedManeuver];
            }
        }

        private void ExecuteSelectedManeuver()
        {
            if (doAilerons)
            {
                HostShip.AssignedManeuver.IsRevealDial = false;
                HostShip.AssignedManeuver.GrantedBy = "Ailerons";
                HostShip.CanPerformActionsWhenBumped = true;
                HostShip.CanPerformActionsWhenOverlapping = true;
                ShipMovementScript.LaunchMovement(FinishAdaptiveAileronsAbility);
            }
            else
            {
                FinishAdaptiveAileronsAbility();
            }
        }

        private void FinishAdaptiveAileronsAbility()
        {
            doAilerons = false;
            HostShip.CanPerformActionsWhenBumped = false;
            HostShip.CanPerformActionsWhenOverlapping = false;
            RestoreManuverColors(HostShip);
            Phases.CurrentSubPhase.IsReadyForCommands = true;
            //ship may have flown off the board; only assign saved maneuver if ship is exists
            if (Roster.GetShipById("ShipId:" + Selection.ThisShip.ShipId) != null)
            {
                ManeuverSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<ManeuverSelectionSubphase>(
                    "Select a maneuver",
                    Triggers.FinishTrigger
                );
                subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
                subphase.Start();
                subphase.IsReadyForCommands = true;

                ShipMovementScript.SendAssignManeuverCommand(SavedManeuver.ToString());
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool AdaptiveAileronsFilter(string maneuverString)
        {
            GenericMovement movement = ShipMovementScript.MovementFromString(maneuverString, HostShip);
            if (movement.ManeuverSpeed != ManeuverSpeed.Speed1) return false;
            if (movement.Bearing == ManeuverBearing.Straight || movement.Bearing == ManeuverBearing.Bank) return true;

            return false;
        }
    }
}
