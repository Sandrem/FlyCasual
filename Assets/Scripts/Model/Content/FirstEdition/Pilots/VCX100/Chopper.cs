using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace Ship
{
    namespace FirstEdition.VCX100
    {
        public class Chopper : VCX100
        {
            public Chopper() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Chopper\"",
                    4,
                    37,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ChopperPilotAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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
