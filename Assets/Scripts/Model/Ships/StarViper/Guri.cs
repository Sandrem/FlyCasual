﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using Abilities;
using Tokens;
using RuleSets;
using ActionsList;

namespace Ship
{
    namespace StarViper
    {
        public class Guri : StarViper, ISecondEditionPilot
        {
            public Guri() : base()
            {
                PilotName = "Guri";
                PilotSkill = 5;
                Cost = 30;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new GuriAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                Cost = 62;

                ActionBar.RemovePrintedAction(typeof(FocusAction));
                ActionBar.AddPrintedAction(new CalculateAction());

                ActionBar.RemoveActionLink(typeof(BarrelRollAction), typeof(FocusAction));
                ActionBar.AddActionLink(typeof(BarrelRollAction), new CalculateAction() { IsRed = true });

                ActionBar.RemoveActionLink(typeof(BoostAction), typeof(FocusAction));
                ActionBar.AddActionLink(typeof(BoostAction), new CalculateAction() { IsRed = true });

                SEImageNumber = 178;
            }
        }
    }
}

namespace Abilities
{
    public class GuriAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterGuriAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterGuriAbility;
        }

        private void RegisterGuriAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskGuriAbility);
        }

        private void AskGuriAbility(object sender, EventArgs e)
        {
            if (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 1), Team.Type.Enemy).Count > 0)
            {
                if (!alwaysUseAbility)
                {
                    AskToUseAbility(AlwaysUseByDefault, UseAbility, null, null, true);
                }
                else
                {
                    AssignFocus(Triggers.FinishTrigger);
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            AssignFocus(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void AssignFocus(Action callback)
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), callback);
        }

    }
}

