﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using RuleSets;

// Second->First: Two same actions
// Triggers are empty

namespace Ship
{
    namespace TIEAdvanced
    {
        public class DarthVader : TIEAdvanced, ISecondEditionPilot
        {
            public DarthVader() : base()
            {
                PilotName = "Darth Vader";
                PilotSkill = 9;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Blue";

                PilotAbilities.Add(new Abilities.DarthVaderAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 6;
                MaxForce = 3;
                ImageUrl = "https://i.imgur.com/Budwwlp.png";
                Cost = 50; // TODO: Change
                //TODO: Change ability
            }
        }
    }
}

namespace Abilities
{
    public class DarthVaderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionDecisionSubphaseEnd += DoSecondAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionDecisionSubphaseEnd -= DoSecondAction;
        }

        private void DoSecondAction(GenericShip ship)
        {
            if (!HostShip.Tokens.HasToken(typeof(Tokens.StressToken)) && Phases.CurrentSubPhase.GetType() == typeof(SubPhases.ActionDecisonSubPhase))
            {
                RegisterAbilityTrigger(TriggerTypes.OnFreeActionPlanned, PerformFreeAction);
            }
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            HostShip.GenerateAvailableActionsList();
            List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();

            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}
