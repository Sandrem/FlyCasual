﻿using Ship;
using Ship.TIEStriker;
using Upgrade;
using System.Collections.Generic;
using System;
using UpgradesList;
using SubPhases;
using ActionsList;
using Abilities;
using Movement;
using GameModes;
using Tokens;

namespace UpgradesList
{
    public class AdaptiveAilerons : GenericUpgrade
    {
        public AdaptiveAilerons() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Adaptive Ailerons";
            Cost = 0;

            UpgradeAbilities.Add(new AdaptiveAileronsAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEStriker;
        }
    }
}

namespace Abilities
{
    public class AdaptiveAileronsAbility : GenericAbility
    {
        private GenericMovement SavedManeuver;

        private static readonly List<string> ChangedManeuversCodes = new List<string>() { "1.L.B", "1.F.S", "1.R.B" };
        private Dictionary<string, MovementComplexity> SavedManeuverColors;

        bool doAilerons = true;

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
            if (HostShip.Owner.UsesHotacAiRules) return;

            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsReadyToBeRevealed, CheckCanUseAbility);
        }

        private void CheckCanUseAbility(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasToken(typeof(StressToken)) && !(HostShip is Duchess))
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
                SavedManeuverColors.Add(changedManeuver, HostShip.Maneuvers[changedManeuver]);
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
            if (HostShip is Duchess)
            {
                AskToUseAbility(AlwaysUseByDefault, UseAbility, DontUseAbility);
            }
            else
            {
                SelectAdaptiveAileronsManeuver(sender, e);
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            doAilerons = false;
            DecisionSubPhase.ConfirmDecision();
        }

        private void DontUseAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            SelectAdaptiveAileronsManeuver(sender, e);
        }

        private void SelectAdaptiveAileronsManeuver(object sender, EventArgs e)
        {
            HostShip.Owner.ChangeManeuver(
                (maneuverCode) => {
                    GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
                    HostShip.OnMovementFinish += RestoreManuverColors;
                },
                AdaptiveAileronsFilter
            );
        }

        private void RestoreManuverColors(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RestoreManuverColors;

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
                HostShip.AssignedManeuver.GrantedBy = "Ailerons"; ;
                ShipMovementScript.LaunchMovement(FinishAdaptiveAileronsAbility);
            }
            else
            {
                doAilerons = true;
                FinishAdaptiveAileronsAbility();
            }
        }

        private void FinishAdaptiveAileronsAbility()
        {
            ShipMovementScript.SendAssignManeuverCommand(Selection.ThisShip.ShipId, SavedManeuver.ToString());
            //GameMode.CurrentGameMode.AssignManeuver(SavedManeuver.ToString());
            // It calls Triggers.FinishTrigger
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