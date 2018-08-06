using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using System.Linq;
using Tokens;
using RuleSets;

namespace Ship
{
    namespace Vcx100
    {
        public class Chopper : Vcx100, ISecondEditionPilot
        {
            public Chopper() : base()
            {
                PilotName = "\"Chopper\"";
                PilotSkill = 4;
                Cost = 37;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.ChopperPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 72;
                PilotAbilities.RemoveAll(ability => ability is Abilities.ChopperPilotAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.ChopperPilotAbilitySE());
            }
        }
    }
}

namespace Abilities
{
    public class ChopperPilotAbility : GenericAbility
    {
        protected List<GenericShip> shipsToAssignStress;

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterPilotAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterPilotAbility;
        }

        private void RegisterPilotAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AssignStressTokens);
        }

        private void AssignStressTokens(object sender, System.EventArgs e)
        {
            shipsToAssignStress = new List<GenericShip>(HostShip.ShipsBumped.Where(n => n.Owner.PlayerNo != HostShip.Owner.PlayerNo));
            AssignStressTokenRecursive();
        }

        protected virtual void AssignStressTokenRecursive()
        {
            if (shipsToAssignStress.Count > 0)
            {
                GenericShip shipToAssignStress = shipsToAssignStress[0];
                shipsToAssignStress.Remove(shipToAssignStress);
                Messages.ShowErrorToHuman(shipToAssignStress.PilotName + " is bumped into \"Chopper\" and gets Stress");
                shipToAssignStress.Tokens.AssignToken(typeof(StressToken), AssignStressTokenRecursive);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChopperPilotAbilitySE : ChopperPilotAbility
    {
        protected override void AssignStressTokenRecursive()
        {
            if(shipsToAssignStress.Count > 0)
            {
                GenericShip shipToAssignStress = shipsToAssignStress[0];
                shipsToAssignStress.Remove(shipToAssignStress);
                Messages.ShowErrorToHuman(shipToAssignStress.PilotName + " is bumped into \"Chopper\" and gets a jam token.");
                shipToAssignStress.Tokens.AssignTokens(() => new JamToken(shipToAssignStress), 2, AssignStressTokenRecursive);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}