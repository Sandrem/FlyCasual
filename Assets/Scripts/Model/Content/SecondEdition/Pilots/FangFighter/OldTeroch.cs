using BoardTools;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class OldTeroch : FangFighter
        {
            public OldTeroch() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Old Teroch",
                    5,
                    56,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.OldTerochAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 156
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Engagement Phase, you may choose 1 enemy ship at range 1. If you do and you are in its (front arc), it removes all of its green tokens.
    public class OldTerochAbility : Abilities.FirstEdition.OldTerochAbility
    {
        public OldTerochAbility()
        {
            AbilityDescription = "Choose a ship. If you are inside its firing arc, it removes all of its green tokens.";
        }

        protected override void DiscardTokens()
        {
            Messages.ShowInfo(string.Format("{0} removed all green tokens from {1}", HostShip.PilotName, TargetShip.PilotName));
            TargetShip.Tokens.RemoveAllTokensByColor(TokenColors.Green, SelectShipSubPhase.FinishSelection);
        }
    }
}
