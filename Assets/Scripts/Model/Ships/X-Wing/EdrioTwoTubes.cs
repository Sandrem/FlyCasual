using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using RuleSets;
using Tokens;

namespace Ship
{
    namespace XWing
    {
        public class EdrioTwoTubes : XWing, ISecondEditionPilot
        {
            public EdrioTwoTubes() : base()
            {
                PilotName = "Edrio Two Tubes";
                PilotSkill = 4;
                Cost = 24;

                IsUnique = true;

                SkinName = "Partisan";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.EdrioTwoTubesAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 59;
            }
        }
    }
}

namespace Abilities
{
    public class EdrioTwoTubesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementActivation += CheckAbilityConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivation -= CheckAbilityConditions;
        }

        private void CheckAbilityConditions(GenericShip ship)
        {
            if (HostShip.Tokens.HasToken(typeof(FocusToken))) RegisterEdrioTwoTubesTrigger();
        }

        private void RegisterEdrioTwoTubesTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementActivation, AskToPerfromFreeAction);
        }

        private void AskToPerfromFreeAction(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(HostShip.GetAvailableActionsList(), Triggers.FinishTrigger);
        }
    }
}