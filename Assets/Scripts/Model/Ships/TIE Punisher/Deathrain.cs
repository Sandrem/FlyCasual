using ActionsList;
using RuleSets;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace TIEPunisher
    {
        public class Deathrain : TIEPunisher, ISecondEditionPilot
        {
            public Deathrain() : base()
            {
                PilotName = "\"Deathrain\"";
                PilotSkill = 4;
                Cost = 42;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new Abilities.SecondEdition.DeathrainAbilitySE());

                SEImageNumber = 140;
            }

            public void AdaptPilotToSecondEdition()
            {
                
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DeathrainAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnBombWasDropped += CheckAbilityOnDrop;
            HostShip.OnBombWasLaunched += CheckAbilityOnLaunch;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBombWasDropped -= CheckAbilityOnDrop;
            HostShip.OnBombWasLaunched -= CheckAbilityOnLaunch;
        }

        private void CheckAbilityOnDrop()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBombWasDropped, AskToPerformFreeAction);
        }

        private void CheckAbilityOnLaunch()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBombWasLaunched, AskToPerformFreeAction);
        }

        private void AskToPerformFreeAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("\"Deathrain\" can perform an action");

            HostShip.AskPerformFreeAction(HostShip.GetAvailableActions(), Triggers.FinishTrigger);
        }
    }
}
