using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class Manaroo : JumpMaster5000
        {
            public Manaroo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Manaroo",
                    3,
                    47,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ManarooAbility),
                    charges: 1,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 215
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Engagement Phase, you may choose a friendly ship at range 0-1. If you do, transfer all green tokens assigned to you to that ship.
    public class ManarooAbility : Abilities.FirstEdition.ManarooAbility
    {
        protected override int MinRange
        {
            get
            {
                return 0;
            }
        }

        protected override int MaxRange
        {
            get
            {
                return 1;
            }
        }

        protected override List<Type> ReassignableTokenTypes
        {
            get
            {
                return new List<Type>()
                {
                    typeof(CalculateToken),
                    typeof(EvadeToken),
                    typeof(FocusToken),
                    typeof(ReinforceForeToken),
                    typeof(ReinforceAftToken)
                };
            }
        }

        protected override string SelectTargetMessage
        {
            get
            {
                return "Choose another friendly ship to assign all your green tokens to it";
            }
        }

        protected override string ReassignTokensMessage
        {
            get
            {
                return string.Format("{0}: all green tokens have been reassigned to {1}", HostShip.PilotInfo.PilotName, TargetShip.PilotInfo.PilotName);
            }
        }

        protected override void CheckAbility()
        {
            if (HostShip.Owner.Ships.Count == 1) return;

            //Skip if only 1 friendly ship in range, since Manaroo is always in range 0 of herself
            if (CountFriendlyShipsInRange() == 1) return;

            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, SelectTarget);
        }
    }
}