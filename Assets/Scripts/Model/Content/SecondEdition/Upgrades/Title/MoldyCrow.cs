using Ship;
using Upgrade;
using System.Collections.Generic;
using Tokens;
using Arcs;

namespace UpgradesList.SecondEdition
{
    public class MoldyCrow : GenericUpgrade
    {
        public MoldyCrow() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Moldy Crow",
                UpgradeType.Title,
                cost: 18, 
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.Hwk290LightFreighter.Hwk290LightFreighter)),
                addArc: new ShipArcInfo(ArcType.Front, 3),
                abilityType: typeof(Abilities.SecondEdition.MoldyCrowAbility),
                seImageNumber: 104
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class MoldyCrowAbility : GenericAbility
    {
        private int tokenCount = 0;

        public override void ActivateAbility()
        {
            HostShip.BeforeRemovingTokenInEndPhase += KeepTwoFocusTokens;
            Phases.Events.OnEndPhaseStart_NoTriggers += OnEndPhaseStart_NoTriggers;
        }

        private void OnEndPhaseStart_NoTriggers()
        {
            tokenCount = 0;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeRemovingTokenInEndPhase -= KeepTwoFocusTokens;
            Phases.Events.OnEndPhaseStart_NoTriggers -= OnEndPhaseStart_NoTriggers;
        }

        private void KeepTwoFocusTokens(GenericShip ship, GenericToken token, ref bool remove)
        {
            if (tokenCount < 2) //We can only keep up to two focus tokens in Second Edition
            {
                if (token is FocusToken) remove = false;
                tokenCount++;
            }
            else
            {
                if (token is FocusToken) remove = true;
            }
        }
    }
}