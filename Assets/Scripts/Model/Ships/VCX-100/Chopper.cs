using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using System.Linq;

namespace Ship
{
    namespace Vcx100
    {
        public class Chopper : Vcx100
        {
            public Chopper() : base()
            {
                PilotName = "\"Chopper\"";
                PilotSkill = 4;
                Cost = 37;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.ChopperPilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class ChopperPilotAbility : GenericAbility
    {
        private List<GenericShip> shipsToAssignStress;

        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers += RegisterPilotAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers -= RegisterPilotAbility;
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

        private void AssignStressTokenRecursive()
        {
            if (shipsToAssignStress.Count > 0)
            {
                GenericShip shipToAssignStress = shipsToAssignStress[0];
                shipsToAssignStress.Remove(shipToAssignStress);
                Messages.ShowErrorToHuman(shipToAssignStress.PilotName + " is bumped into \"Chopper\" and gets Stress");
                shipToAssignStress.Tokens.AssignToken(new Tokens.StressToken(shipToAssignStress), AssignStressTokenRecursive);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
