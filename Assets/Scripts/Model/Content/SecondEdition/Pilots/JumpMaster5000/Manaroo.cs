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
        protected override void CheckAbility()
        {
            if (HostShip.Owner.Ships.Count == 1) return;

            if (CountFriendlyShipsInRange(0, 1) == 1) return;

            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, SelectTarget);
        }

        protected override string GetSelectTargetDescription()
        {
            return "Choose another friendly ship to assign all your green tokens to it";
        }

        protected override bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 0, 1);
        }

        protected override string GetReassignTokensFormatString()
        {
            return "{0}: all green tokens have been reassigned to {1}";
        }

        protected override List<Type> GetReassignableTokenTypes()
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
}