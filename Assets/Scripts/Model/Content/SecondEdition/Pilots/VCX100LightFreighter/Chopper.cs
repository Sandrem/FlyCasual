using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace Ship
{
    namespace SecondEdition.VCX100LightFreighter
    {
        public class Chopper : VCX100LightFreighter
        {
            public Chopper() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Chopper\"",
                    2,
                    68,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ChopperPilotAbility),
                    seImageNumber: 75
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChopperPilotAbility : Abilities.FirstEdition.ChopperPilotAbility
    {
        protected override void AssignStressTokenRecursive()
        {
            if (shipsToAssignStress.Count > 0)
            {
                GenericShip shipToAssignStress = shipsToAssignStress[0];
                shipsToAssignStress.Remove(shipToAssignStress);
                Messages.ShowErrorToHuman(shipToAssignStress.PilotInfo.PilotName + " is at range 0 of " + HostShip.PilotInfo.PilotName + " and gains 2 jam tokens");
                shipToAssignStress.Tokens.AssignTokens(() => new JamToken(shipToAssignStress, HostShip.Owner), 2, AssignStressTokenRecursive);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
