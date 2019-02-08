using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class ObiWanKenobi : Delta7Aethersprite
    {
        public ObiWanKenobi()
        {
            PilotInfo = new PilotCardInfo(
                "Obi-Wan Kenobi",
                5,
                65,
                true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.ObiWanKenobiAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f9/24/f9246e39-4852-4a8f-a331-9b78f62439e9/swz32_obi-wan-kenobi.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After a friendly ship at range 0-2 spends a focus token, you may spend force. If you do, that ship gains 1 focus token.
    public class ObiWanKenobiAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsSpentGlobal += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsSpentGlobal -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, Type tokenType)
        {
            if (HostShip.State.Force > 0 
                && ship.Owner == HostShip.Owner 
                && tokenType == typeof(Tokens.FocusToken) 
                && new BoardTools.DistanceInfo(ship, HostShip).Range < 3)
            {
                TargetShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, AskToUseObiWanAbility);
            }
        }

        private void AskToUseObiWanAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                AlwaysUseByDefault,
                UseAbility
            );
        }

        private void UseAbility(object sender, EventArgs e)
        {
            HostShip.State.Force--;
            TargetShip.Tokens.AssignToken(new Tokens.FocusToken(TargetShip), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}
