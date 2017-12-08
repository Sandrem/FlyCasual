using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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

        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            HostShip.OnCombatPhaseStart += RegisterPilotAbility;
        }

        private void RegisterPilotAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AssignStressTokens);
        }

        private void AssignStressTokens(object sender, System.EventArgs e)
        {
            shipsToAssignStress = new List<GenericShip>(HostShip.ShipsBumped);
            AssignStressTokenRecursive();
        }

        private void AssignStressTokenRecursive()
        {
            if (shipsToAssignStress.Count > 0)
            {
                GenericShip shipToAssignStress = shipsToAssignStress[0];
                shipsToAssignStress.Remove(shipToAssignStress);
                Messages.ShowErrorToHuman(shipToAssignStress.PilotName + " is bumped into \"Chopper\" and gets Stress");
                shipToAssignStress.AssignToken(new Tokens.StressToken(), AssignStressTokenRecursive);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
