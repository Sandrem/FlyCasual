using BoardTools;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class MaceWindu : Delta7Aethersprite
    {
        public MaceWindu()
        {
            PilotInfo = new PilotCardInfo(
                "Mace Windu",
                5,
                46,
                true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.MaceWinduAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ModelInfo.SkinName = "Mace Windu";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/89/28/8928ae70-8883-4c39-9b15-a4754c063b88/swz32_mace-windu.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a red maneuver, recover 1 force.
    public class MaceWinduAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (HostShip.GetLastManeuverColor() == MovementComplexity.Complex && !(Board.IsOffTheBoard(HostShip) || HostShip.IsBumped))
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AssignTokens);
            }
        }
        
        private void AssignTokens(object sender, EventArgs e)
        {
            if (HostShip.State.Force < HostShip.State.MaxForce) HostShip.State.Force++;
            Triggers.FinishTrigger();
        }
    }
}
